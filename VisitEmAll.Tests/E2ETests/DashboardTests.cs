using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace VisitEmAll.PlaywrightTests.Tests;

public class DashboardTests : PageTest
{
    private const string BaseUrl = "http://localhost:5247"; // change if needed

    [Test]
    public async Task Unauthenticated_User_IsRedirected()
    {
        await Page.GotoAsync($"{BaseUrl}/dashboard");
        await Page.WaitForURLAsync("**/Auth/Login");
    }

    [Test]
    public async Task Can_Register_Login_And_View_Dashboard()
    {
        var uniqueEmail = $"test{Guid.NewGuid()}@email.com";

        await Page.GotoAsync($"{BaseUrl}/Auth/SignUp");

        await Page.FillAsync("#Name", "PW User");
        await Page.FillAsync("#Email", uniqueEmail);
        await Page.FillAsync("#Password", "Password1!");
        await Page.FillAsync("#ConfirmPassword", "Password1!");

        await Page.ClickAsync("#signup-submit");

        await Page.WaitForURLAsync(new Regex("Auth/Login/?", RegexOptions.IgnoreCase));

        await Page.FillAsync("#email", uniqueEmail);
        await Page.FillAsync("#password", "Password1!");
        await Page.ClickAsync(".auth-submit-btn");

        await Page.WaitForURLAsync("**/dashboard");

        await Expect(Page.Locator("text=Add Holiday")).ToBeVisibleAsync();
    }
}