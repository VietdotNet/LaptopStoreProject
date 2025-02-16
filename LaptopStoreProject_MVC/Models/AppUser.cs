using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaptopStoreProject_MVC.Models
{
    public class AppUser : IdentityUser
    {
        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string? Address { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? Dob { get; set; }
    }
}
