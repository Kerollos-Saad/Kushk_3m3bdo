using System.ComponentModel.DataAnnotations;

namespace Kushl_3m3bdo.Data.ViewModels
{
	public class RoleFormViewModel
	{
		[Required, StringLength(255)]
		public string Name { get; set; }
	}
}
