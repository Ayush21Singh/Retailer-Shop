using AshishGeneralStore.DTOs.Authentication;
using AshishGeneralStore.Models;

namespace AshishGeneralStore.Services.Authentication
{
    public interface IAuthService
    {
        Task<TokenDto> LoginAsync(LoginDto loginDto, bool forceLogin = false);
        Task<User> RegisterAsync(UserCreateDto userDto);
        Task<TokenDto> RefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);

        Task RevokeAllTokensAsync(int userId);
    }
}
