using JiraAutomation_SK.Configs;
using JiraAutomation_SK.Plugins;
using Microsoft.SemanticKernel;

namespace JiraAutomation_SK.Kernels;

public static class TestcaseGenerator
{
    private static readonly ExtConfig config;
    private static readonly Kernel kernel;
    private static KernelFunction testGenFunction;

    static TestcaseGenerator()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        config = configuration.GetSection(ExtConfig.Key).Get<ExtConfig>()!;
        var builder = Kernel.CreateBuilder();
        builder.Services.AddOpenAIChatCompletion(config.LLMModel, config.OpenAIKey);
        kernel = builder.Build();
        var promptTemplate = File.ReadAllText("testcase-generator.txt");
        testGenFunction = kernel.CreateFunctionFromPrompt(promptTemplate);

    }

    public static void CreateTestCaseGenrator()
    {
        
        var jiraPlugin = new JiraPlugin(config.JiraBaseUrl, config.JiraUsername, config.JiraApiToken);
        kernel.ImportPluginFromObject(jiraPlugin, "Jira");

    }
    public static async Task<List<string>> GenerateTestCaseAsync(string userStoryKey)
    {
        var story = await kernel.InvokeAsync("Jira", "GetUserStory", new() { ["issueKey"] = userStoryKey });
        var result = await kernel.InvokeAsync(testGenFunction, new() { ["input"] = story.ToString() });
        var testcases = new List<string>();
        Console.WriteLine("Generated Test Cases:\n" + result);

        var testCases = result.ToString().Split("\n\n");

        foreach (var testCase in testCases)
        {
            var lines = testCase.Split('\n');
            var title = lines[0].Trim('1', '.', ' ');
            var steps = string.Join("\n", lines.Skip(1));

            var createResponse = await kernel.InvokeAsync("Jira", "CreateTestCase", new()
            {
                ["storyKey"] = userStoryKey,
                ["title"] = title,
                ["steps"] = steps
            });

            Console.WriteLine($"Created Test Case:\n{createResponse}");
            testcases.Add(createResponse.ToString());
        }
        return testcases;
    }
}
