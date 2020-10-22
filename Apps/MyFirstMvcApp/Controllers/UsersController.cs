using BattleCards.Services;
using SUS.HTTP;
using SUS.MvcFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BattleCards.Controllers
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

        [HttpPost("/Users/Login")]
        public HttpResponse DoLogin(string username, string password) 
        {
            if (this.IsUserSignedIn())
            {
                return this.Redirect("/");
            }

            var userId = this.usersService.GetUserId(username, password);

            if (userId == null)
            {
                return this.Error("Invalid username or password");
            }

            this.SignIn(userId);

            return this.Redirect("/Cards/All");
        }
        public HttpResponse Register()
        {
            if (this.IsUserSignedIn())
            {
                return this.Redirect("/");
            }
            return this.View();
        }

        [HttpPost("/Users/Register")]
        public HttpResponse DoRegister(string username, string email, string password, string confirmPassword)
        {
            if (this.IsUserSignedIn())
            {
                return this.Redirect("/");
            }

            //var username = this.Request.FormData["username"];
            //var email = this.Request.FormData["email"];
            //var password = this.Request.FormData["password"];
            //var confirmPass = this.Request.FormData["confirmPassword"];

            if (username == null || username.Length < 5 || username.Length > 20)
            {
                return this.Error("Invalid username.");
            }
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))
            {
                return this.Error("Invalid username. Only alphanum-chars are allowed.");
            }
            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            {
                return this.Error("Invalid email.");
            }

            if (password == null || password.Length < 6 || password.Length > 20)
            {
                return this.Error("Invalid password.");
            }

            if (password != confirmPassword)
            {
                return this.Error("Password should be the same.");
            }
            if (!this.usersService.IsUsernameAvailable(username))
            {
                return this.Error("Username already taken.");
            }
            if (!this.usersService.IsEmailAvailable(email))
            {
                return this.Error("Email already taken");
            }

            this.usersService.CreateUser(username, email, password);

            return this.Redirect("/Users/Login");
        }

        public HttpResponse Logout() 
        {
            if (!this.IsUserSignedIn())
            {
                return this.Error("Only jogged-in users can log-out");
            }
          
                this.SignOut();
                return this.Redirect("/");
        }
    }
}
