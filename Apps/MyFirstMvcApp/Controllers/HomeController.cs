using BattleCards.ViewModels;
using SUS.HTTP;
using SUS.MvcFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleCards.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public HttpResponse Index()
        {
            if (this.IsUserSignedId())
            {
                return this.Redirect("/Card/All");
            }
            else
            {
                return this.View();
            }
            
        }

        public HttpResponse About()
        {
            this.SignIn("teo");
            return this.View();
        }
    }
}
