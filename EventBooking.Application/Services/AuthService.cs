using EventBooking.Application.Models;
using EventBooking.Application.Services.IService;
using EventBooking.Core.Entities;
using EventBooking.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace EventBooking.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;

        public AuthService(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IOptions<JWT> jwt)
        {
            _db = db;
            _userManager = userManager;
            _jwt = jwt.Value;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, Roles.Role_User);
            var jwtSecurityToken = await createjwtToken(user);
            
            return new AuthModel
            {
                IsAuthenticated = true,
                Roles = new List<string> { Roles.Role_User },
                Username = user.UserName,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
                ApplicationUserId = user.Id,
            };
        }

        public async Task<AuthModel> GetTokenAsync(TokenRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthModel { Message = "Email or Password is incorrect!" };

            return new AuthModel
            {
                IsAuthenticated = true,
                Roles = new List<string> { Roles.Role_User },
                Username = model.Email,
                Email = model.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(await createjwtToken(user)),
                ExpiresOn = DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                ApplicationUserId = user.Id
            };
        }

        private async Task<JwtSecurityToken> createjwtToken(ApplicationUser user)
        {

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach(var role in roles)
            {
                roleClaims.Add(new Claim("role", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
            };
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var siginingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: siginingCredentials
            );
            return token;
        }
    }
}