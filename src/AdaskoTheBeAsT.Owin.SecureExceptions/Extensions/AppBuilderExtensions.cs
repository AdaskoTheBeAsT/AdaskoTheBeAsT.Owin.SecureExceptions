using System.Collections.Generic;
using Owin;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.Extensions;

public static class AppBuilderExtensions
{
    public static void UseSecureExceptions(
        this IAppBuilder app,
        ITransformsCollection transforms,
        IEnumerable<IExceptionProvider>? swallowedExceptionsProviders = null)
    {
        var options = new SecureExceptionsParameters
        {
            SwallowedExceptionsProviders = swallowedExceptionsProviders ?? [],
        };

        app.Use<SecureExceptionsMiddleware>(transforms, options);
    }
}
