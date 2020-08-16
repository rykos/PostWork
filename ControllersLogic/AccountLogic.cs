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
        public AccountLogic(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task Login(IFormCollection data)
        {
            try
            {
                if (data["email"].Count == 0)
                {
                    throw new InvalidLoginDataException();
                }
                if (data["password"].Count == 0)
                {
                    throw new InvalidLoginDataException();
                }
                Console.WriteLine($"Email: {data["email"]} Password: {data["password"]}");
            }
            catch
            {
                throw;
            }
        }

        public async Task Logout()
        {
            await this.signInManager.SignOutAsync();
        }

        public async Task Register(IFormCollection data)
        {
            Task<bool> emailValidationTask = this.FieldIsIncorrect(data["email"]);
            Task<bool> usernameValidationTask = this.FieldIsIncorrect(data["username"]);
            Task<bool> passwordValidationTask = this.FieldIsIncorrect(data["password"]);
            if (await emailValidationTask)
            {
                throw new InvalidLoginDataException();
            }
            if (await usernameValidationTask)
            {
                throw new InvalidLoginDataException();
            }
            if (await passwordValidationTask)
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
        private async Task<bool> FieldIsIncorrect(string value)
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
    }

    public interface IAccountLogic
    {
        Task Login(IFormCollection data);
        Task Logout();
        Task Register(IFormCollection data);
    }
}