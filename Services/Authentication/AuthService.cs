using AshishGeneralStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AshishGeneralStore.Common;
using AshishGeneralStore.Models;
using AshishGeneralStore.DTOs.Authentication;
namespace AshishGeneralStore.Services.Authentication

{
    public class AuthService : IAuthService
    {
        private readonly StoreDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(StoreDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto, bool forceLogin = false)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            var existingToken = await _context.Tokens
                .SingleOrDefaultAsync(t => t.UserId == user.Id && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

            if (existingToken != null)
            {
                if (forceLogin)
                {
                    await RevokeAllTokensAsync(user.Id); // Revoke all existing tokens
                }
                else
                {
                    throw new InvalidOperationException("User is already logged in. Please log out from all devices or use force-login.");
                }
            }

            return await GenerateTokensAsync(user);
        }

        public async Task<User> RegisterAsync(UserCreateDto userDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                throw new InvalidOperationException("Username is already taken.");
            }

            var user = new User
            {
                Username = userDto.Username,
                Name = userDto.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                PhoneNumber = userDto.PhoneNumber,
                Email = userDto.Email,
                Role = userDto.Role
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.Tokens
                .SingleOrDefaultAsync(t => t.RefreshToken == refreshToken && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

            if (tokenEntity == null)
            {
                throw new SecurityTokenException("Invalid or revoked refresh token.");
            }

            var user = await _context.Users.FindAsync(tokenEntity.UserId);
            if (user == null)
            {
                throw new SecurityTokenException("User not found.");
            }

            // Delete the old refresh token instead of revoking it
            _context.Tokens.Remove(tokenEntity);
            await _context.SaveChangesAsync();

            return await GenerateTokensAsync(user);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.Tokens
                .SingleOrDefaultAsync(t => t.RefreshToken == refreshToken && !t.IsRevoked);

            if (tokenEntity != null)
            {
                _context.Tokens.Remove(tokenEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllTokensAsync(int userId)
        {
            var tokens = await _context.Tokens
                .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            if (tokens.Any())
            {
                _context.Tokens.RemoveRange(tokens);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<TokenDto> GenerateTokensAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Constants.Jwt.Key);

            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),   
            new Claim("userId", user.Id.ToString()),              
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.Name),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = Constants.Jwt.Issuer,
                Audience = Constants.Jwt.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
            var accessTokenString = tokenHandler.WriteToken(accessToken);

            var refreshTokenString = Guid.NewGuid().ToString("N");
            var refreshTokenEntity = new Token
            {
                RefreshToken = refreshTokenString,
                UserId = user.Id,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                IsRevoked = false
            };
            _context.Tokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenDto
            {
                AccessToken = accessTokenString,
                RefreshToken = refreshTokenString
            };
        }

    }
}
