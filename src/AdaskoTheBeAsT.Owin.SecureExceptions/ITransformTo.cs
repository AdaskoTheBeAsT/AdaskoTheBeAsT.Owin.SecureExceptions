using System;
using System.Net;

namespace AdaskoTheBeAsT.Owin.SecureExceptions;

public interface ITransformTo<out T>
{
    ITransformsMap To(
        HttpStatusCode statusCode,
        string reasonPhrase,
        Func<T, string> contentGenerator,
        string contentType = "text/plain");
}
