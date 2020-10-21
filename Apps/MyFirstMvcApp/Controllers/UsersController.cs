using SUS.HTTP;
using SUS.MvcFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BattleCards.Controllers
{
    public class UsersController : Controller
    {
        public HttpResponse Login()
        {
            return this.View();
        }

        [HttpPost("/Users/Login")]
        public HttpResponse DoLogin() 
        {
            return this.Redirect("/");
        }
        public HttpResponse Register()
        {
            return this.View();
        }

        [HttpPost("/Users/Register")]
        public HttpResponse DoRegister()
        {
            return this.Redirect("/");
        }

        public HttpResponse Logout() 
        {
            if (!this.IsUserSignedId())
            {
                return this.Error("Only jogged-in users can log-out");
            }
          
                this.SignOut();
                return this.Redirect("/");
        }
    }
}
