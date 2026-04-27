using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JogsifoglaloApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Ez az email cím már foglalt.");
            }

            user.Jelszo = BCrypt.Net.BCrypt.HashPassword(password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Jelszo))
            {
                return null;
            }

            return CreateToken(user);
        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Felhasznalo_Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Szerepkor_Nev.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
