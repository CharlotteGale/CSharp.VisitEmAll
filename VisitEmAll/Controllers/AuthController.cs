using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;
using VisitEmAll.Services;

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
        // || !_hasher.Verify(model.Password, user.Password))
        {
            model.ErrorMessage = "Invalid email or password";
            return View(model);
        }

        HttpContext.Session.SetInt32("User_Id", user.Id);
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
