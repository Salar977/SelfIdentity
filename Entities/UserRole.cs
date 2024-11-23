using System.ComponentModel.DataAnnotations.Schema;

namespace SelfIdentity.Entities;

public class UserRole
{
    [ForeignKey(nameof(UserId))]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [ForeignKey(nameof(RoleId))]
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
