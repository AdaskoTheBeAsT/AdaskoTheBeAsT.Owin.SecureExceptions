using System.Threading.Tasks;
using Microsoft.Owin;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;

public class NoHttpResourceFoundSanitizerMiddleware(OwinMiddleware next)
    : OwinMiddleware(next)
{
    public override Task Invoke(IOwinContext context)
    {
        context.Response.Body = new OutputStreamAdapter(context.Response.Body);
        return Next.Invoke(context);
    }
}
