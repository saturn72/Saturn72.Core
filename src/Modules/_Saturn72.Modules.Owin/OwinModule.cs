using System;
using Microsoft.Owin.Hosting;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Modules;

namespace Saturn72.Modules.Owin
{
    public class OwinModule : Resolver, IModule
    {
        private static string _baseUri;

        public void Load()
        {
            //TODO: set base address from config
            _baseUri = "http://localhost:9000/";
        }

        public void Start()
        {
            Console.WriteLine("Starting web Server...");
            using (WebApp.Start<Startup>(_baseUri))
            {
                Console.WriteLine("Server running at {0} - press Enter to quit. ", _baseUri);
                while (true)
                {
                    
                }
            }
        }

        public void Stop()
        {
        }

        public int StartupOrder => 100;

        public int StopOrder => 100;
    }
}