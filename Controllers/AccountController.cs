using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfIdentity.Data;
using SelfIdentity.DTOs;
using SelfIdentity.Entities;
using SelfIdentity.Services.Interfaces;

namespace SelfIdentity.Controllers;
[Route("api/account")]
[ApiController]
public class AccountController(AppDbContext dbContext,
                               ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] UserRequest userRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new User
        {
            Username = userRequest.Username,
            Email = userRequest.Email,
            Created = DateTime.Now
        };
        user.PasswordSalt = BC.GenerateSalt();
        user.PasswordHash = BC.HashPassword(userRequest.Password, user.PasswordSalt);

        try
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            await AssignRole("User", user.Username);
            return Ok(new { Message = "User created successfully", user.UserId });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == userLogin.Username.ToLower());
        if (user == null) return Unauthorized("Invalid login credentials.");

        if(!BC.Verify(userLogin.Password, user.PasswordHash))
            return Unauthorized("Invalid login credentials.");

        return Ok(
            new UserToken(
                user.Username,
                user.Email,
                tokenService.CreateToken(user))
            );

    }

    [Authorize(Roles = "Admin")]
    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole(string role)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var roleExists = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name.Equals(role));
        if(roleExists is not null) return Conflict($"{role} Role Exists.");

        var createRole = new Role { Name = role };

        await dbContext.Roles.AddAsync(createRole);
        await dbContext.SaveChangesAsync();

        return Ok(new { message = $"{role} Role Added Successfully."});
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(string role, string username)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Check if user exists
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username.Equals(username));
        if (user is null) return NotFound("User not found.");

        // Check if role exists
        var roleExists = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name.Equals(role));
        if (roleExists is null) return NotFound("Role not found.");

        // Check if user already has the role
        var isAlreadyRole = await dbContext.UserRoles.AnyAsync(x => x.UserId == user.UserId && x.RoleId == roleExists.RoleId);
        if (isAlreadyRole) return Conflict($"User already has the role '{roleExists.Name}'.");

        // Assign role to user
        var userRole = new UserRole { UserId = user.UserId, RoleId = roleExists.RoleId };
        await dbContext.UserRoles.AddAsync(userRole);
        await dbContext.SaveChangesAsync();

        return Ok($"Role '{roleExists.Name}' assigned successfully to user '{username}'.");
    }

}
