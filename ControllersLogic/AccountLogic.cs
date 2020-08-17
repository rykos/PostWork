using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostWork.ControllersLogic
{
    public class AccountLogic : IAccountLogic
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public AccountLogic(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task Login(IFormCollection data)
        {
            if (this.FieldIsIncorrect(data["email"]))
            {
                throw new InvalidLoginDataException();
            }
            if (this.FieldIsIncorrect(data["password"]))
            {
                throw new InvalidLoginDataException();
            }
            IdentityUser user = await this.userManager.FindByEmailAsync(data["email"]);
            if (user == null)
            {
                throw new UserDoesNotExistException();
            }
            //this.signInManager.CheckPasswordSignInAsync(user, data["password"], false);
            SignInResult loginResoult = await this.signInManager.PasswordSignInAsync(user, data["password"], false, false);
            if (!loginResoult.Succeeded)
            {
                throw new InvalidPasswordException();
            }
            else
            {
                Console.WriteLine("User logged in");
            }
        }

        public async Task Logout()
        {
            await this.signInManager.SignOutAsync();
        }

        public async Task Register(IFormCollection data)
        {
            if (this.FieldIsIncorrect(data["email"]))
            {
                throw new InvalidLoginDataException();
            }
            if (this.FieldIsIncorrect(data["username"]))
            {
                throw new InvalidLoginDataException();
            }
            if (this.FieldIsIncorrect(data["password"]))
            {
                throw new InvalidLoginDataException();
            }
            Task<IdentityUser> findAccountWithEmailTask = this.userManager.FindByEmailAsync(data["email"]);
            Task<IdentityUser> findAccountWithUsernameTask = this.userManager.FindByNameAsync(data["username"]);
            if (await findAccountWithEmailTask != null)//Email is already taken
            {
                throw new EmailIsTakenException();
            }
            if (await findAccountWithUsernameTask != null)//Username is already taken
            {
                throw new UsernameIsTakenException();
            }
            IdentityUser newUser = new IdentityUser(data["username"])
            {
                Email = data["email"]
            };
            await this.userManager.CreateAsync(newUser, data["password"]);
        }

        //Check if field is valid
        private bool FieldIsIncorrect(string value)
        {
            if (value == null)
            {
                return true;
            }
            if (value == "")
            {
                return true;
            }
            return false;
        }

        //If no accounts exist create admin account, ensure that admin role exists
        public async Task ValidateDatabase()
        {
            if (!await this.roleManager.RoleExistsAsync("admin"))//Create admin role if not exists
            {
                IdentityRole role = new IdentityRole("admin");
                await this.roleManager.CreateAsync(role);
            }
            Console.WriteLine("Users: " + this.userManager.Users.Count());
            if (this.userManager.Users.Count() == 0)//If there are no users create admin account
            {
                Console.WriteLine("Im in loop");
                IdentityUser adminIdentity = new IdentityUser("admin")
                {
                    Email = "admin@admin.admin"
                };
                await this.userManager.CreateAsync(adminIdentity, "Admin11!");
                await this.userManager.AddToRoleAsync(adminIdentity, "admin");
            }
        }
    }

    public interface IAccountLogic
    {
        Task Login(IFormCollection data);
        Task Logout();
        Task Register(IFormCollection data);
        Task ValidateDatabase();
    }
}