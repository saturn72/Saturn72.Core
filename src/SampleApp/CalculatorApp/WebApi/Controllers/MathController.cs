using System.Web.Http;
using Saturn72.Core.Web.Controllers;

namespace CalculatorApp.WebApi.Controllers
{
    [RoutePrefix("calculator")]
    public class MathController : Saturn72ApiControllerBase
    {
        public string Get()
        {
            return "Please enter mathematical expression - in the folowing format: <baseurl>/calculator/add/x,y";
        }

        [HttpGet]
        [Route("addone/{x:int}")]
        public int Add(int x)
        {
            return ++x;
        }

        [Route("add/{x:int},{y:int}")]
        public int Get(int x, int y)
        {
            return x+y;
        }
    }
}