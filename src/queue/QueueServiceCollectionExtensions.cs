using Microsoft.Extensions.DependencyInjection;

namespace Postman;

/// <summary>
/// Dependency injection extensions for registering generic queue listeners and publishers
/// </summary>
internal static class QueueServiceCollectionExtensions
{
    public static IServiceCollection AddQueueListener<TListener, TConfig>(this IServiceCollection serviceCollection,
        TConfig config, string name = "")
        where TListener : class, IQueueListener<TConfig>
    {
        serviceCollection.AddSingleton<TListener>();
        serviceCollection.AddSingleton(typeof(IQueueListener), serviceProvider =>
        {
            var publisher = serviceProvider.GetRequiredService<TListener>();
            publisher.Connect(config, name);

            return publisher;
        });

        return serviceCollection;
    }

    public static IServiceCollection AddQueuePublisher<TPublisher, TConfig>(this IServiceCollection serviceCollection,
        TConfig config)
        where TPublisher : class, IQueuePublisher<TConfig>
    {
        serviceCollection.AddSingleton<TPublisher>();
        serviceCollection.AddSingleton(typeof(IQueuePublisher), serviceProvider =>
        {
            var publisher = serviceProvider.GetRequiredService<TPublisher>();
            publisher.Connect(config);

            return publisher;
        });

        return serviceCollection;
    }
}