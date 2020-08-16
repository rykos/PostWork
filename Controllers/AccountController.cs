using System;
using System.Security.Claims;
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
        private readonly UserManager<IdentityUser> userManager;
        public AccountController(IAccountLogic accountLogic, UserManager<IdentityUser> userManager)
        {
            this.accountLogic = accountLogic;
            this.userManager = userManager;
        }

        //Default
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            return View(await this.userManager.GetUserAsync(User));
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

        public IActionResult RecoverPassword()
        {
            return View("/Views/Account/PasswordRecovery.cshtml");
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
            catch (InvalidPasswordException)
            {
                Console.WriteLine("Bad password");
            }
            catch (UserDoesNotExistException)
            {
                Console.WriteLine("User does not exist");
            }
            return RedirectToAction("Index");
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
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await this.accountLogic.Logout();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}