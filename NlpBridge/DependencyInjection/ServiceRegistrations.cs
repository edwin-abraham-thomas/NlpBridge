using Microsoft.Extensions.DependencyInjection;
using NlpBridge.Models;
using System.Net.Http.Headers;
using System.Web;

namespace NlpBridge.DependencyInjection
{
    public static class ServiceRegistrations
    {
        public static IServiceCollection RegisterNlpBridge<TNlpRequest, TNlpResponse, TClientRequest, TClientResponse>(this IServiceCollection services,
            Config<TNlpRequest, TNlpResponse> config)
            where TNlpRequest : new()
            where TNlpResponse : new()
            where TClientRequest : new()
            where TClientResponse : new()
        {
            var uriBuilder = new UriBuilder(config.NlpServiceUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (config.NlpUrlParams != null)
            {
                foreach (var param in config.NlpUrlParams)
                {
                    query[param.Key] = param.Value;
                }
            }
            uriBuilder.Query = query.ToString();

            services.AddHttpClient(Constants.NlpServiceHttpClientName, httpClient =>
            {
                httpClient.BaseAddress = uriBuilder.Uri;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddScoped(typeof(IExecutor<,,,>), typeof(Executor<,,,>));
            services.AddSingleton(config);

            return services;
        }
    }
}
