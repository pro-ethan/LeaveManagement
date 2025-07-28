using LeaveManagement.Models.Domain;
using LeaveManagement.Models.DTO;
using LeaveManagement.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHost;

        public LeaveRequestController(AppDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHost)
        {
            _context = context;
            _userManager = userManager;
            _webHost = webHost;
        }

        //get all the statuses
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllStatuses()
        {
            var user = await _userManager.GetUserAsync(User);
            var statuses = await _context.leaveRequests.Where(l => l.UserId == user.Id)
                .Select(l => new { l.Id, l.Status })
                .ToListAsync();

            return Json(statuses);
        }


        [Authorize(Roles = "user")]
        // all the leaves (pending , approved, rejected) for a particular user (get)
        public async Task<IActionResult> GetAllLeaves(string? sort)
        {
            var user = await _userManager.GetUserAsync(User);
            var allLeavesQuery =  _context.leaveRequests.Where(l => l.UserId == user.Id);
            
            allLeavesQuery = sort switch
            {
                "LeaveTypeAsc" => allLeavesQuery.OrderBy(l => l.LeaveType),
                "LeaveTypeDesc" => allLeavesQuery.OrderByDescending(l => l.LeaveType),
                "statusAsc" => allLeavesQuery.OrderBy(l => l.Status),
                "statusDesc" => allLeavesQuery.OrderByDescending(l => l.Status),
                "startDateAsc" => allLeavesQuery.OrderBy(l => l.StartDate),
                "startDateDesc" => allLeavesQuery.OrderByDescending(l => l.StartDate),
                "Sick" => allLeavesQuery.Where(l => l.LeaveType == "Sick"),
                "Casual" => allLeavesQuery.Where(l => l.LeaveType == "Casual"),
                "Emergency" => allLeavesQuery.Where(l => l.LeaveType == "Emergency"),
                _ => allLeavesQuery
            };

            var finalLeaves = await allLeavesQuery.ToListAsync();

            var files = await _context.userFiles.Where(f => f.UserId == user.Id).ToListAsync();

            var result = finalLeaves.Select(leave => new LeaveWithFilesViewModel
            {
                Leave = leave,
                File = files.FirstOrDefault(f => f.LeaveRequestId == leave.Id),
            }).ToList();

            return View(result);

        }

        [Authorize]
        // leave form (get)
        public IActionResult Leave()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // leave form (post)
        public async Task<IActionResult> Leave(LeaveRequest model)
        {
            if(model.StartDate > model.EndDate)
            {
                ModelState.AddModelError("DateError", "Start date cannot be greater then end date");
            }
            var user = await _userManager.GetUserAsync(User);
            model.UserId = user.Id;
            ModelState.Remove(nameof(model.UserId));

            if (ModelState.IsValid)
            {
                
                model.Status = "Pending";
                model.CreatedAt = DateTime.Now;

                _context.leaveRequests.Add(model);
                await _context.SaveChangesAsync();

                TempData["succ"] = "Leave Reqest added successfully";
                return RedirectToAction(nameof(UploadAttachment), new { id= model.Id});

            }
            
            TempData["err"] = "Please fill the details correctly!";
            return View(model);
        }

        [Authorize]
        public IActionResult UploadAttachment(int id)
        {
            return View(id);
        }


        //File upload
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadAttachment(IFormFile file, int LeaveRequestId, string? fileupload)
        {
            var user = await _userManager.GetUserAsync(User);

            if (file == null || file.Length == 0)
                TempData["err"] = "File not selected";
            else
            {
                //File upload
                string folder = Path.Combine(_webHost.WebRootPath, "uploads");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Path.GetFileName(file.FileName);
                string fileSavePath = Path.Combine(folder, fileName);

                // reading and saving the file
                using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
                    await file.CopyToAsync(stream);
                TempData["succ"] = "File uploaded scuccessfully";

                var attachment = new UserFile
                {
                    UserId = user.Id,
                    LeaveRequestId = LeaveRequestId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/{file.FileName}",
                    UploadedAt = DateTime.Now,
                };
                _context.userFiles.Add(attachment);
                await _context.SaveChangesAsync();


            }
            return RedirectToAction(nameof(GetAllLeaves));
        }



        [Authorize(Roles = "approver")]
        // approver page to (approved, rejected) for all the users
        //get
        public IActionResult ApproverView(string? sort)
        {
            var leaves = _context.leaveRequests.Where(l => l.Status == "Pending");
            //if(sort == "startDateAsc")
            //{
            //    leaves = leaves.OrderBy(l => l.StartDate);
            //}else if(sort == "startDateDesc")
            //{
            //    leaves = leaves.OrderByDescending(l => l.StartDate);
            //}else if (sort == "LeaveAsc")
            //{
            //    leaves = leaves.OrderBy(l => l.LeaveType);
            //}
            //else if (sort == "LeaveDesc")
            //{
            //    leaves = leaves.OrderByDescending(l => l.LeaveType);
            //}

            leaves = sort switch
            {
                "startDateAsc" => leaves.OrderBy(l => l.StartDate),
                "startDateDesc" => leaves.OrderByDescending(l => l.StartDate),
                "LeaveAsc" => leaves.OrderBy(l => l.LeaveType),
                "LeaveDesc" => leaves.OrderByDescending(l => l.LeaveType),
                _ => leaves,
            };

            return View(leaves.ToList());
        }


        [Authorize(Roles = "approver")]
        [HttpPost]
        //update
        public async Task<IActionResult> Approve(int? id)
        {
            if(id== null || id == 0)
            {
                TempData["err"] = "Please provide a valid id!";
                return View();
            }

            var leave = await _context.leaveRequests.FirstOrDefaultAsync(l => l.Id == id);
            if(leave != null)
            {
                leave.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["succ"] = "Approved Successfully";
            }
            return RedirectToAction(nameof(ApproverView));
        }

        [Authorize(Roles = "approver")]
        [HttpPost]
        //update
        public async Task<IActionResult> Reject(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["err"] = "Please provide a valid id!";
                return View();
            }

            var leave = await _context.leaveRequests.FirstOrDefaultAsync(l => l.Id == id);
            if (leave != null)
            {
                leave.Status = "Rejected";
                await _context.SaveChangesAsync();
                TempData["succ"] = "Rejected Successfully";
            }
            return RedirectToAction(nameof(ApproverView));
        }

    }
}
