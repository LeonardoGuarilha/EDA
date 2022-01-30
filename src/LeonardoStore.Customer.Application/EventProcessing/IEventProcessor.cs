using System.Threading.Tasks;

namespace LeonardoStore.Customer.Application.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string message);
    }
}