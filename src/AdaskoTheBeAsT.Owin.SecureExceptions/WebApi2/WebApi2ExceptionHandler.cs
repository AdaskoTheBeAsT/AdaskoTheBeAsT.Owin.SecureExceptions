using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;

public class WebApi2ExceptionHandler(ITransformsCollection transforms) : IExceptionHandler
{
    public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Handle(context, transforms);
        return Task.FromResult(0);
    }

    private static void Handle(ExceptionHandlerContext context, ITransformsCollection transforms)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var exceptionContext = context.ExceptionContext;

        var request = exceptionContext.Request;

        if (request == null)
        {
            throw new ArgumentException(nameof(request));
        }

        if (exceptionContext.CatchBlock == ExceptionCatchBlocks.IExceptionFilter)
        {
            // The exception filter stage propagates unhandled exceptions by default (when no filter handles the
            // exception).
            return;
        }

        var owinContext = context.Request.GetOwinContext();
        owinContext.Set("webapi.exception", context.Exception);

        if (transforms.FindTransform(context.Exception) != null)
        {
            context.Result = new EmptyResponse();
        }
    }

    private sealed class EmptyResponse
        : IHttpActionResult
    {
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var res = new HttpResponseMessage();
            return Task.FromResult(res);
        }
    }
}
