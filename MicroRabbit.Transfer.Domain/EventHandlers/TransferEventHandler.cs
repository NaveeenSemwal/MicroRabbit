using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Transfer.Domain.EventHandlers
{
    public class TransferEventHandler : IEventHandler<TransferCreatedEvent>
    {
        public TransferEventHandler()
        {

        }

        /// <summary>
        /// This will be called by RabbitMQ
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task Handle(TransferCreatedEvent @event)
        {
            return Task.CompletedTask;  
        }
    }
}
