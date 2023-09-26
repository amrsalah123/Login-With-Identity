using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityTask.Models;
using IdentityTask.ViewModel;
using System.IO;
using Microsoft.AspNetCore.Hosting;


namespace IdentityTask.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Register()
        {
            ViewData["mode"] = "create";
            var All_User = _userManager.Users.ToList();
            ViewData["user"] = All_User;
            return View();
        }
     
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel Model)
        {
            
            if (ModelState.IsValid)
            {
                string Filename = "";
                if (Model.file != null)
                {
                    string upload = Path.Combine(_hostEnvironment.WebRootPath, @"ProfileImage");
                    string fullpath = Path.Combine(upload, Model.file.FileName);
                    FileStream fs = new FileStream(fullpath, FileMode.Create);
                    await Model.file.CopyToAsync(fs);
                    Filename = Model.file.FileName;
                    fs.Close();

                }
                var User = new ApplicationUser { Email = Model.Email, ImageUrl = Filename, job = Model.Job, MentorId = Model.MentorId,UserName=Model.Email };
                var Result= await _userManager.CreateAsync(User, Model.Password);
                if (Result.Succeeded)
                {
                   await _signInManager.SignInAsync(User, isPersistent: false);
                    return RedirectToAction(nameof(Profile));
                }
                foreach (var error in Result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            var user = _userManager.Users.ToList();


            ViewData["user"] = user;
            return View(Model);
        }
        
        [HttpGet]
        public IActionResult signin() => View();

        [HttpPost]
        public async Task<IActionResult> signin(LoginViewModel Model)
        {

            if (ModelState.IsValid)
            {
                var Result = await _signInManager.PasswordSignInAsync(Model.Email, Model.PassWord, Model.RememberMe,false);
                if (Result.Succeeded)
                {
                    return RedirectToAction(nameof(Profile));
                }
                ModelState.AddModelError("", "Invalid Email Or PassWord");
            }
            return View(Model);
        }
        
        public async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> Profile()
        {
            var Current_User = await _userManager.GetUserAsync(HttpContext.User);
            var Users_With_Same_Email = _userManager.Users.Where(x => x.job == Current_User.job).ToList();
            ViewBag.all = Users_With_Same_Email;
            return View(Current_User);
        }

        [HttpGet]
        public async Task< IActionResult> Update()
        {
            ViewData["mode"] = "update";
            var Current_User = await _userManager.GetUserAsync(HttpContext.User);
            var All_User = _userManager.Users.ToList();
            ViewData["user"] = All_User;
            TempData["old"] = Current_User.ImageUrl;
            RegisterViewModel b = new RegisterViewModel { MentorId = Current_User.MentorId, Email = Current_User.Email, Job = Current_User.job ,
            
            };
            ViewBag.img = Current_User.ImageUrl;
            return View("Register",b); 
        }

        [HttpPost]
        public async Task<IActionResult> Update(RegisterViewModel Model)
        {
           
            if (ModelState.IsValid)
            {
                string OldFileName = TempData["old"] as string;
                string Filename = OldFileName;
                if (Model.file != null )
                {
                    string upload = Path.Combine(_hostEnvironment.WebRootPath, @"ProfileImage");
                    string fullpath = Path.Combine(upload, Model.file.FileName);
                    FileStream fs = new FileStream(fullpath, FileMode.Create);
                    await Model.file.CopyToAsync(fs);
                    string oldpath = Path.Combine(_hostEnvironment.WebRootPath, @"ProfileImage", OldFileName);
                    Filename = Model.file.FileName;
                    fs.Close();
                    System.IO.File.Delete(oldpath);
                }
                var Current_User = await _userManager.GetUserAsync(HttpContext.User);
                Current_User.Email = Model.Email;
                Current_User.ImageUrl = Filename;
                Current_User.job = Model.Job;
                Current_User.MentorId = Model.MentorId;
               await _userManager.UpdateAsync(Current_User);

                return RedirectToAction(nameof(Profile));
            }
            var user = _userManager.Users.ToList();


            ViewData["user"] = user;
            return View("Register",Model);
        }





    }
}
