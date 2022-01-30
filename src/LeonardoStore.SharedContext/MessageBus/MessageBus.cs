using System;
using System.Threading.Tasks;
using EasyNetQ;
using LeonardoStore.SharedContext.Commands;
using LeonardoStore.SharedContext.IntegrationEvents;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace LeonardoStore.SharedContext.MessageBus
{
    public class MessageBus : IMessageBus
    {
         private IBus _bus;
        private IAdvancedBus _advancedBus; // Outra interface do bus que tem recursos avançados, consigo manipular algumas coisas que a interface IBus não oferece

        private readonly string _connectionString; 

        public MessageBus(string connectionString)
        {
            _connectionString = connectionString;
            TryConnect();
        }

        public bool IsConnected => _bus?.IsConnected ?? false; // Se o bus existir, retorna se o mesmo está conectado ou não
        public IAdvancedBus AdvancedBus => _bus?.Advanced; // Se existir um bus, eu quero o Advanced dele.

        public void Publish<T>(T message) where T : IntegrationEventsBase
        {
            TryConnect();
            _bus.Publish(message);
        }
        
        public async Task PublishAsync<T>(T message) where T : IntegrationEventsBase
        {
            TryConnect();
            await _bus.PublishAsync(message);
        }

        public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class
        {
            TryConnect();
            _bus.Subscribe(subscriptionId, onMessage);
        }

        public void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class
        {
            TryConnect();
            _bus.SubscribeAsync(subscriptionId, onMessage);
        }

        public TResponse Request<TRequest, TResponse>(TRequest request) where TRequest : IntegrationEventsBase
            where TResponse : CommandResult
        {
            TryConnect();
            return _bus.Request<TRequest, TResponse>(request);
        }

        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEventsBase where TResponse : CommandResult
        {
            TryConnect();
            return await _bus.RequestAsync<TRequest, TResponse>(request);
        }

        public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
            where TRequest : IntegrationEventsBase where TResponse : CommandResult
        {
            TryConnect();
            return _bus.Respond(responder);
        }

        public IDisposable RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEventsBase where TResponse : CommandResult
        {
            TryConnect();
            return _bus.RespondAsync(responder);
        }

        private void TryConnect()
        {
            if(IsConnected) return;

            var policy = Policy.Handle<EasyNetQException>() // Se for exceção do EasyNetQ
                .Or<BrokerUnreachableException>() // Se for exceção do RabbitMQ
                .WaitAndRetry(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() =>
            {
                _bus = RabbitHutch.CreateBus(_connectionString); // Enivar para a fila
                _advancedBus = _bus.Advanced;
                // O Disconected é um EventHandler
                _advancedBus.Disconnected += OnDisconnect; // Assim que a conexão do bus for desconectada
            });
        }

        // Quando a fila for desconectada
        private void OnDisconnect(object s, EventArgs e) // Padrão de assinatura de um EventHandler
        {
            var policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .RetryForever(); // Enquanto eu estiver conectado, estarei o tempo inteiro tentando reconectar

            policy.Execute(TryConnect);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }
    }
}