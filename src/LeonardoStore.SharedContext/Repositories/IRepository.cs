using System;
using LeonardoStore.SharedContext.Entities;

namespace LeonardoStore.SharedContext.Repositories
{
    public interface IRepository<T>: IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}