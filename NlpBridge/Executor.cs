using Newtonsoft.Json;
using NlpBridge.Models;
using NlpBridge.Utils;
using System;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace NlpBridge
{
    public class Executor<TRequest, TResponse> : IExecutor<TRequest, TResponse> 
        where TRequest: new() where TResponse: new()
    {
        private readonly Config<TRequest> _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public Executor(Config<TRequest> config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<TResponse> ExecuteAsync(TRequest request)
        {
            ExpressionHelpers.SetValue(request, _config.PromptProperty, "Hi how are you. This is an updated request");
            Console.WriteLine(JsonConvert.SerializeObject(request, Formatting.Indented));

            var httpClient = _httpClientFactory.CreateClient(Constants.NlpServiceHttpClientName);

            var response = await httpClient.PostAsJsonAsync("", request, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var responseContent = await response.Content.ReadFromJsonAsync<TResponse>();

            return responseContent;
        }
    }
}