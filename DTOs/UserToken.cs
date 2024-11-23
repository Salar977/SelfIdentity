namespace SelfIdentity.DTOs;

public record UserToken(string Username,
                        string Email,
                        string Token);