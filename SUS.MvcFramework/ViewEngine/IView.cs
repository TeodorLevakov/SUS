using System;
using System.Collections.Generic;
using System.Text;

namespace SUS.MvcFramework.ViewEngine
{
    public interface IView
    {
        string ExecuteTamplate(object viewModel, string user);
    }
}
