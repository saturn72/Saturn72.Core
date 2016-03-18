using System;
using Saturn72.App.Common;

namespace Saturn72.App
{
    internal class Program
    {
        private static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            var appId = args.Length >= 2 ? args[1] : AppDomain.CurrentDomain.FriendlyName;

            var app = new Saturn72App(appId);
            app.Start();
        }
    }
}