using System;
using System.Net.Http;
using System.Threading.Tasks;
using AdaskoTheBeAsT.Owin.SecureExceptions.IntegrationTest.Util;
using FluentAssertions;
using Microsoft.Owin.Testing;
using Reqnroll;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.IntegrationTest.Steps
{
    [Binding]
    public sealed class CallingApiStepDefinitions(ScenarioContext scenarioContext)
        : IDisposable
    {
        private HttpResponseMessage? _response;

        [Given("I have proper client")]
        public void GivenIHaveProperClient()
        {
            var server = scenarioContext.Get<TestServer>(Constants.Server);
            scenarioContext[Constants.Client] = server.HttpClient;
        }

        [When("I call api with proper parameters")]
        public async Task WhenICallApiWithProperParametersAsync()
        {
            var client = scenarioContext.Get<HttpClient>(Constants.Client);
            _response?.Dispose();
            _response = await client.GetAsync("api/sample").ConfigureAwait(false);
        }

        [Then("I should get success result")]
        public async Task ThenIShouldGetSuccessResultAsync()
        {
            if (_response is null)
            {
                throw new InvalidOperationException("Response is null");
            }

            var content = await _response.Content.ReadAsStringAsync().ConfigureAwait(false);
            content.Should().Be("\"Hello world!!!\"");
            _response.EnsureSuccessStatusCode();
        }

        public void Dispose() => _response?.Dispose();
    }
}
