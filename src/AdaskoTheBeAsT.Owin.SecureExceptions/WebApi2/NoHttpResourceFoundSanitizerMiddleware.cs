using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;

public class NoHttpResourceFoundSanitizerMiddleware(OwinMiddleware next)
    : OwinMiddleware(next)
{
    private const string NoHttpResourceFoundMessage = "No HTTP resource was found that matches the request URI";

    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
    };

    public override async Task Invoke(IOwinContext context)
    {
        // Buffer the response stream in order to read and then replace it if necessary
        var originalResponseBody = context.Response.Body;
        using (var newResponseBody = new MemoryStream())
        {
            context.Response.Body = newResponseBody;

            await Next.Invoke(context);

            // Reset the memory stream position back to the beginning
            newResponseBody.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(newResponseBody);

            // Read the response stream
            var responseBody = await reader.ReadToEndAsync();

            // Check if the response contains the specific message
            if (!string.IsNullOrEmpty(responseBody) &&
                responseBody.StartsWith("{\"Message\":\"No HTTP resource was found that matches the request URI"))
            {
                // Modify the response here as needed
                var noHttpResourceFoundResponse = JsonConvert.DeserializeObject<NoHttpResourceFoundResponse>(responseBody);
                if (noHttpResourceFoundResponse is not null && !string.IsNullOrEmpty(noHttpResourceFoundResponse.Message))
                {
                    noHttpResourceFoundResponse.Message =
                        noHttpResourceFoundResponse.Message!.Substring(0, NoHttpResourceFoundMessage.Length);
                }

                var sanitizedResponse = JsonConvert.SerializeObject(noHttpResourceFoundResponse, JsonSerializerSettings);
                var sanitizedBytes = Encoding.UTF8.GetBytes(sanitizedResponse);

                // Write the modified response
                await originalResponseBody.WriteAsync(sanitizedBytes, 0, sanitizedBytes.Length);
            }
            else
            {
                // If no modification is needed, write the original response back
                newResponseBody.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalResponseBody);
            }
        }

        // Restore the original response body stream
        context.Response.Body = originalResponseBody;
    }
}
