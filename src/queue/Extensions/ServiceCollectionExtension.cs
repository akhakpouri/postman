using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sontiq.Queue.RabbitMq;

namespace Sontiq.Queue.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services, IConfiguration config)
    {
        var rabbitConfig = config.GetSection("RabbitMQ").Get<RabbitConfig>();
        services.AddRabbitQueuePublisher(rabbitConfig);
        return services;
    }
        
    public static IServiceCollection AddRabbitMqListener(this IServiceCollection services, IConfiguration config)
    {
        var rabbitConfig = config.GetSection("RabbitMQ").Get<RabbitConfig>();
        services.AddRabbitQueueListener(rabbitConfig);
        return services;
    }
}