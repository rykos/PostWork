using Microsoft.AspNetCore.Mvc;
using PostWork.Models;
using PostWork.Data;
using PostWork.ControllersLogic;
using Microsoft.AspNetCore.Identity;

namespace PostWork.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly IPostLogic postLogic;
        private readonly UserManager<IdentityUser> userManager;
        public SubmissionController(IPostLogic postLogic, UserManager<IdentityUser> userManager)
        {
            this.postLogic = postLogic;
            this.userManager = userManager;
        }

        public IActionResult Review(int id)
        {
            Submission submission = this.postLogic.GetSubmissionById(id);
            if (submission != null)
            {
                return View(submission);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult SetState(string stateString, int SubmissionId)
        {
            SubmissionState state = System.Enum.Parse<SubmissionState>(stateString);
            Submission submission = this.postLogic.GetSubmissionById(SubmissionId);
            if (submission == null)
            {
                return NotFound();
            }
            if (this.postLogic.UserIsPostCreator(this.userManager.GetUserId(User), submission.PostId) == false)
            {
                return Forbid();
            }
            this.postLogic.ModifySubmission(submission, (sub) =>
            {
                sub.State = state;
            });
            return Ok();
        }
    }
}