using BattleCards.Controllers;
using SUS.HTTP;
using SUS.MvcFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BattleCards
{
    class Program
    {
        public static async Task Main(string[] args)
        {
           
            await Host.CreateHostAsync(new Startup(), 80);
        }

    }
}
