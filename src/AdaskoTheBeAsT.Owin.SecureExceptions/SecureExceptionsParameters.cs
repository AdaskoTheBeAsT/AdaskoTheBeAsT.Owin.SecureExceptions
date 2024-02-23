using System.Collections.Generic;

namespace AdaskoTheBeAsT.Owin.SecureExceptions;

public class SecureExceptionsParameters
{
    public SecureExceptionsParameters()
    {
        SwallowedExceptionsProviders = new List<IExceptionProvider>();
    }

    public IEnumerable<IExceptionProvider> SwallowedExceptionsProviders { get; set; }
}
