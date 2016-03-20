using System;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Saturn72.Core.Modules;
using Saturn72.Extensions;

namespace Saturn72.Modules.Owin
{
    public class OwinModule : IModule
    {
        private static string _baseUri;

        public void Load()
        {
            //TODO: load from configuration
            _baseUri = "http://localhost:9000/";
        }

        public void Start()
        {
            Guard.HasValue(_baseUri);

            Console.WriteLine("Starting web Server...");
            Task.Factory.StartNew(StartWebServer);
        }

        private static void StartWebServer()
        {
            using (WebApp.Start<Startup>(_baseUri))
            {
                Console.WriteLine("web server started. uri: " + _baseUri);

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