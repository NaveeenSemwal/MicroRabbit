using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Infra.Bus
{
    public class RabbitMQBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _events;

        public RabbitMQBus(IMediator mediator)
        {
            _mediator = mediator;
            _handlers = new Dictionary<string, List<Type>>();
            _events = new List<Type>();
        }
        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public void Publish<T>(T @event) where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var eventName = @event.GetType().Name;

                    channel.QueueDeclare(eventName, false, false, false, null);

                    var message = JsonConvert.SerializeObject(@event);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", eventName, null, body);
                }
            }
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_events.Contains(typeof(T)))
            {
                _events.Add(typeof(T));
            }

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers.Add(eventName, new List<Type>());

            }

            _handlers[eventName].Add(handlerType);

            StartBasicConsume<T>();

        }

        private void StartBasicConsume<T>() where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localhost", DispatchConsumersAsync = true };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var eventName = typeof(T).Name;

            channel.QueueDeclare(eventName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            channel.BasicConsume(eventName, true, consumer);

        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                var subscriptions = _handlers[eventName];

                foreach (var subscription in subscriptions)
                {
                    var handler = Activator.CreateInstance(subscription);
                    if (handler == null) continue;
                    

                    var eventType = _events.FirstOrDefault(x => x.Name == eventName);

                    var @event = JsonConvert.DeserializeObject(message, eventType);

                    var concrete = typeof(IEventHandler<>).MakeGenericType(eventType);

                    await (Task)concrete.GetMethod("Handle").Invoke(handler, new object[] { @event });
                }
            }
        }
    }
}
