using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;
using VisitEmAll.Services;
using VisitEmAll.ViewModels;

namespace VisitEmAll.Controllers;

public class AuthController : Controller
{
    private readonly VisitEmAllDbContext _context;
    private readonly PasswordHasher _hasher;

    public AuthController(VisitEmAllDbContext context)
    {
        _context = context;
        _hasher = new PasswordHasher();
    }

    [HttpGet]
    public IActionResult SignUp()
    {
        if(HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new SignUpViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(User model)
    {
        if (!ModelState.IsValid) return View(model);

        var emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
        if (emailExists)
        {
            ModelState.AddModelError("Email", "An account with this email already exists. Please log in to continue.");
            return View(model);
        }

        var newUser = new User
        {
            Name = model.Name,
            Email = model.Email,
            Password = _hasher.Hash(model.Password),
            HomeTown = model.HomeTown,
            ProfileImg = model.ProfileImg
        };
        
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Account created successfully! Please log in.";
        return RedirectToAction("Login");
    }
    

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
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

        if (user == null || !_hasher.Verify(model.Password, user.Password))
        {
            model.ErrorMessage = "Invalid email or password";
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
