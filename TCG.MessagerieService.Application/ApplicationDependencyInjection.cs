using System.Reflection;
using GreenPipes;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TCG.Common.MassTransit.Messages;
using TCG.Common.Settings;
using TCG.MessagerieService.Application.Consumer;

namespace TCG.MessageService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
       
        return services;
    }
    
    public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection serviceCollection)
    {
        //Config masstransit to rabbitmq
        serviceCollection.AddMassTransit(configure =>
        {
            configure.AddConsumer<AddMessageConsumer>();
            configure.AddConsumer<UpdateOfferInMessageConsumer>();
            configure.UsingRabbitMq((context, configurator) =>
            {
                var config = context.GetService<IConfiguration>();
                //On récupère le nom de la table Catalog
                ////On recupère la config de seeting json pour rabbitMQ
                var rabbitMQSettings = config.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                configurator.Host(new Uri(rabbitMQSettings.Host));
                // Retry policy for consuming messages
                configurator.UseMessageRetry(retryConfig =>
                {
                    // Exponential back-off (second argument is the max retry count)
                    retryConfig.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
                });
                
                //Defnir comment les queues sont crées dans rabbit
                //configurator.ConfigureEndpoints(context);
                configurator.ReceiveEndpoint("postservice", e =>
                {
                    e.Consumer<AddMessageConsumer>(context);
                    e.Consumer<UpdateOfferInMessageConsumer>(context);
                });
            });
        });
        //Start rabbitmq bus pour exanges
        serviceCollection.AddMassTransitHostedService();
        return serviceCollection;
    }
}