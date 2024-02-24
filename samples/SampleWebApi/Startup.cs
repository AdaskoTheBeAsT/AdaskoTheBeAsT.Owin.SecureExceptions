using System;
using System.Web.Http;
using AdaskoTheBeAsT.Owin.SecureExceptions.Extensions;
using AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Owin;
using SampleWebApi;

[assembly: OwinStartup(typeof(Startup))]

namespace SampleWebApi;

public partial class Startup
{
    public void Configuration(IAppBuilder app)
    {
        if (app is null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        app.UseNoHttpResourceFoundSanitizer();
        var secureExceptions = GetSecureExceptions();
        app.UseSecureExceptions(
            secureExceptions,
            new[] { new WebApi2ExceptionProvider() });

#pragma warning disable CA2000 // Dispose objects before losing scope
#pragma warning disable IDISP001 // Dispose created.
        var httpConfiguration = new HttpConfiguration();
#pragma warning restore IDISP001 // Dispose created.
#pragma warning restore CA2000 // Dispose objects before losing scope
        httpConfiguration.UseWebApi2ExceptionHandler(secureExceptions);

        ConfigureRoute(httpConfiguration);
        app.UseWebApi(httpConfiguration);
        httpConfiguration.EnsureInitialized();

        app.UseStageMarker(PipelineStage.MapHandler);
    }
}
