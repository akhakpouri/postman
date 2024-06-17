// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Sontiq.Queue.Extensions;
using Sontiq.Queue.PubSub.Business;

var services = new ServiceCollection();


static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSystemd()
        .ConfigureServices((hostContext, services) =>
        {
            var config = hostContext.Configuration;
            services.AddRabbitMqListener(config);
            services.AddInternals();
            services.AddServices();
            services.AddHostedServices();
        })
        .ConfigureLogging(logging =>
            logging.AddFilter("System", LogLevel.Trace)
                .AddFilter<DebugLoggerProvider>("Microsoft", LogLevel.Information)
                .AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.Trace));


var app = CreateHostBuilder(args).Build();
app.Run();