using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LeonardoStore.Customer.Application.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.JwtSigningCredentials.Interfaces;

namespace LeonardoStore.Customer.Application.Services
{
    public class AuthenticationAuthorizationService
    {
        public readonly SignInManager<IdentityUser> SignInManager; // Gerencia questoes de login
        public readonly UserManager<IdentityUser> UserManager; // Gerencia como que eu administro o usuário
        private readonly IJsonWebKeySetService _jwksService;

        public AuthenticationAuthorizationService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IJsonWebKeySetService jwksService)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            _jwksService = jwksService;
        }
        
        public async Task<CommandResult> GerarJwt(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            var claims = await UserManager.GetClaimsAsync(user);

            var identityClaims = await ObterClaimsUsuario(claims, user);
            var encodedToken = CodificarToken(identityClaims);

            // Gera o Refresh Token
            //var refreshToken = await GerarRefreshToken(email);

            return ObterRespostaToken(encodedToken, user, claims);
        }
        
        private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await UserManager.GetRolesAsync(user);

            // Lista de claims
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            foreach (var userRole in userRoles)
            {
                // Adiciono as roles como claim
                // Para o identity isso é diferente, mas a gente vê da mesma forma
                claims.Add(new Claim("role", userRole));
            }

            // Uma role é um papel
            // Uma claim é um dado, um dado aberto. Ele pode representar tanto uma permissão
            // quanto um dado do usuário
            // Posso adicionar as roles como claims

            // ClaimsIdentity = Objeto real da coleção de claims que o usuário vai ter na sua representação
            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }
        
        private string CodificarToken(ClaimsIdentity identityClaims)
        {
            // Manipulador do Token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Pega o endpoint da API de Identidade
            var currentIssuer = "http://localhost:5000";
            
            // Gero a chave do token, vai gerar uma chave aleatória
            // Se ele não tiver, ele vai gerar uma
            var key = _jwksService.GetCurrent();
            // Gero os dados Token
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                // O Issuer é o prórprio endpoint da API de Identidade
                Issuer =  currentIssuer,//_appSettings.Emissor, // Vai ser o próprio endpoint da API de identidade
                //Audience = _appSettings.ValidoEm, Não tem mais o Audience, porque onde o Token for válido, ele é válido. Por isso tem agora a chave publica.
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(1), // Adiciona uma hora pelo padrão UTC para a expiração do token
                SigningCredentials = key
            });

            // Gera o token
            return tokenHandler.WriteToken(token);
        }
        
        private CommandResult ObterRespostaToken(string encodedToken, IdentityUser user,
            IEnumerable<Claim> claims)
        {
            return new CommandResult(true, "", new
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(1).TotalSeconds,
                Id = user.Id,
                Email = user.Email,
                Claims = claims.Select(c => new { c.Type, c.Value })
            });

        }
        
        // Anotação do sistema Unix de como exibir uma data
        // Essa data será exibida em um formato OffSet porque é o padrão do JWT
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    }
    
    
}