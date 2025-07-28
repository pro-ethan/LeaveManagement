using LeaveManagement.Models.DTO;

namespace LeaveManagement.Models.ViewModel
{
    public class LeaveWithFilesViewModel
    {
        public LeaveRequest Leave { get; set; }
        public UserFile File { get; set; }
    }
}
