using System;
using System.Collections.Generic;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;

public class WebApi2ExceptionProvider
    : IExceptionProvider
{
    public Exception? GetException(IDictionary<string, object> environment)
    {
        environment.TryGetValue("webapi.exception", out var exception);

        return exception as Exception;
    }
}
