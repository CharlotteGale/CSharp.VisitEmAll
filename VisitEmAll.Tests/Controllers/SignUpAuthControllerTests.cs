namespace VisitEmAll.Tests.Controllers;

public class AuthControllerTests : NUnitTestBase
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
    public void SignUp_Get_ReturnsView()
    {
        var result = _controller.SignUp() as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ViewName, Is.Null); 
    }

    [Test]
    public async Task SignUp_Post_ValidModel_SavesUserAndRedirects()
    {
        var user = new User
        {
            Name = "Faisal",
            Email = "faisal@email.com",
            Password = "Password1!",
            HomeTown = "London"
        };

        var result = await _controller.SignUp(user) as RedirectToActionResult;

        Assert.That(result?.ActionName, Is.EqualTo("Login"));

        var savedUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == "faisal@email.com");

        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser.Password, Is.Not.EqualTo("Password1!")); // hashed
    }

    [Test]
    public async Task SignUp_Post_DuplicateEmail_ReturnsViewWithError()
    {
        var existingUser = new User
        {
            Name = "Existing",
            Email = "test@email.com",
            Password = "hashedpassword"
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var newUser = new User
        {
            Name = "New",
            Email = "test@email.com",
            Password = "Password1!"
        };

        var result = await _controller.SignUp(newUser) as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(_controller.ModelState.ContainsKey("Email"), Is.True);

        var usersWithEmail = _context.Users
            .Where(u => u.Email == "test@email.com")
            .ToList();

        Assert.That(usersWithEmail.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task SignUp_Post_InvalidModel_DoesNotInsertUser()
    {
        var before = _context.Users.Count();

        var user = new User
        {
            Name = "",
            Email = "bademail",
            Password = "short"
        };

        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.SignUp(user) as ViewResult;

        var after = _context.Users.Count();

        Assert.That(result, Is.Not.Null);
        Assert.That(after, Is.EqualTo(before)); 
    }
}