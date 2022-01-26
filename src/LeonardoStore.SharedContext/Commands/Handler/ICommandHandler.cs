using System.Threading.Tasks;

namespace LeonardoStore.SharedContext.Commands.Handler
{
    public interface ICommandHandler<T> where T: ICommand
    {
        Task<ICommandResult> HandleAsync(T command);
    }
}