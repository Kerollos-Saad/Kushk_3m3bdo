using Microsoft.AspNetCore.Identity;

namespace Kushk_3m3bdo.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        public byte[] ProfilePicture { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
