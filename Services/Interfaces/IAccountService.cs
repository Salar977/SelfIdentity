using SelfIdentity.DTOs;
using SelfIdentity.Entities;

namespace SelfIdentity.Services.Interfaces;

public interface IAccountService
{
    Task<ServiceResponse> RegisterUserAsync(UserRequest userRequest);
    Task<UserToken> LoginAsync(UserLogin userLogin);
    Task<ServiceResponse> AddRoleAsync(string role);
    Task<ServiceResponse> AssignRoleAsync(string role, string username);

}
