using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Models.Domain
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string FullName { get; set; }
    }
}
