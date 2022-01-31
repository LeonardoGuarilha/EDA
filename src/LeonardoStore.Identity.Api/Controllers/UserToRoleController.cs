using System.Threading.Tasks;
using LeonardoStore.Identity.Api.Commands;
using LeonardoStore.Identity.Api.Commands.Handlers;
using LeonardoStore.SharedContext.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LeonardoStore.Identity.Api.Controllers
{
    [Route("api/v1/identity")]
    public class UserToRoleController : Controller
    {
        private readonly UserToRoleHandler _handler;

        public UserToRoleController(UserToRoleHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("user-to-role")]
        public async Task<ICommandResult> UserToRole([FromBody] UserToRoleCommand command)
        {
            return await _handler.HandleAsync(command);
        }
    }
}