using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PostWork.ControllersLogic;

namespace PostWork.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountLogic accountLogic;
        public AccountController(IAccountLogic accountLogic)
        {
            this.accountLogic = accountLogic;
        }

        //Default
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        //Login Page
        public IActionResult Login()
        {
            return View();
        }

        //Register Page
        public IActionResult Register()
        {
            return View();
        }

        //Login request
        //required email and password 
        [HttpPost]
        public async Task<IActionResult> Login(IFormCollection data)
        {
            Console.WriteLine("Login called");
            try
            {
                await this.accountLogic.Login(data);
            }
            catch (InvalidLoginDataException)
            {
                Console.WriteLine("Login data was invalid");
            }
            return View();
        }

        //Register request
        //Required email, username and password
        [HttpPost]
        public async Task<IActionResult> Register(IFormCollection data)
        {
            try
            {
                await this.accountLogic.Register(data);
            }
            catch (InvalidLoginDataException)
            {
                Console.WriteLine("invalid login data");
            }
            catch (EmailIsTakenException)
            {
                Console.WriteLine("email is taken");
            }
            catch (UsernameIsTakenException)
            {
                Console.WriteLine("username is taken");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok();
        }

        //Logout request
        [HttpGet]
        public IActionResult Logout()
        {
            if (!User.Identity.IsAuthenticated)//User is not logged in
            {
                return BadRequest();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}