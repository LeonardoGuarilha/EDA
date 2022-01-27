using System.Threading.Tasks;
using LeonardoStore.Customer.Application.Commands;
using LeonardoStore.Customer.Application.Commands.Handlers;
using LeonardoStore.SharedContext.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeonardoStore.Customer.Api.Controllers
{
    [Route("api/v1")]
    public class RoleController : Controller
    {
        private readonly RolesCommandHandler _handler;

        public RoleController(RolesCommandHandler handler)
        {
            _handler = handler;
        }
        
        [HttpPost]
        [Route("role")]
        public async Task<ICommandResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            return await _handler.HandleAsync(command);
        }

        [HttpPost]
        [Route("add-role-to-user")]
        public async Task<ICommandResult> AddToleToUser([FromBody] UserToRoleCommand command)
        {
            return await _handler.HandleAsync(command);
        }
        
        [HttpGet]
        [Route("teste")]
        [Authorize(Roles = "Admin")]
        public string Teste()
        {
            return "Teste";
        }
    }
}