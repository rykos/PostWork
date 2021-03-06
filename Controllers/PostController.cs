using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostWork.ControllersLogic;
using PostWork.Models;
using Microsoft.AspNetCore.Identity;
using PostWork.Data;
using System.Linq;
using System.Collections.Generic;
using System.IO;

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

        public IActionResult Index(IEnumerable<Post> posts = null)
        {
            if (posts.Count() == 0)
            {
                return View(this.postContext.Posts.ToArray());
            }
            else
            {
                return View(posts);
            }
        }

        [HttpGet]
        public async Task<IEnumerable<Post>> Find(string query)
        {
            return await this.postLogic.GetPostsByTags(query.Split(','));
        }

        public async Task<IActionResult> FindWithView(string query)
        {
            var posts = await this.postLogic.GetPostsByTags(query.Split(','));
            return View("/Views/Post/Index.cshtml", posts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [Route("/Posts/My")]
        public IActionResult MyPosts()
        {
            return View(postLogic.GetPostsByCreatorId(this.userManager.GetUserId(User)));
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
            Post post = this.postLogic.GetPostById(id);
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
            Post post = this.postLogic.GetPostById((int.Parse(data["id"])));
            if (post.CreatorId == this.userManager.GetUserId(User))//User is the creator
            {
                this.postLogic.EditPost(data);
                return RedirectToAction("Read", new { id = post.Id });
            }
            return Forbid();
        }

        [HttpPost]
        public IActionResult Submit(int id, IFormCollection data)//PostId,formData
        {
            this.postLogic.MakeSubmission(id, data, this.userManager.GetUserId(User));
            return RedirectToAction("Index");
        }

        public IActionResult Submissions(int id)
        {
            if (this.postLogic.GetPostById(id).CreatorId != this.userManager.GetUserId(User))
            {
                return Forbid();
            }
            return View(this.postLogic.GetSubmissionsForPostId(id));
        }

        public FileResult DownloadCV(int submissionID)
        {
            Submission submission = this.postLogic.GetSubmissionById(submissionID);
            if (submission != default)
            {
                return File(submission.Cv, "application/octet-stream", submission.CvName);
            }
            else
            {
                return null;
            }
        }
    }
}