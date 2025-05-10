using Azure.Core;
using EventBooking.Application.Models;
using EventBooking.Core.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace EventBooking.Application.Services.IService
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);

        Task<AuthModel> GetTokenAsync(TokenRequest model);

    }
}
