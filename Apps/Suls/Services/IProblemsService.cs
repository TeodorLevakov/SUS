using Suls.ViewModels.Problems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suls.Services
{
    public interface IProblemsService
    {
        void Create(string name, int points);

        IEnumerable<HomePageProblemViewModel> GetAll();

        string GetNameById(string id);

        ProblemViewModel GetById(string id);
    }
}
