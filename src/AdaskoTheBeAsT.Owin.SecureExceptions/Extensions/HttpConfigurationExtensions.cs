using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.Extensions;

public static class HttpConfigurationExtensions
{
    public static void UseWebApi2ExceptionHandler(
        this HttpConfiguration config,
        ITransformsCollection transforms)
    {
        config.Services.Replace(typeof(IExceptionHandler), new WebApi2ExceptionHandler(transforms));
    }
}
