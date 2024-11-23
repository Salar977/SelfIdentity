namespace SelfIdentity.Entities;

public class Role
{
    public int RoleId { get; set; }
    public required string Name { get; set; }


    // Navigation property for users
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
