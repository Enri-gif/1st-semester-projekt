using System;
using Reqnroll;

namespace Tests.ServiceTests;


[Binding]
public class TestHooks
{
    private readonly ScenarioContext _scenarioContext;

    public TestHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        Console.WriteLine("========================================");
        Console.WriteLine($"Starting scenario: {_scenarioContext.ScenarioInfo.Title}");
        Console.WriteLine("========================================");
    }

    [AfterScenario]
    public void AfterScenario()
    {
        if (_scenarioContext.TestError != null)
        {
            Console.WriteLine("Scenario failed!");
            Console.WriteLine($"Error: {_scenarioContext.TestError.Message}");
            Console.WriteLine(_scenarioContext.TestError.StackTrace);
        }
        else
        {
            Console.WriteLine("Scenario passed!");
        }

        Console.WriteLine("========================================");
    }

    [BeforeStep]
    public void BeforeStep()
    {
        var step = _scenarioContext.StepContext.StepInfo;

        Console.WriteLine($"> {step.StepDefinitionType} {step.Text}");
    }

    [AfterStep]
    public void AfterStep()
    {
        if (_scenarioContext.TestError != null)
        {
            var step = _scenarioContext.StepContext.StepInfo;

            Console.WriteLine($"Step failed: {step.Text}");
            Console.WriteLine($"Error: {_scenarioContext.TestError.Message}");
        }
        else
        {
            Console.WriteLine("✔ Step succeeded");
        }
    }
}
