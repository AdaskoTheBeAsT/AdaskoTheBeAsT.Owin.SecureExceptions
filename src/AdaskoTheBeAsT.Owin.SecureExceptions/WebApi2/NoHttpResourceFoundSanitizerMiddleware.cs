using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;

public class NoHttpResourceFoundSanitizerMiddleware(OwinMiddleware next)
    : OwinMiddleware(next)
{
    private static readonly Regex FindMessageWithUrlRegex = new Regex(
        @"^([{]\s*""[Mm]essage"":\s*""No\sHTTP\sresource\swas\sfound\sthat\smatches\sthe\srequest\sURI)(.*)",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex ReplaceUrlRegex = new Regex(
        @"^(No\sHTTP\sresource\swas\sfound\sthat\smatches\sthe\srequest\sURI)(.*)",
        RegexOptions.Compiled | RegexOptions.Multiline);

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
                FindMessageWithUrlRegex.IsMatch(responseBody))
            {
                // Modify the response here as needed
                var noHttpResourceFoundResponse = JsonConvert.DeserializeObject<NoHttpResourceFoundResponse>(responseBody);
                if (noHttpResourceFoundResponse is not null && !string.IsNullOrEmpty(noHttpResourceFoundResponse.Message))
                {
                    noHttpResourceFoundResponse.Message = ReplaceUrlRegex.Replace(
                        noHttpResourceFoundResponse.Message!,
                        "$1.");
                }

                var sanitizedResponse = JsonConvert.SerializeObject(noHttpResourceFoundResponse);
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
