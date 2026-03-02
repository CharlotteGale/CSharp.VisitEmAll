
namespace VisitEmAll.Tests.Controllers;

public class AuthControllerTests : NUnitTestBase
{
    private AuthController _controller;
    private readonly IWebHostEnvironment hostEnvironment;

    [SetUp]
    public void LocalSetUp()
    {
        _controller = new AuthController(_context, hostEnvironment);

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
        var user = new SignUpViewModel
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

        var newUser = new SignUpViewModel
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

        var user = new SignUpViewModel
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
        var hasher = new PasswordHasher<User>();

        var user = new User
        {
            Name = "Test",
            Email = "test@email.com",
            Password = ""
        };
        string hashedPassword = hasher.HashPassword(user, "Password1!");
        user.Password = hashedPassword;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var model = new LoginViewModel
        {
            Email = "test@email.com",
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
        var hasher = new PasswordHasher<User>();

        var user = new User
        {
            Name = "Test 2",
            Email = "test2@email.com",
            Password = ""
        };
        string hashedPassword = hasher.HashPassword(user, "Password1!");
        user.Password = hashedPassword;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var model = new LoginViewModel
        {
            Email = "test2@email.com",
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