using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace graphql_subscription_poc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = CreateHost();
            await host.StartAsync();

            var consumer = host.Services.GetService<CategoryConsumer>();
            var log = host.Services.GetService<ILogger<Program>>();

            try
            {
                consumer.Subscribe();
            }
            catch (Exception e)
            {
                log.LogError(e, "falha na integração com graphql server");
            }
        }

        private static IHost CreateHost()
        {
            var builder = new HostBuilder();

            builder.ConfigureAppConfiguration((context, builder) =>
            {
                builder.Sources.Clear();
                builder
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables();
            });

            builder.ConfigureServices((context, builder) =>
            {
                var config = context.Configuration;

                builder.AddLogging(c => c.AddConsole()).Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Debug);
                builder.AddScoped<IGraphQLClient>(s => new GraphQLHttpClient(config["GraphQLURI"], new NewtonsoftJsonSerializer()));
                builder.AddScoped<CategoryConsumer>();
            });

            return builder.Build();
        }
    }
}
