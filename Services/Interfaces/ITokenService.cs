using SelfIdentity.Entities;

namespace SelfIdentity.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}