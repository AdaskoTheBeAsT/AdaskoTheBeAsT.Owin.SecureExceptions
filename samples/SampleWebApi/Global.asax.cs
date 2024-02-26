using System;
using System.Web;

namespace SampleWebApi;

#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable MA0048 // File name must match type name
public class Global : HttpApplication
#pragma warning restore MA0048 // File name must match type name
#pragma warning restore SA1649 // File name should match first type name
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
    protected void Application_Start()
    {
        // noop
    }

    protected void Application_Error(object sender, EventArgs e)
    {
        var exception = Server.GetLastError();
        if (!(exception is HttpException) ||
            !exception.Message.Contains("Request.Path"))
        {
            return;
        }

        // Clear the error to prevent ASP.NET from displaying the default error page.
        Server.ClearError();

        Response.TrySkipIisCustomErrors = true;
        Response.Clear();

        // Write a simple text response indicating an invalid path.
        Response.ContentType = "text/plain";
        Response.StatusCode = 400; // Bad Request
        Response.Write("Invalid path");
        Response.End();
    }
#pragma warning restore CA1707 // Identifiers should not contain underscores
}
