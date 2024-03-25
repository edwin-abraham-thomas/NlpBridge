﻿using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using NlpBridge.Models;
using NlpBridge.Utils;
using System.Net.Http.Json;
using System.Text.Json;

namespace NlpBridge
{
    public class Executor<TClientRequest, TClientResponse, TNlpRequest, TNlpResponse> : IExecutor<TClientRequest, TClientResponse, TNlpRequest, TNlpResponse>
        where TClientRequest : new()
        where TClientResponse : new()
        where TNlpRequest : new()
        where TNlpResponse : new()
    {
        private readonly Config<TNlpRequest, TNlpResponse> _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public Executor(Config<TNlpRequest, TNlpResponse> config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<TClientResponse> ExecuteAsync(TClientRequest request)
        {
            // Build NLP prompt
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(TClientResponse));


            var prompt = $"Act as a tightly typed llm and respond in json format following below json schema structure: \n" +
                $"{schema}" +
                $"\n" +
                $"Use ``` as delimiters around the json response like ```{{}}```" +
                $"\n" +
                $"Build a fancy username and password using the provided information below" +
                $"{JsonConvert.SerializeObject(request)}";

            // Assign to NLP request
            ExpressionHelpers.SetValue(_config.DefaultRequest, _config.PromptProperty, prompt);
            Console.WriteLine(JsonConvert.SerializeObject(_config.DefaultRequest, Formatting.Indented));

            var httpClient = _httpClientFactory.CreateClient(Constants.NlpServiceHttpClientName);

            // Call NLP model API
            var response = await httpClient.PostAsJsonAsync(
                string.Empty,
                _config.DefaultRequest,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            var responseContent = await response.Content.ReadFromJsonAsync<TNlpResponse>();

            // Extract response text
            var apiResponseText = ExpressionHelpers.GetValue(responseContent, _config.ResponseTextProperty);

            // Parse response text to client response object
            int startIndex = apiResponseText.IndexOf("```") + 3; // Adding 3 to skip the triple backticks
            int endIndex = apiResponseText.LastIndexOf("```");
            string jsonObjectString = apiResponseText.Substring(startIndex, endIndex - startIndex);
            var clientResponse = JsonConvert.DeserializeObject<TClientResponse>(jsonObjectString);

            return clientResponse;
        }
    }
}