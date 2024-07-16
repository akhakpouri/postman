using Microsoft.Extensions.DependencyInjection;
using Postman.PubSub.Business.Dto;
using Postman.PubSub.Business.Managers;
using Postman.PubSub.Business.Publishers;
using Postman.PubSub.Business.Services;
using Postman.PubSub.Business.Services.Hosted;

namespace Postman.PubSub.Business
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
