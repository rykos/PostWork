using Microsoft.AspNetCore.Mvc;
using PostWork.Models;
using PostWork.Data;
using PostWork.ControllersLogic;

namespace PostWork.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly IPostLogic postLogic;
        public SubmissionController(IPostLogic postLogic)
        {
            this.postLogic = postLogic;
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
    }
}