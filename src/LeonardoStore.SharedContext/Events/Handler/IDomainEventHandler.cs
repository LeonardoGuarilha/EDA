namespace LeonardoStore.SharedContext.Events.Handler
{
    public interface IDomainEventHandler<T> where T: IDomainEvent
    {
        void Handle(T domainEvent);
    }
}