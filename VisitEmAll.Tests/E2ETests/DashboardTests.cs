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

        await Page.FillAsync("input[name='Name']", "PW User");
        await Page.FillAsync("input[name='Email']", uniqueEmail);
        await Page.FillAsync("input[name='Password']", "Password1!");
        await Page.FillAsync("input[name='ConfirmPassword']", "Password1!");

        await Page.ClickAsync("button[type='submit']");

        await Page.WaitForURLAsync("**/Auth/Login");

        await Page.FillAsync("input[name='Email']", uniqueEmail);
        await Page.FillAsync("input[name='Password']", "Password1!");
        await Page.ClickAsync("button[type='submit']");

        await Page.WaitForURLAsync("**/dashboard");

        await Expect(Page.Locator("text=Add Holiday")).ToBeVisibleAsync();
    }
}