using System;
using Saturn72.App.Common;

namespace CalculatorApp
{
    internal class Program
    {
        private static void Main()
        {
            //var args = Environment.GetCommandLineArgs();
            //var app = new Saturn72App(args[1]);

            var app = new Saturn72App("calculator");
            app.Start();
        }
    }
}