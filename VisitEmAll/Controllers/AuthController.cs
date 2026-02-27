using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using VisitEmAll.Models;
using VisitEmAll.ViewModels;

namespace VisitEmAll.Controllers;

public class AuthController : Controller
{
    private readonly VisitEmAllDbContext _context;
    private readonly PasswordHasher<User> _hasher;
    private readonly IWebHostEnvironment webHostEnvironment;

    public AuthController(VisitEmAllDbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hasher = new PasswordHasher<User>();
        webHostEnvironment = hostEnvironment;
    }

    [HttpGet]
    public IActionResult SignUp()
    {
        if(HttpContext.Session.GetInt32("User_Id") != null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new SignUpViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
      if (!ModelState.IsValid) return View(model);

      var emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
      if (emailExists)
      {
        ModelState.AddModelError("Email", "An account with this email already exists.");
        return View(model);
      }

      string uniqueFileName = null;
      if (model.ProfileImg != null)
      {
        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads/profiles");
        
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImg.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          await model.ProfileImg.CopyToAsync(fileStream);
        }
      }
      string hashedPw = _hasher.HashPassword(null!, model.Password);
      var newUser = new User
      {
        Name = model.Name,
        Email = model.Email,
        HomeTown = model.HomeTown,
        ProfileImg = uniqueFileName,
        Password = hashedPw
      };
      
      _context.Users.Add(newUser);
      await _context.SaveChangesAsync();

      TempData["SuccessMessage"] = "Account created successfully! Please log in.";
      return RedirectToAction("Login");
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("User_Id") != null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ErrorMessage = "Please fill in all fields correctly";
            return View(model);
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            model.ErrorMessage = "Invalid email or password";
            return View(model);
        }

        PasswordVerificationResult result;
        bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        bool isPlainTextPassword = user.Password.Length < 20;
        if(isDevelopment && isPlainTextPassword)
        {
            result = user.Password == model.Password
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
        else
        {
            result = _hasher.VerifyHashedPassword(user, user.Password, model.Password);
        }

        if(result != PasswordVerificationResult.Failed)
        {
            if(result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _hasher.HashPassword(user, model.Password);
                await _context.SaveChangesAsync();
            }
            HttpContext.Session.SetInt32("User_Id", user.Id);
            return RedirectToAction("Index", "Dashboard");
        }

            model.ErrorMessage = "Invalid email or password";
            return View(model);

    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
