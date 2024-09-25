using System.ComponentModel.DataAnnotations;

namespace Kushl_3m3bdo.Models.ViewModels
{
    public class RoleFormViewModel
    {
        [Required, StringLength(255)]
        public string Name { get; set; }
    }
}
