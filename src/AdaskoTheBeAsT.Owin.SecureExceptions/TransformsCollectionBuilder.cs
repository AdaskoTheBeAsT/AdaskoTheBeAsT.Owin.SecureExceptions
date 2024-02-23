using System;
using System.Collections.Generic;
using System.Net;

namespace AdaskoTheBeAsT.Owin.SecureExceptions;

public sealed class TransformsCollectionBuilder
    : ITransformsMap,
        ITransformsCollection
{
    private readonly List<ITransform?> _transforms = [];

    private TransformsCollectionBuilder()
    {
    }

    public static ITransformsMap Begin() => new TransformsCollectionBuilder();

    public ITransform? FindTransform(Exception exception)
    {
        return _transforms.Find(x => x?.CanHandle(exception) ?? false);
    }

    public ITransformTo<T> Map<T>()
        where T : Exception =>
        new Transform<T>(this);

    public ITransformTo<Exception> Map(Func<Exception, bool> matcher) => new Transform<Exception>(this, matcher);

    public ITransformTo<Exception> MapAllOthers() => Map<Exception>();

    public ITransformsCollection Done() => this;

    private sealed class Transform<T> : ITransformTo<T>, ITransform
        where T : Exception
    {
        private readonly TransformsCollectionBuilder _transformsCollectionBuilder;
        private readonly Func<Exception, bool> _matcher;
        private Func<T, string>? _contentGenerator;

        public Transform(TransformsCollectionBuilder transformsCollectionBuilder)
            : this(transformsCollectionBuilder, (ex) => ex.GetType() == typeof(T))
        {
        }

        public Transform(TransformsCollectionBuilder transformsCollectionBuilder, Func<Exception, bool> matcher)
        {
            _transformsCollectionBuilder = transformsCollectionBuilder;
            _matcher = matcher;
        }

        public string? ContentType { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        public string? ReasonPhrase { get; private set; }

        public string GetContent(Exception? ex)
        {
            if (ex is not T exCasted)
            {
                return string.Empty;
            }

            if (_contentGenerator == null)
            {
                return string.Empty;
            }

            return _contentGenerator(exCasted);
        }

        public bool CanHandle<T2>(T2 ex)
            where T2 : Exception
        {
            var result = _matcher(ex);
            if (!result)
            {
                result = _matcher(new Exception());
            }

            return result;
        }

        public ITransformsMap To(
            HttpStatusCode statusCode,
            string reasonPhrase,
            Func<T, string> contentGenerator,
            string contentType = "text/plain")
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            ContentType = contentType;
            _contentGenerator = contentGenerator;
            _transformsCollectionBuilder._transforms.Add(this);
            return _transformsCollectionBuilder;
        }
    }
}
