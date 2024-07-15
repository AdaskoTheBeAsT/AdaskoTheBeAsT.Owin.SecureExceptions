using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace AdaskoTheBeAsT.Owin.SecureExceptions;

public class SecureExceptionsMiddleware

    // : OwinMiddleware
{
    private readonly Func<IDictionary<string, object>, Task> _next;
    private readonly SecureExceptionsParameters _parameters;
    private readonly ITransformsCollection _transformsCollection;

    public SecureExceptionsMiddleware(
        Func<IDictionary<string, object>, Task> next,
        ITransformsCollection transformsCollection)
        : this(next, transformsCollection, DefaultProperties())
    {
    }

    public SecureExceptionsMiddleware(
        Func<IDictionary<string, object>, Task> next,
        ITransformsCollection transformsCollection,
        SecureExceptionsParameters parameters)
    {
        _next = next;
        _transformsCollection = transformsCollection;
        _parameters = parameters;
    }

#pragma warning disable VSTHRD200,CC0061 // Use "Async" suffix for async methods
    public async Task Invoke(IDictionary<string, object> environment)
#pragma warning restore VSTHRD200,CC0061 // Use "Async" suffix for async methods
    {
        var context = new OwinContext(environment);

        Exception? exception;
        ITransform? transformer = null;

        try
        {
            await _next.Invoke(environment).ConfigureAwait(true);
            exception = GetSwallowedException(context);

            if (exception != null)
            {
                transformer = _transformsCollection.FindTransform(exception);
            }
        }
        catch (Exception caughtException)
        {
            exception = caughtException;

            // check if we can transform it, otherwise we should throw it
            transformer = _transformsCollection.FindTransform(exception);
            if (transformer == null)
            {
                throw;
            }
        }

        if (transformer != null)
        {
            TransformException(context, transformer, exception);
        }
    }

    private static SecureExceptionsParameters DefaultProperties()
    {
        return new SecureExceptionsParameters();
    }

    private static void TransformException(
        OwinContext context,
        ITransform transform,
        Exception? ex)
    {
        var content = transform.GetContent(ex);

        context.Response.ContentType = transform.ContentType;
        context.Response.StatusCode = (int)transform.StatusCode;
        context.Response.ReasonPhrase = transform.ReasonPhrase;
        context.Response.ContentLength = Encoding.UTF8.GetByteCount(content);
        context.Response.Write(content);
    }

    private Exception? GetSwallowedException(IOwinContext context)
    {
        return _parameters.SwallowedExceptionsProviders.Select(provider => provider.GetException(context.Environment))
            .FirstOrDefault(e => e != null);
    }
}
