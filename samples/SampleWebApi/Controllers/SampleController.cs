using System.Web.Http;

namespace SampleWebApi.Controllers;

[AllowAnonymous]
public class SampleController : ApiController
{
    [HttpGet]
    public IHttpActionResult Get()
    {
        return Ok("Hello world!!!");
    }
}
