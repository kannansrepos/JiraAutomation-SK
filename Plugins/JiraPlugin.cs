using Microsoft.SemanticKernel;
using RestSharp;
using System.Text;

namespace JiraAutomation_SK.Plugins;

public class JiraPlugin
{
    private readonly RestClient _client;
    private readonly string _username;
    private readonly string _apiToken;
    public JiraPlugin(string baseUrl, string username, string apiToken)
    {
        _client = new RestClient(baseUrl);
        _username = username;
        _apiToken = apiToken;
    }

    [KernelFunction]
    public async Task<string> GetUserStory(string issueKey)
    {
        var request = new RestRequest($"/rest/api/2/issue/{issueKey}", Method.Get);
        request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_apiToken}"))}");

        var response = await _client.ExecuteAsync(request);
        return response.Content ?? "";
    }

    [KernelFunction]
    public async Task<string> CreateTestCase(string storyKey, string title, string steps)
    {
        var request = new RestRequest("/rest/api/2/issue", Method.Post);
        request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_apiToken}"))}");
        request.AddJsonBody(new
        {
            fields = new
            {
                project = new { key = "TEST" },
                summary = $"Test Case: {title}",
                description = steps,
                issuetype = new { name = "Test" },
                parent = new { key = storyKey }
            }
        });

        var response = await _client.ExecuteAsync(request);
        return response.Content ?? "";
    }
}
