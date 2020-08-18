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
            if (this.ValidateData(data))
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
            return uniquePosts;
        }

        //Validates post data
        private bool ValidateData(IFormCollection data)
        {
            string[] requiredFields = new string[] { "title", "description", "tags" };
            foreach (string field in requiredFields)
            {
                if (this.ValidateField(data[field]) == false)//Field is invalid
                {
                    return false;
                }
            }
            if (data.Files.Count < 1)
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
    }
}