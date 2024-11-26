using Microsoft.EntityFrameworkCore;
using SelfIdentity.Data;
using SelfIdentity.DTOs;
using SelfIdentity.Services.Interfaces;

namespace SelfIdentity.Services;

public class UserService(AppDbContext dbContext) : IUserService
{
    public async Task<UserResponse> GetUserByIdAsync(int id)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == id);

        if (user == null) return null!;

        return new UserResponse(
            user.UserId,
            user.Username,
            user.Email,
            user.Created,
            user.Updated);
    }

    public async Task<IEnumerable<UserResponse>> GetUsersAsync()
    {
        return await dbContext.Users.Select(
            user => new UserResponse(
            user.UserId,
            user.Username,
            user.Email,
            user.Created,
            user.Updated)).ToListAsync();
    }
}