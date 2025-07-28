namespace LeaveManagement.Models.DTO
{
    public class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; }          
        public int LeaveRequestId { get; set; }     
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
