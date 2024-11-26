using SelfIdentity.DTOs;

namespace SelfIdentity.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetUsersAsync();
    Task<UserResponse> GetUserByIdAsync(int id);
}
