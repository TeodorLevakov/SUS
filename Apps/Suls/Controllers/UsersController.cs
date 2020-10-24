using Suls.Services;
using Suls.ViewModels.Users;
using SUS.HTTP;
using SUS.MvcFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Suls.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }
        public HttpResponse Login() 
        {
            if (this.IsUserSignedIn())
            {
                return this.Redirect("/");
            }

            return this.View();
        }

        [HttpPost]
        public HttpResponse Login(string username, string password) 
        {
            var userId = usersService.GetUserId(username, password);

            if (userId == null)
            {
                return this.Error("Invalid username or password.");
            }
            this.SignIn(userId);

            return this.Redirect("/");
        }

        public HttpResponse Register() 
        {
            if (this.IsUserSignedIn())
            {
                return this.Redirect("/");
            }

            return this.View();
        }

        [HttpPost]
        public HttpResponse Register(RegisterInputModel input) 
        {
            if (string.IsNullOrEmpty(input.Username) || input.Username.Length < 5 || input.Username.Length > 20)
            {
                return this.Error("Username is not valid.");
            }
            if (!this.usersService.IsUsernameAvailable(input.Username))
            {
                return this.Error("Username already taken.");
            }
            if (string.IsNullOrEmpty(input.Email) || !new EmailAddressAttribute().IsValid(input.Email))
            {
                return this.Error("Invalid email address.");
            }
            if (!this.usersService.IsEmailAvailable(input.Email))
            {
                return this.Error("Email already taken.");
            }
            if (string.IsNullOrEmpty(input.Password) || input.Password.Length < 6 || input.Password.Length > 20)
            {
                return this.Error("Pass should be between 6 and 20 char.");
            }
            if (input.Password != input.Confirmpassword)
            {
                return this.Error("Password dont match.");
            }

            this.usersService.CreateUser(input.Username, input.Email, input.Password);
            return this.Redirect("/Users/Login");
        }

        public HttpResponse Logout() 
        {
            if (!this.IsUserSignedIn())
            {
                return this.Redirect("/");
            }

            this.SignOut();
            return this.Redirect("/");
        }
    }
}
