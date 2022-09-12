using Microsoft.AspNetCore.Identity;

namespace BilleSpace.Infrastructure.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsReceptionist { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}
