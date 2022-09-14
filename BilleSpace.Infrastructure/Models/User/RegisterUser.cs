namespace BilleSpace.Infrastructure.Models.User
{
    public class RegisterUser
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsReceptionist { get; set; }
    }
}
