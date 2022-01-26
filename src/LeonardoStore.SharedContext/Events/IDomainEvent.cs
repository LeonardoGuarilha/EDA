using System;

namespace LeonardoStore.SharedContext.Events
{
    public interface IDomainEvent
    {
        Guid Id { get; }
        DateTime OccuredAt { get; }
    }
}