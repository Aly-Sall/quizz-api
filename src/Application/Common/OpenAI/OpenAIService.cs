using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace _Net6CleanArchitectureQuizzApp.Application.Common.OpenAI;
public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIService(IConfiguration configuration, HttpClient httpClient)
    {
        _apiKey = configuration["OpenAI:ApiKey"];
        _httpClient = httpClient;
    }
    public async Task<string> GenerateQuizQuestionAsync(string topic)
    {
        var prompt = $"Create a multiple choice question about {topic}. Include the question, 4 options, the correct option index, and a brief explanation.";

        var requestBody = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };
        if (string.IsNullOrEmpty(_apiKey))
        {
            return null;
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var message = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
        return message;


    }
}
