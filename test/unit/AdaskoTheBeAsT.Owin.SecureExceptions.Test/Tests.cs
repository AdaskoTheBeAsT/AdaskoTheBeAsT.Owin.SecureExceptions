using System;
using System.Net;
using FluentAssertions;
using Xunit;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.Test;

public class Tests
{
    [Fact]
    public void LambdaMatcherMatches()
    {
        var builder = TransformsCollectionBuilder.Begin()
            .Map(ex => ex.Message?.Equals("ok", StringComparison.OrdinalIgnoreCase) ?? false)
            .To(HttpStatusCode.OK, "Success", (ex) => ex.Message);

        var coll = builder.Done();

        var testException1 = new Exception("fail");
        var testException2 = new Exception("ok");

        coll.FindTransform(testException1).Should().BeNull();
        coll.FindTransform(testException2).Should().NotBeNull();
    }

    [Fact]
    public void LambdaMatcherMatchesDerived()
    {
        var builder = TransformsCollectionBuilder.Begin()
            .Map(ex => ex.GetType().IsSubclassOf(typeof(Exception)))
            .To(HttpStatusCode.OK, "Success", (ex) => ex.Message);

        var coll = builder.Done();

        var testException = new ArgumentException("ok");

        coll.FindTransform(testException).Should().NotBeNull();
    }

    [Fact]
    public void ContentTypeDefaultsToTextPlain()
    {
        var builder = TransformsCollectionBuilder.Begin()
            .Map(ex => ex.GetType().IsSubclassOf(typeof(Exception)))
            .To(HttpStatusCode.OK, "Success", (ex) => ex.Message);

        var coll = builder.Done();

        var testException = new ArgumentException("ok");

        var transform = coll.FindTransform(testException);
        transform.Should().NotBeNull();
        transform!.ContentType.Should().Be("text/plain");
    }

    [Fact]
    public void ContentTypeCanBeOverridden()
    {
        const string json = "application/json";

        var builder = TransformsCollectionBuilder.Begin()
            .Map(ex => ex.GetType().IsSubclassOf(typeof(Exception)))
            .To(HttpStatusCode.OK, "Success", (ex) => ex.Message, json);

        var coll = builder.Done();

        var testException = new ArgumentException("ok");

        var transform = coll.FindTransform(testException);
        transform.Should().NotBeNull();
        transform!.ContentType.Should().Be(json);
    }
}
