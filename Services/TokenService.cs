using Microsoft.IdentityModel.Tokens;
using SelfIdentity.Data;
using SelfIdentity.Entities;
using SelfIdentity.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SelfIdentity.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;

    public TokenService(IConfiguration configuration, AppDbContext dbContext)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"]!));
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public string CreateToken(User user)
    {
        // Create base claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username), // Username
            new Claim(ClaimTypes.Email, user.Email)    // Email
        };

        // Fetch user roles from the database
        var userRoles = _dbContext.UserRoles
            .Where(ur => ur.UserId == user.UserId)
            .Select(ur => ur.Role.Name)
            .ToList();

        // Add roles as claims
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Define signing credentials
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        // Define token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
            SigningCredentials = creds,
            Issuer = _configuration["JWT:Issuer"],
            Audience = _configuration["JWT:Audience"]
        };

        // Create the token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the token as a string
        return tokenHandler.WriteToken(token);
    }
}
