using System.Web.Http;

namespace SampleWebApi.Controllers;

public class SanitizeController : ApiController
{
    /// <summary>
    /// [HttpGet].
    /// </summary>
    /// <returns></returns>
    public IHttpActionResult Get()
    {
        return Ok("Error");
    }
}
