using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PostWork.Data;
using PostWork.Models;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Linq;

namespace PostWork.ControllersLogic
{
    public class PostLogic : IPostLogic
    {
        private readonly PostContext postContext;
        public PostLogic(PostContext postContext) => this.postContext = postContext;

        public async Task CreatePost(IFormCollection data, string creatorId)
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
                    await postContext.Posts.AddAsync(newPost);
                    await postContext.SaveChangesAsync();
                }
            }
            else
            {
                Console.WriteLine("Post validation failed");
                //Throw ex
            }
        }

        public async Task<Post> ReadPost(int id)
        {
            return this.postContext.Posts.FirstOrDefault(x => x.Id == id);
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
        Task CreatePost(IFormCollection data, string creatorId);
        Task<Post> ReadPost(int id);
    }
}