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
    [Route("/{action=Index}")]
    [Route("/Posts/{action=Index}/{id?}")]
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
            return View(this.postContext.Posts.ToArray());
        }

        public async Task<IEnumerable<Post>> Find(string query)
        {
            return await this.postLogic.FindByTags(query.Split(','));
        }

        public IActionResult Create()
        {
            return View();
        }

        [Route("/Posts/My")]
        public IActionResult MyPosts()
        {
            return View(postLogic.FindByCreatorId(this.userManager.GetUserId(User)));
        }

        public IActionResult Read(int? id)
        {
            if (id == null)
            {
                return NotFound(id);
            }
            Post post = this.postLogic.ReadPost((int)id);
            if (post == default)
            {
                return NotFound(id);
            }
            return View(post);
        }

        public IActionResult Edit(int id)
        {
            Post post = this.postLogic.FindById(id);
            if (post.CreatorId == this.userManager.GetUserId(User))//User is the creator
            {
                return View(post);
            }
            else//Access denied
            {
                return Forbid();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection data)
        {
            Post post = await this.postLogic.CreatePost(data, this.userManager.GetUserId(User));
            return RedirectToAction("Read", new { id = post.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IFormCollection data)
        {
            Post post = this.postLogic.FindById((int.Parse(data["id"])));
            if (post.CreatorId == this.userManager.GetUserId(User))//User is the creator
            {
                this.postLogic.EditPost(data);
                return RedirectToAction("Read", new { id = post.Id });
            }
            return Forbid();
        }
    }
}