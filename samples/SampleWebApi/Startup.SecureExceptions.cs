using System.Net;
using System.Web;
using System.Web.Http;
using AdaskoTheBeAsT.Owin.SecureExceptions;

namespace SampleWebApi;

public partial class Startup
{
    private ITransformsCollection GetSecureExceptions()
    {
        return TransformsCollectionBuilder.Begin()

            .Map<HttpRequestValidationException>()
            .To(HttpStatusCode.BadRequest, "Invalid input", ex => ex.Message)

            .Map<HttpResponseException>()
            .To(
                HttpStatusCode.BadRequest,
                "This is the reasonphrase",
                ex => "And this is the response content: " + ex.Message)

            ////.Map<SomeCustomException>()
            ////.To(HttpStatusCode.NoContent, "Bucket is emtpy", ex => string.Format("Inner details: {0}", ex.Message))

            ////.Map<EntityUnknownException>()
            ////.To(HttpStatusCode.NotFound, "Entity does not exist", ex => ex.Message)

            ////.Map<InvalidAuthenticationException>()
            ////.To(HttpStatusCode.Unauthorized, "Invalid authentication", ex => ex.Message)

            ////.Map<AuthorizationException>()
            ////.To(HttpStatusCode.Forbidden, "Forbidden", ex => ex.Message)

            ////// Map all other exceptions if needed.
            ////// Also it would be useful if you want to map exception to a known model.
            ////.MapAllOthers()
            ////.To(HttpStatusCode.InternalServerError, "Internal Server Error", ex => ex.Message)

            .Done();
    }
}
