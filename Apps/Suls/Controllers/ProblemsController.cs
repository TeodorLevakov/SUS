using Suls.Services;
using SUS.HTTP;
using SUS.MvcFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suls.Controllers
{
    public class ProblemsController : Controller
    {
        private readonly IProblemsService problemService;

        public ProblemsController(IProblemsService problemService)
        {
            this.problemService = problemService;
        }
        public HttpResponse Create() 
        {
            if (!this.IsUserSignedIn())
            {
                return this.Redirect("/Users/Login");
            }

            return this.View();
        
        }

        [HttpPost]
        public HttpResponse Create(string name, int points)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 5 || name.Length > 20)
            {
                return this.Error("Problemname should be between 5 and 20 char.");
            }
            if (points < 50 || points > 300)
            {
                return this.Error("Invalid points value.");
            }

            this.problemService.Create(name, points);
            return this.Redirect("/");
        }

        public HttpResponse Details(string id) 
        {
            if (!this.IsUserSignedIn())
            {
                return this.Redirect("/Users/Login");
            }

            var viewModel = this.problemService.GetById(id);

            return this.View(viewModel);
        }
    }
}
