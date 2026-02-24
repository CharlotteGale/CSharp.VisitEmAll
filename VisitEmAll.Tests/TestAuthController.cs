using NUnit.Framework;
using VisitEmAll.Controllers;
using VisitEmAll.Models;
using VisitEmAll.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

[TestFixture]
public class TestAuthController
{
    private VisitEmAllDbContext _db;
    private AuthController _controller;

    [SetUp]
    public void Setup()
    {
        // 1. Configure EF Core InMemory database
        var options = new DbContextOptionsBuilder<VisitEmAllDbContext>()
            .UseInMemoryDatabase("AuthTests")
            .Options;

        // 2. Create a fake configuration object
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        // 3. Pass BOTH required constructor arguments
        _db = new VisitEmAllDbContext(options, config);

        // 4. Mock HttpContext for session support
        var httpContext = new DefaultHttpContext();

        _controller = new AuthController(_db)
        {
            ControllerContext = { HttpContext = httpContext }
        };
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_WhenUserDoesNotExist()
    {
        var model = new LoginViewModel
        {
            Email = "missing@example.com",
            Password = "password"
        };

        var result = await _controller.Login(model);

        Assert.IsInstanceOf<ViewResult>(result);

        var view = result as ViewResult;
        var returnedModel = view?.Model as LoginViewModel;

        Assert.That(returnedModel?.ErrorMessage, Is.EqualTo("Invalid email or password"));
    }

}
