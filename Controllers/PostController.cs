using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostWork.ControllersLogic;
using PostWork.Models;
using Microsoft.AspNetCore.Identity;

namespace PostWork.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostLogic postLogic;
        private readonly UserManager<IdentityUser> userManager;
        public PostController(IPostLogic postLogic, UserManager<IdentityUser> userManager)
        {
            this.postLogic = postLogic;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Read(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Post post = await this.postLogic.ReadPost((int)id);
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
            await this.postLogic.CreatePost(data, this.userManager.GetUserId(User));
            return Ok();
        }
    }
}