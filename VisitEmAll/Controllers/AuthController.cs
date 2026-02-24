using Microsoft.AspNetCore.Mvc;
using VisitEmAll.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace VisitEmAll.Controllers;

public class AuthController : Controller
{
    private readonly VisitEmAllDbContext _context;

    public AuthController(VisitEmAllDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null || !VerifyPassword(model.Password, user.Password))
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

    // --- Helper ---
    private bool VerifyPassword(string plain, string hashed)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plain));
        var hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        return hash == hashed;
    }
}
