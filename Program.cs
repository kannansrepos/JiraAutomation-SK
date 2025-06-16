using JiraAutomation_SK.Kernels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
TestcaseGenerator.CreateTestCaseGenrator();
await TestcaseGenerator.GenerateTestCaseAsync("TEST-123");
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.Run();
