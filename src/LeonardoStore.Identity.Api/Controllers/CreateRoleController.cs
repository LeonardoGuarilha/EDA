using System.Threading.Tasks;
using LeonardoStore.Identity.Api.Commands;
using LeonardoStore.Identity.Api.Commands.Handlers;
using LeonardoStore.SharedContext.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LeonardoStore.Identity.Api.Controllers
{
    [Route("api/v1/identity")]
    public class CreateRoleController : Controller
    {
        private readonly CreateRoleCommandHandler _handler;

        public CreateRoleController(CreateRoleCommandHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("role")]
        public async Task<ICommandResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            return await _handler.HandleAsync(command);
        }
    }
}