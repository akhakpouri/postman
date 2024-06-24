using Microsoft.Extensions.DependencyInjection;
using Postman.Codecs;
using Postman.RabbitMq;
using Sontiq.Queue.Codecs;

namespace Postman;

public static class Startup
{
    public static IServiceCollection AddRabbitQueueListener(this IServiceCollection serviceCollection, RabbitConfig config, Func<IServiceProvider, IQueueMessageDecoder> decoder = null, Func<IServiceProvider, IRabbitMessageRouterHelper> router = null, string name = null)
    {
        serviceCollection.AddSingleton(servicesProvider => router != null ? router.Invoke(servicesProvider) : new StandardRabbitMessageRouterHelper());
        serviceCollection.AddSingleton(servicesProvider => decoder != null ? decoder.Invoke(servicesProvider) : new StandardQueueMessageCodec());
        return serviceCollection.AddQueueListener<RabbitMessageQueueListener, RabbitConfig>(config, name);
    }

    public static IServiceCollection AddRabbitQueuePublisher(this IServiceCollection serviceCollection, RabbitConfig config, Func<IServiceProvider, IQueueMessageEncoder> encoder = null, Func<IServiceProvider, IRabbitMessageRouterHelper> router = null)
    {
        serviceCollection.AddSingleton(servicesProvider => router != null ? router.Invoke(servicesProvider) : new StandardRabbitMessageRouterHelper());
        serviceCollection.AddSingleton(servicesProvider => encoder != null ? encoder.Invoke(servicesProvider) : new StandardQueueMessageCodec());
        return serviceCollection.AddQueuePublisher<RabbitMessageQueuePublisher, RabbitConfig>(config);
    }

}