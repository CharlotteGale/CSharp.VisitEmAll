using VisitEmAll.Models;

public static class DbSeeder
{
    public static void Seed(VisitEmAllDbContext context)
    {
        // Clear tables

        context.SaveChanges();


        // Add Seed data

        context.SaveChanges();
    }
}