using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfIdentity.DTOs;
using SelfIdentity.Services.Interfaces;

namespace SelfIdentity.Controllers;
[Route("api/account")]
[ApiController]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRequest userRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await accountService.RegisterUserAsync(userRequest);

        if(!user.Success) return BadRequest(user);

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        var user = await accountService.LoginAsync(userLogin);
        if (user == null) return Unauthorized("Invalid login credentials.");

        return Ok(user);

    }

    [Authorize(Roles = "Admin")]
    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole(string role)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var addRole = await accountService.AddRoleAsync(role);
        if (!addRole.Success) return Conflict(addRole);

        return Ok(addRole);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(string role, string username)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var assignRole = await accountService.AssignRoleAsync(role, username);

        if(!assignRole.Success) return BadRequest(ModelState);

        return Ok(assignRole);
    }

}
