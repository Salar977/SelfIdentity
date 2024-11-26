using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfIdentity.Data;
using SelfIdentity.DTOs;
using SelfIdentity.Services.Interfaces;

namespace SelfIdentity.Controllers;
[Route("api/users")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
    {
        return Ok(await userService.GetUsersAsync());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<UserResponse>> GetUserById(int id)
    {
        var userResponse = await userService.GetUserByIdAsync(id);
        if (userResponse is null) return NotFound("User not Found");
        return Ok(userResponse);
    }
}
