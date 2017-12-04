using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Interfaces;
using WarCardGame.Services;
using WarCardGame.Views;

namespace WarCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());            
            var controller = kernel.Get<IWarGameController>();
            var view = new ConsoleGameView(controller);
            view.PlayGame();
        }
    }
}
