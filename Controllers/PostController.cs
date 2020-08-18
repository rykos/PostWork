using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostWork.ControllersLogic;
using PostWork.Models;
using Microsoft.AspNetCore.Identity;
using PostWork.Data;
using System.Linq;
using System.Collections.Generic;

namespace PostWork.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostLogic postLogic;
        private readonly UserManager<IdentityUser> userManager;
        private readonly PostContext postContext;
        public PostController(IPostLogic postLogic, UserManager<IdentityUser> userManager, PostContext postContext)
        {
            this.postLogic = postLogic;
            this.userManager = userManager;
            this.postContext = postContext;
        }

        public IActionResult Index()
        {
            Post[] elements = this.postContext.Posts.Where(x => x.Tags.Contains("linux")).ToArray();
            return View(elements);
        }

        public async Task<IEnumerable<Post>> Find(string query)
        {
            return await this.postLogic.FindByTags(query.Split(','));
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Read(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Post post = this.postLogic.ReadPost((int)id);
            if (post == default)
            {
                return NotFound();
            }
            return View(post);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection data)
        {
            Post post = await this.postLogic.CreatePost(data, this.userManager.GetUserId(User));
            return Created($"/Post/Read/{post.Id}", post);
        }
    }
}