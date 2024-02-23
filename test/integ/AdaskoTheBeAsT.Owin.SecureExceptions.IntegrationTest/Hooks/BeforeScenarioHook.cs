using AdaskoTheBeAsT.Owin.SecureExceptions.IntegrationTest.Util;
using Microsoft.Owin.Testing;
using Reqnroll;
using SampleWebApi;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.IntegrationTest.Hooks;

[Binding]
public sealed class BeforeScenarioHook(ScenarioContext scenarioContext)
{
    [BeforeScenario]
    public void BeforeScenario()
    {
        scenarioContext[Constants.Server] = TestServer.Create<Startup>();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        var server = scenarioContext.Get<TestServer>(Constants.Server);
        server.Dispose();
    }
}
