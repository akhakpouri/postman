using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Postman.Extensions;
using Postman.Pub;
using Postman.PubSub.Business;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .AddJsonFile("appsettings.json")
        .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
        logging.AddFilter("System", LogLevel.Error)
        .AddFilter<DebugLoggerProvider>("Microsoft", LogLevel.Error)
        .AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.Error))
    
    .Build();
services.AddLogging(config =>
{
    config.AddDebug();
    config.AddConsole();
});
services.AddRabbitMqPublisher(configuration);
services.AddInternals();
services.AddTransient<IHello, Hello>();

var sp = services.BuildServiceProvider();
var hello = sp.GetRequiredService<IHello>();
await hello.Say();
