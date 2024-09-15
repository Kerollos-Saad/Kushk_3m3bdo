using Microsoft.AspNetCore.Identity;

namespace Kushl_3m3bdo.Data.ViewModels
{
	public class UserViewModel
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		
		public byte[] ProfilePicture { get; set; }

		public IEnumerable<String> Roles { get; set; }
	}
}
