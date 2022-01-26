using System.Threading.Tasks;
using LeonardoStore.Customer.Application.Commands;
using LeonardoStore.Customer.Application.Commands.Handlers;
using LeonardoStore.SharedContext.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LeonardoStore.Customer.Api.Controllers
{
    [Route("api/v1")]
    public class LoginCustomerController : Controller
    {
        private readonly LoginCommandHandler _handler;

        public LoginCustomerController(LoginCommandHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("login")]
        public Task<ICommandResult> Login([FromBody] LoginCustomerCommand command)
        {
            return _handler.HandleAsync(command);
        }
    }
}