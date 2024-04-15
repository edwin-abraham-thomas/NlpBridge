using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly ILogger<Executor<TClientRequest, TClientResponse, TNlpRequest, TNlpResponse>> _logger;

        public Executor(Config<TNlpRequest, 
            TNlpResponse> config, 
            IHttpClientFactory httpClientFactory, 
            ILogger<Executor<TClientRequest, TClientResponse, TNlpRequest, TNlpResponse>> logger)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ExecutionResult<TClientResponse>> ExecuteAsync(TClientRequest request, string prompt = null)
        {
            // Build NLP prompt
            var constructedPrompt = ConstructPrompt(prompt, request);
            _logger.LogTrace("Constructed prompt: {ConstructedPrompt}", constructedPrompt);

            // Assign to NLP request
            ExpressionHelpers.SetValue(_config.DefaultRequest, _config.PromptProperty, constructedPrompt);
            _logger.LogTrace("Nlp request: {NlpRequest}", JsonConvert.SerializeObject(_config.DefaultRequest, Formatting.Indented));

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
            _logger.LogTrace("Nlp response: {NlpResponse}", apiResponseText);

            // Parse response text to client response object
            try
            {

                int startIndex = apiResponseText.IndexOf("```") + 3; // Adding 3 to skip the triple backticks
                int endIndex = apiResponseText.LastIndexOf("```");
                string jsonObjectString = apiResponseText.Substring(startIndex, endIndex - startIndex);
                var clientResponse = JsonConvert.DeserializeObject<TClientResponse>(jsonObjectString);

                return ExecutionResult<TClientResponse>.Success(clientResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to pass nlp response");
                return ExecutionResult<TClientResponse>.Failure("Failed to pass nlp response", ex);
            }
        }

        private string ConstructPrompt(string prompt, TClientRequest request)
        {

            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(TClientResponse));

            var constructedPrompt = $"Act as a strictly typed llm and respond in json format following below json schema structure: \n" +
                $"{schema}" +
                $"\n" +
                $"Use ``` as delimiters around the json response like ```{{}}```" +
                $"{prompt} \n\n" +
                $"{JsonConvert.SerializeObject(request)}";

            return constructedPrompt;
        }
    }
}