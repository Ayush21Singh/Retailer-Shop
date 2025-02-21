using AshishGeneralStore.DTOs.Inventory;

namespace AshishGeneralStore.Services.Admin
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(UserCreateUpdateDto userDto);
        Task<bool> UpdateUserAsync(int id, UserCreateUpdateDto userDto);
        Task<bool> DeleteUserAsync(int id);
    }
}
