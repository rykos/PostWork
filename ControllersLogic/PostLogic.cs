using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PostWork.Data;
using PostWork.Models;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PostWork.ControllersLogic
{
    public class PostLogic : IPostLogic
    {
        private readonly PostContext postContext;
        public PostLogic(PostContext postContext) => this.postContext = postContext;

        public async Task<Post> CreatePost(IFormCollection data, string creatorId)
        {
            if (this.ValidateData(data, new string[] { "title", "description", "tags" }, 1))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    data.Files[0].CopyTo(ms);
                    Post newPost = new Post()
                    {
                        CreatorId = creatorId,
                        Title = data["title"].ToString(),
                        Description = data["description"].ToString(),
                        Tags = data["tags"].ToString(),
                        Avatar = ms.ToArray(),
                    };
                    if (int.TryParse(data["salaryMin"], out int salaryMin) && int.TryParse(data["salaryMax"], out int salaryMax))
                    {
                        newPost.SalaryMin = salaryMin;
                        newPost.SalaryMax = salaryMax;
                    }
                    var resoult = await postContext.Posts.AddAsync(newPost);
                    await postContext.SaveChangesAsync();
                    return resoult.Entity;
                }
            }
            else
            {
                throw new PostCreationException();
            }
        }

        public void EditPost(IFormCollection data)
        {
            Post post = this.postContext.Posts.FirstOrDefault(x => x.Id == int.Parse(data["id"]));
            if (post != default)
            {
                this.postContext.Posts.Update(post);
                post.Title = data["title"];
                post.Description = data["description"];
                if (int.TryParse(data["salaryMin"], out int SalaryMin))
                {
                    post.SalaryMin = SalaryMin;
                }
                if (int.TryParse(data["salaryMax"], out int SalaryMax))
                {
                    post.SalaryMax = SalaryMax;
                }
                post.Tags = data["tags"];
                if (data.Files.Count > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        data.Files[0].CopyTo(ms);
                        post.Avatar = ms.ToArray();
                    }
                }
                this.postContext.SaveChanges();
                Console.WriteLine("Post edited");
            }
        }

        public Post ReadPost(int id)
        {
            return this.postContext.Posts.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<Post>> FindByTags(string[] tags)
        {
            List<Task> tasks = new List<Task>();
            HashSet<Post> uniquePosts = new HashSet<Post>();
            foreach (string tag in tags)
            {
                Task task = Task.Run(() =>
                {
                    Post[] posts;
                    lock (this.postContext)
                    {
                        posts = this.postContext.Posts.Where(x => x.Tags.Contains(tag)).ToArray();
                    }
                    foreach (Post post in posts)
                    {
                        uniquePosts.Add(post);
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
            tasks.Clear();
            foreach (Post post in uniquePosts)//If does not contains all tags, remove from hash
            {
                tasks.Add(Task.Run(() =>
                {
                    foreach (string tag in tags)
                    {
                        if (post.Tags.Contains(tag) == false)
                        {
                            lock (uniquePosts)
                            {
                                uniquePosts.Remove(post);
                            }
                            break;
                        }
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            return uniquePosts;
        }

        public Post[] FindByCreatorId(string id)
        {
            return this.postContext.Posts.Where(x => x.CreatorId == id).ToArray();
        }

        public Post FindById(int id)
        {
            return this.postContext.Posts.FirstOrDefault(x => x.Id == id);
        }

        public void MakeSubmission(int postId, IFormCollection data)
        {
            if (this.ValidateData(data, new string[] { "name", "email" }, 0))//Form data is valid
            {
                if (this.postContext.Posts.FirstOrDefault(x => x.Id == postId) != null)//Post exists
                {
                    Submission submission = new Submission()
                    {
                        PostId = postId,
                        Name = data["name"].ToString(),
                        Email = data["email"].ToString(),
                        Message = data["message"].ToString()
                    };
                    if (data.Files.Count() > 0)
                    {
                        IFormFile file = data.Files[0];
                        if (file.Length < 15000)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                submission.Cv = ms.ToArray();
                            }
                        }
                    }
                    else
                    {
                        submission.Cv = new byte[0];
                    }
                    this.postContext.Submissions.Add(submission);
                    this.postContext.SaveChanges();
                }
            }
        }

        //Validates post data
        private bool ValidateData(IFormCollection data, string[] requiredFields, int filesRequired)
        {
            foreach (string field in requiredFields)
            {
                if (this.ValidateField(data[field]) == false)//Field is invalid
                {
                    return false;
                }
            }
            if (data.Files.Count < filesRequired)
            {
                return false;
            }
            return true;
        }

        //Checks if field contains minimum 1 value
        private bool ValidateField(StringValues field)
        {
            if (field.Count > 0 && field[0].Length > 0)
            {
                return true;
            }
            return false;
        }
    }

    public interface IPostLogic
    {
        Task<Post> CreatePost(IFormCollection data, string creatorId);
        Post ReadPost(int id);
        Task<IEnumerable<Post>> FindByTags(string[] tags);
        Post[] FindByCreatorId(string id);
        Post FindById(int id);
        void EditPost(IFormCollection data);
        void MakeSubmission(int postId, IFormCollection data);
    }
}