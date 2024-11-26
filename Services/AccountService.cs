using Microsoft.EntityFrameworkCore;
using SelfIdentity.Data;
using SelfIdentity.DTOs;
using SelfIdentity.Entities;
using SelfIdentity.Services.Interfaces;

namespace SelfIdentity.Services;

public class AccountService(AppDbContext dbContext,
                            ITokenService tokenService) : IAccountService
{
    public async Task<ServiceResponse> AddRoleAsync(string role)
    {
        var roleExists = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name.Equals(role));
        if (roleExists is not null) return new ServiceResponse(Message: $"{role} already exists.");

        var createRole = new Role { Name = role };

        await dbContext.Roles.AddAsync(createRole);
        await dbContext.SaveChangesAsync();

        return new ServiceResponse(true, Message: $"{role} Role Added Successfully.");
    }

    public async Task<ServiceResponse> AssignRoleAsync(string role, string username)
    {
        var userRoleInfo = await dbContext.Users
            .Where(u => u.Username == username)
            .Select(u => new
            {
                User = u,
                Role = dbContext.Roles.FirstOrDefault(r => r.Name == role),
                HasRole = dbContext.UserRoles.Any(ur => ur.UserId == u.UserId && ur.Role.Name == role),
            }).FirstOrDefaultAsync();

        if (userRoleInfo is null || userRoleInfo.User is null) return new ServiceResponse(Message: "User not found.");
        if (userRoleInfo.Role is null) return new ServiceResponse(Message: "Role not found.");
        if (userRoleInfo.HasRole) return new ServiceResponse(Message: "The user already has the specified role.");

        // Assign role to user
        var userRole = new UserRole { UserId = userRoleInfo.User.UserId, RoleId = userRoleInfo.Role.RoleId };
        await dbContext.UserRoles.AddAsync(userRole);
        await dbContext.SaveChangesAsync();

        return new ServiceResponse(true, Message: $"Role '{userRoleInfo.Role.Name}' assigned successfully to user '{username}'.");
    }

    public async Task<UserToken> LoginAsync(UserLogin userLogin)
    {
        // Check if user exists
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == userLogin.Username.ToLower());
        if (user == null) return null!;

        // Verify password
        if (!BC.Verify(userLogin.Password, user.PasswordHash))
            return null!;

        // Return user token
        return new UserToken(
            user.Username,
            user.Email,
            tokenService.CreateToken(user));
    }

    public async Task<ServiceResponse> RegisterUserAsync(UserRequest userRequest)
    {
        try
        {
            var user = new User
            {
                Username = userRequest.Username,
                Email = userRequest.Email,
                Created = DateTime.Now
            };
            user.PasswordSalt = BC.GenerateSalt();
            user.PasswordHash = BC.HashPassword(userRequest.Password, user.PasswordSalt);

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            await AssignRoleAsync(nameof(User), user.Username);

            return new ServiceResponse(true, Message: "User created successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return new ServiceResponse(Message: "Something went wrong");
        }
    }
}
