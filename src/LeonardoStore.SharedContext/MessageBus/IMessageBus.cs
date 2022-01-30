using System;
using System.Threading.Tasks;
using EasyNetQ;
using LeonardoStore.SharedContext.Commands;
using LeonardoStore.SharedContext.IntegrationEvents;

namespace LeonardoStore.SharedContext.MessageBus
{
    public interface IMessageBus : IDisposable
    {
        bool IsConnected { get; }
        IAdvancedBus AdvancedBus { get; }

        // Esses métodos estão na interface IBus
        // Mas eu implemento da forma que eu quiser
        void Publish<T>(T message) where T : IntegrationEventsBase;

        Task PublishAsync<T>(T message) where T : IntegrationEventsBase;

        void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class;

        void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class;

        TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEventsBase
            where TResponse : CommandResult;

        Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEventsBase
            where TResponse : CommandResult;

        IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
            where TRequest : IntegrationEventsBase
            where TResponse : CommandResult;

        IDisposable RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEventsBase
            where TResponse : CommandResult;
    }
}