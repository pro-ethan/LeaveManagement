using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Models.DTO
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        public string UserId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string LeaveType { get; set; }  // Sick, Casual, etc.

        public string Reason { get; set; }

        public string Status { get; set; } = "Pending";  // Pending, Approved, Rejected

        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
