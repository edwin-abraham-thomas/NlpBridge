
using Console;
using Console.Models.Request;
using Console.Models.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NlpBridge.DependencyInjection;

internal class Program
{

    private static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        BuildConfig(builder);

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                //services.RegisterNlpBridge<Request, Response>(
                //    context.Configuration.GetValue<string>("NlpApiUrl"),
                //    new Dictionary<string, string>
                //    {
                //        { context.Configuration.GetValue<string>("NlpApiKeyName"), context.Configuration.GetValue<string>("NlpApiKey") }
                //    }
                //    );
                services.RegisterNlpBridge<Request, Response>(new NlpBridge.Models.Config<Request>
                {
                    NlpServiceUrl = context.Configuration.GetValue<string>("NlpApiUrl"),
                    NlpUrlParams = new Dictionary<string, string>
                    {
                        { context.Configuration.GetValue<string>("NlpApiKeyName"), context.Configuration.GetValue<string>("NlpApiKey") }
                    },
                    PromptProperty = x => x.Contents[0].Parts[0].Text,
                });
                services.AddSingleton<IService, Service>();
            })
            .Build();

        var svc = ActivatorUtilities.CreateInstance<Service>(host.Services);
        await svc.RunAsync();
    }

    static void BuildConfig(IConfigurationBuilder builder)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables();
    }
}