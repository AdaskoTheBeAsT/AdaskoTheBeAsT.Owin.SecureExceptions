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

        [When("I call api {string}")]
        public async Task WhenICallApiAsync(string url)
        {
            var client = scenarioContext.Get<HttpClient>(Constants.Client);
            _response?.Dispose();
            _response = await client.GetAsync(url).ConfigureAwait(false);
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

        [When("I call non existent api {string}")]
        public async Task WhenICallNonExistentApiAsync(string nonExistentApi)
        {
            var client = scenarioContext.Get<HttpClient>(Constants.Client);
            _response?.Dispose();
            _response = await client.GetAsync(nonExistentApi).ConfigureAwait(false);
        }

        [Then("I should get error with message '(.*)'")]
        public async Task ThenIShouldGetErrorWithMessageAsync(string errorMessage)
        {
            if (_response is null)
            {
                throw new InvalidOperationException("Response is null");
            }

            var content = await _response.Content.ReadAsStringAsync().ConfigureAwait(false);
            content.Should().Be(errorMessage);
        }

        [When(@"^I call api with malicious url ""(.*)""$")]
#pragma warning disable S4144 // Methods should not have identical implementations
        public async Task WhenICallApiWithMaliciousUrlApiValuesQueryScriptAlertScriptAsync(string url)
#pragma warning restore S4144 // Methods should not have identical implementations
        {
            var client = scenarioContext.Get<HttpClient>(Constants.Client);
            _response?.Dispose();
            _response = await client.GetAsync(url).ConfigureAwait(false);
        }

        public void Dispose() => _response?.Dispose();
    }
}
