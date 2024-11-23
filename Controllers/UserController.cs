using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfIdentity.Data;
using SelfIdentity.DTOs;

namespace SelfIdentity.Controllers;
[Route("api/users")]
[ApiController]
public class UserController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
    {
        return Ok(await dbContext.Users.Select(
            user => new UserResponse(
            user.UserId,
            user.Username,
            user.Email,
            user.Created,
            user.Updated)).ToListAsync());
    }
}
