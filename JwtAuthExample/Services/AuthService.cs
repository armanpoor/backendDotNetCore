using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using YourNamespace.Models;

namespace YourNamespace.Services
{
    public interface IAuthService
    {
        LoginResponse Login(LoginRequest request, IConfiguration configuration);
    }

    public class AuthService : IAuthService
    {
        private List<User> users = new List<User>
        {
            new User { Username = "test", Password = "test" }
        };

        public LoginResponse Login(LoginRequest request, IConfiguration configuration)
        {
            var user = users.SingleOrDefault(u => u.Username == request.Username && u.Password == request.Password);
            if (user == null)
                return null;

            var token = GenerateJwtToken(user, configuration);

            return new LoginResponse { Token = token, Success = true };
        }

        private string GenerateJwtToken(User user, IConfiguration configuration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
