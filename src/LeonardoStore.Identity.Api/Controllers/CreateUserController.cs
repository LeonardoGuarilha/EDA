using System.Linq;
using System.Threading.Tasks;
using LeonardoStore.Identity.Api.Models;
using LeonardoStore.Identity.Api.Services;
using LeonardoStore.SharedContext.Commands;
using LeonardoStore.SharedContext.IntegrationEvents;
using LeonardoStore.SharedContext.MessageBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeonardoStore.Identity.Api.Controllers
{
    [Route("api/v1/identity")]
    public class CreateUserController : Controller
    {
        private readonly AuthenticationAuthorizationService _authenticationAuthorizationService;
        //private readonly IMessageBusClient _messageBusClient;
        private readonly IMessageBus _messageBus;

        public CreateUserController(AuthenticationAuthorizationService authenticationAuthorizationService, 
            //IMessageBusClient messageBusClient, 
            IMessageBus messageBus)
        {
            _authenticationAuthorizationService = authenticationAuthorizationService;
            //_messageBusClient = messageBusClient;
            _messageBus = messageBus;
        }

        [HttpPost]
        [Route("user")]
        public async Task<ICommandResult> Login([FromBody] UserRegister userRegister)
        {
            if (!ModelState.IsValid)
                return new CommandResult(
                    false, 
                    "Não foi possível realizar seu cadastro, verifique as informações", 
                    null);
            
            var user = new IdentityUser
            {
                UserName = userRegister.Email,
                Email = userRegister.Email,
                EmailConfirmed = true
            };
            
            var result = await _authenticationAuthorizationService.UserManager.CreateAsync(user, userRegister.Password);

            if (result.Succeeded)
            {
                var clientResult = await RegisterCustomer(userRegister);

                if (!clientResult.Success)
                {
                    await _authenticationAuthorizationService.UserManager.DeleteAsync(user);
                    return new CommandResult(false, "Erro ao cadastrar cliente, favor verificar", clientResult.Data);
                }
            }
            
            return new CommandResult(true, "Erro ao criar usuário, favor verificar", 
                result.Errors.Select(e => e.Description).ToList());
        }

        [HttpPost]
        [Route("user/login")]
        public async Task<ICommandResult> Login([FromBody] UserLogin userLogin)
        {
            if (!ModelState.IsValid)
                return new CommandResult(
                    false, 
                    "Não foi possível realizar seu login, verifique as informações", 
                    null);
            
            var result = await _authenticationAuthorizationService.SignInManager.PasswordSignInAsync(
                userLogin.Email, userLogin.Password, false, false);
            
            if (result.Succeeded)
            {
                return new CommandResult(
                    true,
                    "Login realizado com sucesso",
                    new
                    {
                        jwt = _authenticationAuthorizationService.CreateJwt(userLogin.Email)
                    });
            }
            
            if(result.IsLockedOut)
                return new CommandResult(
                    false, 
                    "Usuário temporariamente bloqueado por tentativas inválidas", 
                    null);
            
            return new CommandResult(false, "Usuário ou senha incorretos", null);
        }

        private async Task<CommandResult> RegisterCustomer(UserRegister userRegister)
        {
            var usuario = await _authenticationAuthorizationService
                .UserManager.FindByEmailAsync(userRegister.Email);
            
            var userRegisteredEvent = new UserRegisteredEvent(userRegister.FirstName, userRegister.LastName, 
                userRegister.Document, userRegister.Email);

            try
            {
                
                return await _messageBus.RequestAsync<UserRegisteredEvent, CommandResult>(userRegisteredEvent);
            }
            catch
            {
                await _authenticationAuthorizationService.UserManager.DeleteAsync(usuario);
                throw;
            }
            
        }
        
    }
}