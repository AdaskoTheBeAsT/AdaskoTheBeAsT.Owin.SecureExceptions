using System.Collections.Generic;
using AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;
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

    public static void UseNoHttpResourceFoundSanitizer(this IAppBuilder app) =>
        app.Use<NoHttpResourceFoundSanitizerMiddleware>();
}
