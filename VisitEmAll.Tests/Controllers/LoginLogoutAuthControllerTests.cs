using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Moq;
using VisitEmAll.Controllers;
using VisitEmAll.Models;
using VisitEmAll.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace VisitEmAll.Tests.Controllers;

[TestFixture]
public class LoginLogoutAuthControllerTests : NUnitTestBase
{
    private AuthController _controller;

    [SetUp]
    public void LocalSetUp()
    {
        _controller = new AuthController(_context);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var tempData = new TempDataDictionary(
            _controller.HttpContext,
            Mock.Of<ITempDataProvider>());

        _controller.TempData = tempData;
    }

    [TearDown]
    public void LocalTearDown()
    {
        _controller?.Dispose();
    }

    [Test]
    public void Login_Get_ReturnsView_WhenNotLoggedIn()
    {
        var result = _controller.Login() as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ViewName, Is.Null);
    }

    [Test]
    public void Login_Get_RedirectsToDashboard_WhenAlreadyLoggedIn()
    {
        _controller.HttpContext.Session.SetInt32("User_Id", 1);

        var result = _controller.Login() as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Index"));
        Assert.That(result.ControllerName, Is.EqualTo("Home"));
    }

    [Test]
    public async Task Login_Post_ValidCredentials_SetsSessionAndRedirects()
    {
        var hasher = new VisitEmAll.Services.PasswordHasher();

        var user = new User
        {
            Name = "Sam",
            Email = "sam@email.com",
            Password = hasher.Hash("Password1!")
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var model = new LoginViewModel
        {
            Email = "sam@email.com",
            Password = "Password1!"
        };

        var result = await _controller.Login(model) as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Index"));
        Assert.That(result.ControllerName, Is.EqualTo("Dashboard"));

        var sessionUserId = _controller.HttpContext.Session.GetInt32("User_Id");
        Assert.That(sessionUserId, Is.Not.Null);
    }

    [Test]
    public async Task Login_Post_UserDoesNotExist_ReturnsViewWithError()
    {
        var model = new LoginViewModel
        {
            Email = "missing@example.com",
            Password = "password"
        };

        var result = await _controller.Login(model) as ViewResult;

        Assert.That(result, Is.Not.Null);

        var returnedModel = result.Model as LoginViewModel;
        Assert.That(returnedModel?.ErrorMessage, Is.EqualTo("Invalid email or password"));
    }

    [Test]
    public async Task Login_Post_WrongPassword_ReturnsViewWithError()
    {
        var hasher = new VisitEmAll.Services.PasswordHasher();

        var user = new User
        {
            Name = "Sam",
            Email = "sam@email.com",
            Password = hasher.Hash("CorrectPassword1!")
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var model = new LoginViewModel
        {
            Email = "sam@email.com",
            Password = "WrongPassword1!"
        };

        var result = await _controller.Login(model) as ViewResult;

        Assert.That(result, Is.Not.Null);

        var returnedModel = result.Model as LoginViewModel;
        Assert.That(returnedModel?.ErrorMessage, Is.EqualTo("Invalid email or password"));
    }

    [Test]
    public async Task Login_Post_InvalidModel_ReturnsViewWithError()
    {
        _controller.ModelState.AddModelError("Email", "Required");

        var model = new LoginViewModel
        {
            Email = "",
            Password = ""
        };

        var result = await _controller.Login(model) as ViewResult;

        Assert.That(result, Is.Not.Null);

        var returnedModel = result.Model as LoginViewModel;
        Assert.That(returnedModel?.ErrorMessage, Is.EqualTo("Please fill in all fields correctly"));
    }

    [Test]
    public async Task Login_Post_DoesNotSetSession_WhenCredentialsInvalid()
    {
        var model = new LoginViewModel
        {
            Email = "nobody@email.com",
            Password = "irrelevant"
        };

        await _controller.Login(model);

        var sessionUserId = _controller.HttpContext.Session.GetInt32("User_Id");
        Assert.That(sessionUserId, Is.Null);
    }

    [Test]
    public void Logout_ClearsSessionAndRedirectsToLogin()
    {
        _controller.HttpContext.Session.SetInt32("User_Id", 42);

        var result = _controller.Logout() as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Login"));

        var sessionUserId = _controller.HttpContext.Session.GetInt32("User_Id");
        Assert.That(sessionUserId, Is.Null);
    }

    [Test]
    public void Logout_WhenNotLoggedIn_StillRedirectsToLogin()
    {
        var result = _controller.Logout() as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Login"));
    }
}