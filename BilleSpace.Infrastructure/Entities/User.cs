using Microsoft.AspNetCore.Identity;

namespace BilleSpace.Infrastructure.Entities
{
    public class User : IdentityUser
    {
        public bool IsReceptionist { get; set; }
    }
}
