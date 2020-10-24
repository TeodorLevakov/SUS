using Suls.Services;
using Suls.ViewModels.Problems;
using SUS.HTTP;
using SUS.MvcFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suls.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProblemsService problemService;

        public HomeController(IProblemsService problemService)
        {
            this.problemService = problemService;
        }

        [HttpGet("/")]
        public HttpResponse Index() 
        {
            if (this.IsUserSignedIn())
            {
                var viewModel = problemService.GetAll();

                return this.View(viewModel, "IndexLoggedIn");
            }
            else
            {
                return this.View();
            }
        }

    }
}
