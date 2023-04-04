using SSR.WebAPI.Models;

namespace SSR.WebAPI.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public List<Role> Role { get; set; }
        public string newPass { get; set; }
        public string confirmPass { get; set; }
    }
}