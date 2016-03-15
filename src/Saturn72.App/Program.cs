using System;
using Saturn72.App.Common;

namespace Saturn72.App
{
    internal class Program
    {
        private static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            var app = new Saturn72App(args[1]);
            app.Start();
        }
    }
}