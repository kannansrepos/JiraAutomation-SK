namespace JiraAutomation_SK.Configs;

public class ExtConfig
{
    internal static string Key = "OpenApiConfig";
    public string LLMModel { get; set; } = "gpt-4";
    public string OpenAIKey { get; set; } = string.Empty;
    public string JiraBaseUrl { get; set; } = "https://your-jira-instance.atlassian.net";
    public string JiraUsername { get; set; } = "your-jira-username";
    public string JiraApiToken { get; set; } = "your-jira-api-token";

}
