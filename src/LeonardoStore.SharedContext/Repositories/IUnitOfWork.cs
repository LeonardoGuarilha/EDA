using System.Threading.Tasks;

namespace LeonardoStore.SharedContext.Repositories
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}