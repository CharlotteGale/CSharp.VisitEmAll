using VisitEmAll.Models;

public static class DbSeeder
{
    public static void Seed(VisitEmAllDbContext context)
    {
        // Clear tables
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();


        // Add Seed data
        // === USERS === \\
        context.Users.AddRange(
            new User { Name = "Alice", Email = "alice@email.com", Password = "Password1!", HomeTown = "Manchester, UK" },
            new User { Name = "Brian", Email = "brian@email.com", Password = "Password1!", HomeTown = "Dublin, ROI"},
            new User { Name = "Charlie", Email = "charlie@email.com", Password = "Password1!", HomeTown = null},
            new User { Name = "Dave", Email = "dave@email.com", Password = "Password1!", HomeTown = "Miami, FL"},
            new User { Name = "Emily", Email = "emily@email.com", Password = "Password1!", HomeTown = "Paris, France"},
            new User { Name = "Frank", Email = "frank@email.com", Password = "Password1!", HomeTown = null},
            new User { Name = "Grace", Email = "grace@email.com", Password = "Password1!", HomeTown = "Spain"}
        );
        context.SaveChanges();
    }
}