using Microsoft.AspNetCore.Mvc;
using VisitEmAll.Models;

namespace VisitEmAll.Controllers;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        // TEMPORARY: Stubbed login until your teammate creates the User table
        if (model.Email == "test@example.com" && model.Password == "password123")
        {
            HttpContext.Session.SetInt32("UserId", 1);
            return RedirectToAction("Index", "Home");
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
