namespace SelfIdentity.DTOs;

public record UserResponse(int Id,
                           string Username,
                           string Email,
                           DateTime Created,
                           DateTime? Updated);