using System.Web.Http;
using Saturn72.Core.Web.Controllers;

namespace Saturn72.Modules.Owin.Controllers
{
    public class HomeController : Saturn72ApiControllerBase
    {

        //[Route("api/")]
        public string Get()
        {
            return "Welcome!!!";
        }
    }
}
