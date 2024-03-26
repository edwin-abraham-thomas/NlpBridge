
using Console;
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
                //NlpBridge Registration
                services.RegisterNlpBridge<
                    Console.Models.Nlp.Request.Request,
                    Console.Models.Nlp.Response.Response,
                    Console.Models.Client.Person,
                    Console.Models.Client.Credentials>(
                    new NlpBridge.Models.Config<Console.Models.Nlp.Request.Request, Console.Models.Nlp.Response.Response>
                    {
                        NlpServiceUrl = context.Configuration.GetValue<string>("NlpApiUrl"),
                        NlpUrlParams = new Dictionary<string, string>
                        {
                            { context.Configuration.GetValue<string>("NlpApiKeyName"), context.Configuration.GetValue<string>("NlpApiKey") }
                        },
                        PromptProperty = x => x.Contents[0].Parts[0].Text,
                        ResponseTextProperty = x => x.Candidates[0].Content.Parts[0].Text,
                        DefaultRequest = defaultRequest
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

    private static readonly Console.Models.Nlp.Request.Request defaultRequest = new Console.Models.Nlp.Request.Request
    {
        Contents = new List<Console.Models.Nlp.Request.Content>
                            {
                                new Console.Models.Nlp.Request.Content
                                {
                                    Parts = new List<Console.Models.Nlp.Request.Part>
                                    {
                                        new Console.Models.Nlp.Request.Part()
                                    }
                                }
                            }
    };
}