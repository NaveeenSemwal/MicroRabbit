using MicroRabbit.Domain.Core.Events;
using System.Threading.Tasks;

namespace MicroRabbit.Domain.Core.Bus
{
    /// <summary>
    /// Contravariant generic interface
    /// 
    /// For generic type parameters, the in keyword specifies that the type parameter is contravariant.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventHandler<in T> : IEventHandler where T : Event
    {
        Task Handle(T @event);
    }

    public interface IEventHandler
    {
    }
}