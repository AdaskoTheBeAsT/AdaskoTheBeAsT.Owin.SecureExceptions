using System;
using System.Collections.Generic;

namespace AdaskoTheBeAsT.Owin.SecureExceptions;

public interface IExceptionProvider
{
    Exception? GetException(IDictionary<string, object> environment);
}
