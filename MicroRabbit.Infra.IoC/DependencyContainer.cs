using MediatR;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data;
using MicroRabbit.Banking.Data.Repository;
using MicroRabbit.Banking.Domain.CommandHandlers;
using MicroRabbit.Banking.Domain.Commands;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Application.Services;
using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Data.Repository;
using MicroRabbit.Transfer.Domain.EventHandlers;
using MicroRabbit.Transfer.Domain.Events;
using MicroRabbit.Transfer.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MicroRabbit.Infra.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services, string serviceType)
        {
            // Domian bus
            services.AddSingleton<IEventBus, RabbitMQBus>(x =>
            {
                // Here we are injecting IServiceScopeFactory's object in the RabbitMQBus's constructor in order to pass its dependency
                var scopeFactory = x.GetRequiredService<IServiceScopeFactory>();
                return new RabbitMQBus(x.GetService<IMediator>(), scopeFactory);
            });

            if (serviceType == "Banking")
            {
                services.AddTransient<IAccountService, AccountService>();
                services.AddTransient<IAccountRepository, AccountRepository>();
                services.AddTransient<BankingDbContext>();
            }
            else
            {

                services.AddTransient<ITransferService, TransferService>();
                services.AddTransient<ITransferRepository, TransferRepository>();
                services.AddTransient<TransferDbContext>();

                // Subscriptions : So that any microservice can subscribe to it.
                services.AddTransient<TransferEventHandler>();
            }



            // Domain Events
            //services.AddTransient<IEventHandler<TransferCreatedEvent>, TransferEventHandler>();

            // Banking Domain Command
            services.AddTransient<IRequestHandler<CreateTransferCommand, bool>, TransferCommandHandler>();



        }

    }
}
