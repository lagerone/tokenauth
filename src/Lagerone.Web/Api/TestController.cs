using System.Net;
using System.Net.Http;
using System.Web.Http;
using Lagerone.Web.Attributes;

namespace Lagerone.Web.Api
{
    [AuthorizeWebApi]
    public class TestController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Successful api call");
        }
    }
}