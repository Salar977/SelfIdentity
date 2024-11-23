namespace SelfIdentity.DTOs;

public record UserRequest(string Username,
                          string Email,
                          string Password);