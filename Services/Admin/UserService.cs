using AshishGeneralStore.Data;
using AshishGeneralStore.DTOs.Inventory;
using AshishGeneralStore.Models;
using Microsoft.EntityFrameworkCore;

namespace AshishGeneralStore.Services.Admin;

public class UserService : IUserService
{
    private readonly StoreDbContext _context;

    public UserService(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Name = u.Name,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Name = u.Name,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                Role = u.Role
            })
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        return user;
    }

    public async Task<UserDto> CreateUserAsync(UserCreateUpdateDto userDto)
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

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Name = user.Name,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<bool> UpdateUserAsync(int id, UserCreateUpdateDto userDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        if (await _context.Users.AnyAsync(u => u.Username == userDto.Username && u.Id != id))
        {
            throw new InvalidOperationException("Username is already taken by another user.");
        }

        user.Username = userDto.Username;
        user.Name = userDto.Name;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
        user.PhoneNumber = userDto.PhoneNumber;
        user.Email = userDto.Email;
        user.Role = userDto.Role;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        // Prevent deleting the last admin
        if (user.Role == "Admin" && await _context.Users.CountAsync(u => u.Role == "Admin") == 1)
        {
            throw new InvalidOperationException("Cannot delete the last admin user.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}