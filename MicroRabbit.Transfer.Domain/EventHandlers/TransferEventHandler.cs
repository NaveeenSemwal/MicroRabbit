using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Domain.Events;
using MicroRabbit.Transfer.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Transfer.Domain.EventHandlers
{
    public class TransferEventHandler : IEventHandler<TransferCreatedEvent>
    {
        private readonly ITransferRepository _transferRepository;

        public TransferEventHandler(ITransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        /// <summary>
        /// This will be called by RabbitMQ
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task Handle(TransferCreatedEvent @event)
        {
            _transferRepository.Add(new Models.TransferLog { FromAccount = @event.From, ToAccount = @event.To, TransferAmount = @event.Amount });
            return Task.CompletedTask;
        }
    }
}
