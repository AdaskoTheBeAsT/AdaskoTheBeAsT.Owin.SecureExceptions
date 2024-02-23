# AdaskoTheBeAsT.Owin.SecureExceptions

Secure exceptions for Owin based on OwinFriendlyExceptions and OwinFriendlyExceptions.Plugins.
A middleware that can translate exceptions into nice http resonses. This allows you to throw meaningfull exceptions from your framework, business code or other middlewares and translate the exceptions to nice and friendly http responses.

## Installation

`Install-package AdaskoTheBeAsT.Owin.SecureExceptions`

See [Troubleshooting](#troubleshooting) for help with any installation errors

## Example

```cs
    using System;
    using System.Net;
    using Api.Exceptions;
    using Api.Logic.Exceptions;
    using Owin;
    using AdaskoTheBeAsT.Owin.SecureExceptions;
    using AdaskoTheBeAsT.Owin.SecureExceptions.Extensions;
    using AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;
    
    namespace Api
    {
        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                // Use AdaskoTheBeAsT.Owin.SecureExceptions before your other middlewares
                app.UseSecureExceptions(GetSecureExceptions(), new [] {new WebApi2ExceptionProvider()});
    
                // Then the rest of your application
                //app.UseCors(options);
                //app.UseWelcomePage();
                //app.UseWebApi(config)
            }
    
            private ITransformsCollection GetSecureExceptions()
            {
                return TransformsCollectionBuilder.Begin()
    
                    .Map<ExampleException>()
                    .To(HttpStatusCode.BadRequest, "This is the reasonphrase",
                        ex => "And this is the response content: " + ex.Message)
    
                    .Map<SomeCustomException>()
                    .To(HttpStatusCode.NoContent, "Bucket is emtpy", ex => string.Format("Inner details: {0}", ex.Message))
    
                    .Map<EntityUnknownException>()
                    .To(HttpStatusCode.NotFound, "Entity does not exist", ex => ex.Message)
    
                    .Map<InvalidAuthenticationException>()
                    .To(HttpStatusCode.Unauthorized, "Invalid authentication", ex => ex.Message)
    
                    .Map<AuthorizationException>()
                    .To(HttpStatusCode.Forbidden, "Forbidden", ex => ex.Message)
                    
                    // Map all other exceptions if needed. 
                    // Also it would be useful if you want to map exception to a known model.
                    .MapAllOthers()
                    .To(HttpStatusCode.InternalServerError, "Internal Server Error", ex => ex.Message)
    
                .Done();
            }
        }
    }
```

### Specify content type

By default, AdaskoTheBeAsT.Owin.SecureExceptions sets the response Content-Type to `text/plain`. To use a different type:

```cs    
    .Map<SomeJsonException>()
    .To(HttpStatusCode.BadRequest, "This exception is json",
        ex => JsonConvert.SerializeObject(ex.Message), "application/json")
```

### WebApi2

Installation:  

1. `Install-package AdaskoTheBeAsT.Owin.SecureExceptions`
2. `app.UseFriendlyExceptions(exceptionsToHandle, new [] {new WebApi2ExceptionProvider()});`
3. `config.Services.Replace(typeof(IExceptionHandler), new WebApi2ExceptionHandler(exceptionsToHandle));`
Install the package, and supply the WebApi Exception Provider to the AdaskoTheBeAsT.Owin.SecureExceptions extension method. In order for the Plugin to get swallowed exceptions you have to replace the ExcepionHandler service in Web Api. The plugin takes a list of which exceptions we can handle, so WebApi can still take care of unhandled exceptions for you.

### Troubleshooting

When installing the Web Api Plugin, sometimes your System.Web.Http reference will mismatch. Use this Package Manager Console command to fix your Assembly Bining redirect:   `Get-Project YourProjectReferencingOwinFriendlyExceptions | Add-BindingRedirect`
