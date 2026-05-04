using Reqnroll;

namespace api.Reqnroll;

[Binding]
public class Hooks
{
    private readonly ScenarioContext _scenarioContext;

    public Hooks (ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario ()
    {
        var factory = new ApiFactory ();
        var client = factory.CreateClient ();

        _scenarioContext.Set (client);
    }
}
