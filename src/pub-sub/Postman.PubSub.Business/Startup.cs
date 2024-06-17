using Microsoft.Extensions.DependencyInjection;
using Sontiq.Queue.Business.PubSub.Publishers;
using Sontiq.Queue.PubSub.Business.Dto;
using Sontiq.Queue.PubSub.Business.Managers;
using Sontiq.Queue.PubSub.Business.Publishers;
using Sontiq.Queue.PubSub.Business.Services;
using Sontiq.Queue.PubSub.Business.Services.Hosted;

namespace Sontiq.Queue.PubSub.Business
{
    public static class Startup
    {
        

        public static IServiceCollection AddInternals(this IServiceCollection services) 
        {
            services.AddScoped<IUserConfirmPublisher, UserConfirmPublisher>();
            services.AddScoped<IFooClientPublisher, FooClientPublisher>();
            services.AddTransient<IManager<UserConfirmDto>, UserConfirmManager>();
            services.AddTransient<IManager<FooDto>, FooManager>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IFooService, FooService>();
            return services;
        }

        public static IServiceCollection AddHostedServices(this IServiceCollection services) 
        {
            services.AddHostedService<FooHostedService>();
            services.AddHostedService<UserConfirmationService>();
            return services;
        }
    }
}
