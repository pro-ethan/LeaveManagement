using LeaveManagement.Models.DTO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Models.Domain
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) :base(options)
        {
            
        }

        public DbSet<LeaveRequest>leaveRequests { get; set; }
        public DbSet<UserFile> userFiles { get; set; }
    }


}
