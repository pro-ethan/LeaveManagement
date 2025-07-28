using LeaveManagement.AuthImplementation.Abstract;
using LeaveManagement.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LeaveManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserAuthService _sevice;
        public AccountController(IUserAuthService sevice)
        {
            _sevice = sevice;
        }

        //get
        public IActionResult Login()
        {
            return View();
        }

        //login handler
        //post 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) { 
                return View(model);
            }

            var result = await _sevice.LoginAsync(model);
            if(result.statusCode == 404)
            {
                TempData["err"] = result.message;
                return RedirectToAction("Index", "Home");
            }
            else if(result.statusCode == 401)
            {
                TempData["err"] = result.message;
                return RedirectToAction("Index", "Home");
            }
            else if(result.statusCode == 500)
            {
                TempData["err"] = result.message;
                return RedirectToAction("Index", "Home");
            }

            //success
            TempData["succ"] = result.message;

            return RedirectToAction("Index","Home"); // work left here
        }


        [Authorize]
        // logout handler
        public async Task<IActionResult> Logout()
        {
            await _sevice.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        //get
        public IActionResult Signin()
        {
            return View();
        }
        // post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signin(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // assigns role
            model.role = "user";
            var result = await _sevice.RegisterAsync(model);
            if(result.statusCode == 409)
            {
                TempData["err"] = result.message;
                return RedirectToAction(nameof(Signin));
            }else if(result.statusCode == 500)
            {
                TempData["err"] = result.message;
                return RedirectToAction(nameof(Signin));
            }
           

            //success
            TempData["succ"] = result.message;

            return RedirectToAction("Index", "Home"); // work left here
        }

        // admin add
        //public async Task<IActionResult> Adminadd()
        //{
        //    var model = new RegisterViewModel
        //    {
        //        Email = "admin@gmail.com",
        //        FullName = "admin",
        //        Password = "Admin@123",
        //    };
        //    model.role = "approver";
        //    var result = await _sevice.RegisterAsync(model);
        //    return Ok(result);
        //}
    }
}
