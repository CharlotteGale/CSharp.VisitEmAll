using VisitEmAll.Models;

public static class DbSeeder
{
    public static void Seed(VisitEmAllDbContext context)
    {
        // Clear tables
        context.Users.RemoveRange(context.Users);
        context.Holidays.RemoveRange(context.Holidays);
        context.Activities.RemoveRange(context.Activities);
        
        context.SaveChanges();


        // Add Seed data
        // === USERS === \\
        var users = new List<User>
        {
            new User
            {
                Name = "Alice",
                Email = "alice@email.com",
                Password = "Password1!",
                HomeTown = "Manchester, UK"
            },
            new User
            {
                Name = "Brian",
                Email = "brian@email.com",
                Password = "Password1!",
                HomeTown = "Dublin, ROI"
            },
            new User
            {
                Name = "Charlie",
                Email = "charlie@email.com",
                Password = "Password1!",
                HomeTown = null
            },
            new User
            {
                Name = "Dave",
                Email = "dave@email.com",
                Password = "Password1!",
                HomeTown = "Miami, FL"
            },
            new User
            {
                Name = "Emily",
                Email = "emily@email.com",
                Password = "Password1!",
                HomeTown = "Paris, France"
            },
            new User
            {
                Name = "Frank",
                Email = "frank@email.com",
                Password = "Password1!",
                HomeTown = null
            },
            new User
            {
                Name = "Grace",
                Email = "grace@email.com",
                Password = "Password1!",
                HomeTown = "Spain"
            }
        };
        context.Users.AddRange(users);
        context.SaveChanges();



        // === HOLIDAYS === \\
        var holidays = new List<Holiday>
        {
            // Alice's Trip (users[0])
            new Holiday {
                UserId = users[0].Id,
                Title = "Summer in Santorini",
                Location = "Greece",
                StartDate = new DateOnly(2024, 7, 10),
                EndDate = new DateOnly(2024, 7, 24),
                Accommodation = "Blue Dome Suites",
                Cost =  1200.50m },
            
            // Brian's Trip (users[1])
            new Holiday {
                UserId = users[1].Id,
                Title = "Skiing in the Alps",
                Location = "Chamonix, France",
                StartDate = new DateOnly(2024, 12, 15),
                EndDate = new DateOnly(2024, 12, 22),
                Accommodation = "Alpine Lodge", Cost = 850.00m },
            
            // Dave's Trip (users[3])
            new Holiday
            {
                UserId = users[3].Id,
                Title = "Tokyo Adventure",
                Location = "Japan",
                StartDate = new DateOnly(2025, 3, 5),
                EndDate = new DateOnly(2025, 3, 30),
                Accommodation = "Shinjuki Park Hotel",
                Cost =  3100.00m
            },
            
            // Emily's Trip (users[4])
            new Holiday
            {
                UserId = users[4].Id,
                Title = "Weekend in Rome",
                Location = "Rome, Italy",
                StartDate = new DateOnly(2024, 5, 12),
                EndDate = new DateOnly(2024, 5, 15),
                Accommodation = "AirBnB near Colosseum",
                Cost = 450.00m
            },
            
            // Grace's Trip (users[6])
            new Holiday
            {
                UserId = users[6].Id,
                Title = "Hiking in the Highlands",
                Location = "Scotland",
                StartDate = new DateOnly(2024, 8, 1),
                EndDate = new DateOnly(2024, 8, 7),
                Accommodation = "Campsites",
                Cost = 560.00m
            }
        };
        context.Holidays.AddRange(holidays);
        context.SaveChanges();



        // === ACTIVITIES === \\
        var activities = new List<Activity>
        {
            // Activities for Alice's Trip (holidays[0] - Santorini)
            new Activity { HolidayId = holidays[0].Id, Name = "Sunset Dinner in Oia" },
            new Activity { HolidayId = holidays[0].Id, Name = "Catamaran Sailing Tour" },
            new Activity { HolidayId = holidays[0].Id, Name = "Wine Tasting at Venetsanos" },

            // Activities for Brian's Trip (holidays[1] - Alps)
            new Activity { HolidayId = holidays[1].Id, Name = "Full Day Ski Pass - Les Houches" },
            new Activity { HolidayId = holidays[1].Id, Name = "Aiguille du Midi Cable Car" },
            new Activity { HolidayId = holidays[1].Id, Name = "Après-ski at La Folie Douce" },

            // Activities for Dave's Trip (holidays[2] - Tokyo)
            new Activity { HolidayId = holidays[2].Id, Name = "Robot Cafe Experience" },
            new Activity { HolidayId = holidays[2].Id, Name = "Tsukiji Outer Market Breakfast" },
            new Activity { HolidayId = holidays[2].Id, Name = "Shibuya Crossing Photo Op" },
            new Activity { HolidayId = holidays[2].Id, Name = "Day Trip to Mt. Fuji" },

            // Activities for Emily's Trip (holidays[3] - Rome)
            new Activity { HolidayId = holidays[3].Id, Name = "Colosseum Underground Tour" },
            new Activity { HolidayId = holidays[3].Id, Name = "Pasta Making Class" }
        };
        context.Activities.AddRange(activities);
        context.SaveChanges();

        context.SaveChanges();
    }
}