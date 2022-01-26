using System.Threading.Tasks;
using LeonardoStore.Customer.Application.Commands;
using LeonardoStore.Customer.Application.Commands.Handlers;
using LeonardoStore.SharedContext.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LeonardoStore.Customer.Api.Controllers
{
    [Route("api/v1")]
    public class CreateCustomerController : Controller
    {
        private readonly CustomerCommandHandler _handler;

        public CreateCustomerController(CustomerCommandHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("customer")]
        public Task<ICommandResult> CreateCustomer([FromBody] CreateCustomerCommand command)
        {
            return _handler.HandleAsync(command);
        }
    }
}