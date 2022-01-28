using System;
using System.Threading.Tasks;
using LeonardoStore.Identity.Api.Models;
using LeonardoStore.Identity.Api.Services;
using LeonardoStore.SharedContext.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeonardoStore.Identity.Api.Controllers
{
    [Route("api/v1/identity")]
    public class CreateUserController : Controller
    {
        private readonly AuthenticationAuthorizationService _authenticationAuthorizationService;

        public CreateUserController(AuthenticationAuthorizationService authenticationAuthorizationService)
        {
            _authenticationAuthorizationService = authenticationAuthorizationService;
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
                // Fazer ontegração com customer(RabbitMQ)
                // Verificar se foi salvo com sucesso o customer
                // Caso não, deletar o usuário criado
                // Retornar o ICommandResult
            }
            
            return new CommandResult(true, "Usuário criado com sucesso", 
                new
                {
                    Id = user.Id,
                    Email = userRegister.Email
                });
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
        
    }
}