using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LeonardoStore.Identity.Api.Data;
using LeonardoStore.Identity.Api.Entities;
using LeonardoStore.Identity.Api.Models;
using LeonardoStore.SharedContext.RefreshTokenAppSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.JwtSigningCredentials.Interfaces;

namespace LeonardoStore.Identity.Api.Services
{
    public class AuthenticationAuthorizationService
    {
        public readonly SignInManager<IdentityUser> SignInManager; // Gerencia questoes de login
        public readonly UserManager<IdentityUser> UserManager; // Gerencia como que eu administro o usuário
        public readonly RoleManager<IdentityRole> RoleManager;
        private readonly IJsonWebKeySetService _jwksService;
        private readonly ApplicationDbContext _context;
        private readonly AppTokenSettings _appTokenSettingsSettings;

        public AuthenticationAuthorizationService(SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, 
            IJsonWebKeySetService jwksService, 
            ApplicationDbContext context, 
            IOptions<AppTokenSettings> appTokenSettingsSettings,
            RoleManager<IdentityRole> roleManager)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            _jwksService = jwksService;
            _context = context;
            RoleManager = roleManager;
            _appTokenSettingsSettings = appTokenSettingsSettings.Value;
        }
        
        public async Task<UserLoginData> CreateJwt(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            var claims = await UserManager.GetClaimsAsync(user);

            var identityClaims = await GetUserClaims(claims, user);
            var encodedToken = CodifyToken(identityClaims);

            // Gera o Refresh Token
            var refreshToken = await GerarRefreshToken(email);

            return GetTokenResponse(encodedToken, user, claims, refreshToken);
        }
        
        private async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await UserManager.GetRolesAsync(user);

            // Lista de claims
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
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
        
        private string CodifyToken(ClaimsIdentity identityClaims)
        {
            // Manipulador do Token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Pega o endpoint da API de Identidade
            var currentIssuer = "localhost";
            
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
        
        private UserLoginData GetTokenResponse(string encodedToken, IdentityUser user,
            IEnumerable<Claim> claims, RefreshToken refreshToken)
        {
            return new UserLoginData
            {
                AccessToken = encodedToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = TimeSpan.FromHours(1).TotalSeconds,
                UsuarioToken = new UserToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UserClaim { Type = c.Type, Value = c.Value })
                }
            };
        }
        
        // Anotação do sistema Unix de como exibir uma data
        // Essa data será exibida em um formato OffSet porque é o padrão do JWT
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        
        private async Task<RefreshToken> GerarRefreshToken(string email)
        {
            var refreshToken = new RefreshToken
            {
                Username = email,
                ExpirationDate = DateTime.UtcNow.AddHours(_appTokenSettingsSettings.RefreshTokenExpiration),
            };

            // Sempre que eu gerar um token para um usuário, eu removo os antigos
            _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(u => u.Username == email));
            
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
        
        public async Task<RefreshToken> GetRefreshToken(Guid refreshToken)
        {
            var token = await _context.RefreshTokens.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Token == refreshToken);

            return token != null && token.ExpirationDate.ToLocalTime() > DateTime.Now
                ? token
                : null;
        }
    }


}