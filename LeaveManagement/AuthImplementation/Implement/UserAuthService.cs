using LeaveManagement.AuthImplementation.Abstract;
using LeaveManagement.Models.Domain;
using LeaveManagement.Models.ViewModel;
using Microsoft.AspNetCore.Identity;

namespace LeaveManagement.AuthImplementation.Implement
{
    public class UserAuthService : IUserAuthService
    {

        // All provided by default by the asp .net core
        //this handles the signin , sign out--->
        private readonly SignInManager<ApplicationUser> _signInManager;
        // this handles and stores data about the current user ---->
        private readonly UserManager<ApplicationUser> _userManager;
        // this deals with the roles ----->
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserAuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        //to handle user login
        // i need to add claims here , when i will use JWT or add more properties(info) to the cookie
        public async Task<Status> LoginAsync(LoginViewModel model)
        {
            var status = new Status();
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user == null)
            {
                status.statusCode = 404;
                status.message = "User does not exist!";
                return status;
            }

            // match password
            if(!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                status.statusCode = 401;
                status.message = "Please enter correct password!";
                return status;
            }

            // add the user in signinmanager
            var signin = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!signin.Succeeded)
            {
                status.statusCode= 500;
                status.message = "Unable to login. Plz try again";
                return status;
            }

            status.statusCode = 200;
            status.message = "Logged in successfully!";
            return status;
        }

        //to handle the user logout

        public async Task LogoutAsync()
        {
            //var status = new Status();
            await _signInManager.SignOutAsync();
            //status.statusCode = 1;
            //status.message = "Logged Out Successfully";
            //return status;
        }

        // to handle the user registration
        public async Task<Status> RegisterAsync(RegisterViewModel model)
        {
            var status = new Status();
            var if_user_exist = await _userManager.FindByNameAsync(model.FullName);

            if(if_user_exist != null)
            {
                status.statusCode = 409;
                status.message = "User already Exists!";
                return status;
            }

            //else create new user
            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                FullName = model.FullName,
                Email = model.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.statusCode = 500;
                status.message = "Failed to register User";
                return status;
            }

            // check if role is present
            if(!await _roleManager.RoleExistsAsync(model.role))
                await _roleManager.CreateAsync(new IdentityRole(model.role));

            // add role to the user
            await _userManager.AddToRoleAsync(user, model.role);

            status.statusCode = 200;
            status.message = "User has been registered successfully";

            return status;

        }
    }
}
