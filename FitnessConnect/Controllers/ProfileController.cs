using FitnessConnect.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace FitnessConnect.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHostingEnvironment _environment;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHostingEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(UserId);
                var avatar = user.FirstName[0] + "" + user.LastName[0];
                ViewBag.User = user;
                ViewBag.Avatar = avatar;
                return View();
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string data)
        {
            try
            {
                var pwdDetails = data.Split(",");
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, pwdDetails[0], false);

                if (signInResult.Succeeded)
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, pwdDetails[0], pwdDetails[1]);

                    return Ok(changePasswordResult.Succeeded ?
                        // Password changed successfully
                        "Success" :
                        // Failed to change password
                        "Failed to change password");
                }
                else
                {
                    // Incorrect current password
                    return Ok("Incorrect current password");
                }
            }
            catch (Exception ex)
            {
                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Handle the exception and return an error response
                return StatusCode(500, "Incorrect Corrent Password");
            }
        }
        [HttpPost]
        public async Task<IActionResult> changeProfile(IFormFile myfile)
        {
            try
            {
                // Generate a unique file name to avoid conflicts
                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + myfile.FileName;
                string uploadFolder = Path.Combine(_environment.WebRootPath, "ProfileImg");
                string FileName = "Profile_" + myfile.FileName;

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(uploadFolder);
                // Combine the upload folder path with the unique file name

                string filePath = Path.Combine(uploadFolder, FileName);
                //Delete File Before Update New Pic
                var user = await _userManager.FindByIdAsync(UserId);
                if (user.ProfileImg != null)
                {
                    string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profileImg");
                    string imagePath = Path.Combine(rootPath, user.ProfileImg);
                    System.IO.File.Delete(imagePath);
                }
                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await myfile.CopyToAsync(stream);
                    user.ProfileImg = FileName;
                    await _userManager.UpdateAsync(user);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return View(ex);
            }
        }
    }
}
