using System.ComponentModel.DataAnnotations;

namespace SelfIdentity.Entities;

public class User
{
    [Key]
    public int UserId { get; set; }
    public required string Username { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public DateTime Created {  get; set; }
    public DateTime? Updated { get; set; }


    // Navigation property for roles
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
