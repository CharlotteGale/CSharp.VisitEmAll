using VisitEmAll.Services;

public class TestPasswordHashing
{
    [Test]
    public void Verify_ReturnsTrue_ForCorrectPassword()
    {
        var hasher = new PasswordHasher();

        var plain = "password123";
        var hashed = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f";

        Assert.True(hasher.Verify(plain, hashed));
    }

    [Test]
    public void Verify_ReturnsFalse_ForIncorrectPassword()
    {
        var hasher = new PasswordHasher();

        var hashed = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f";

        Assert.False(hasher.Verify("wrongpassword", hashed));
    }
}
