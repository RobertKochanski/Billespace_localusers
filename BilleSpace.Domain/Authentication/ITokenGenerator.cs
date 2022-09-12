using BilleSpace.Infrastructure.Entities;

namespace BilleSpace.Domain.Authentication
{
    public interface ITokenGenerator
    {
        string CreateToken(User user);
    }
}
