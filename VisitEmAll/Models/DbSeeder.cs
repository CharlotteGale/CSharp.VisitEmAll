using VisitEmAll.Models;

public static class DbSeeder
{
  public static void Seed(VisitEmAllDbContext context)
  {
    // Clear tables
    context.DayItems.RemoveRange(context.DayItems);
    context.HolidayDays.RemoveRange(context.HolidayDays);
    context.Holidays.RemoveRange(context.Holidays);
    context.Users.RemoveRange(context.Users);

    context.SaveChanges();

    // Add Seed data
    // === USERS === \\
    var users = new List<User>
    {
      new User{Name = "Alice", Email = "alice@email.com", Password = "Password1!", HomeTown = "Manchester, UK"},
      new User{Name = "Brian", Email = "brian@email.com", Password = "Password1!", HomeTown = "Dublin, ROI"},
      new User{Name = "Charlie", Email = "charlie@email.com", Password = "Password1!", HomeTown = null},
      new User{Name = "Dave", Email = "dave@email.com", Password = "Password1!", HomeTown = "Miami, FL"},
      new User{Name = "Emily", Email = "emily@email.com", Password = "Password1!", HomeTown = "Paris, France"},
      new User{Name = "Frank", Email = "frank@email.com", Password = "Password1!", HomeTown = null},
      new User{Name = "Grace", Email = "grace@email.com", Password = "Password1!", HomeTown = "Spain"}
    };
    context.Users.AddRange(users);
    context.SaveChanges();

    // === HOLIDAYS === \\
    var holidays = new List<Holiday>
    {
      new Holiday { UserId = users[0].Id, Title = "Summer in Santorini", Location = "Greece", StartDate = new DateOnly(2024, 7, 10), EndDate = new DateOnly(2024, 7, 11) },
      new Holiday { UserId = users[1].Id, Title = "Skiing in the Alps", Location = "Chamonix, France", StartDate = new DateOnly(2024, 12, 15), EndDate = new DateOnly(2024, 12, 16) },
      new Holiday { UserId = users[3].Id, Title = "Tokyo Adventure", Location = "Japan", StartDate = new DateOnly(2025, 3, 5), EndDate = new DateOnly(2025, 3, 6) },
      new Holiday { UserId = users[4].Id, Title = "Weekend in Rome", Location = "Rome, Italy", StartDate = new DateOnly(2024, 5, 12), EndDate = new DateOnly(2024, 5, 13) },
      new Holiday { UserId = users[6].Id, Title = "Hiking in the Highlands", Location = "Scotland", StartDate = new DateOnly(2024, 8, 1), EndDate = new DateOnly(2024, 8, 2) }
    };
    context.Holidays.AddRange(holidays);
    context.SaveChanges();

    // === HOLIDAY DAYS === \\
    var holidayDays = new List<HolidayDay>();
    foreach (var h in holidays)
    {
      var start = h.StartDate ?? new DateOnly(2024,1,1);
      holidayDays.Add(new HolidayDay { HolidayId = h.Id, Date = start });
      holidayDays.Add(new HolidayDay { HolidayId = h.Id, Date = start.AddDays(1) });
  }
    context.HolidayDays.AddRange(holidayDays);
    context.SaveChanges();

    // === DAY ITEMS (TPH) === \\
    var items = new List<DayItem>
    {
      // --- Santorini (Holidays[0]) ---
      new DayAccommodation { HolidayDayId = holidayDays[0].Id, Name = "Blue Dome Suites", Time = new TimeOnly(14, 0), Location = "Oia" },
      new DayRestaurant { HolidayDayId = holidayDays[0].Id, Name = "Sunset Dinner in Oia", Time = new TimeOnly(19, 30) },
      new DayActivity { HolidayDayId = holidayDays[1].Id, Name = "Catamaran Sailing Tour", Time = new TimeOnly(10, 0), Cost = 150.00m },

      // --- Alps (Holidays[1]) ---
      new DayAccommodation { HolidayDayId = holidayDays[2].Id, Name = "Alpine Lodge", Time = new TimeOnly(15, 0), Location = "Chamonix" },
      new DayActivity { HolidayDayId = holidayDays[2].Id, Name = "Full Day Ski Pass", Time = new TimeOnly(8, 30), Cost = 65.00m },
      new DayRestaurant { HolidayDayId = holidayDays[3].Id, Name = "Après-ski at La Folie Douce", Time = new TimeOnly(16, 0) },

      // --- Tokyo (Holidays[2]) ---
      new DayAccommodation { HolidayDayId = holidayDays[4].Id, Name = "Shinjuku Park Hotel", Time = new TimeOnly(15, 0) },
      new DayRestaurant { HolidayDayId = holidayDays[4].Id, Name = "Tsukiji Outer Market Breakfast", Time = new TimeOnly(7, 30) },
      new DayActivity { HolidayDayId = holidayDays[5].Id, Name = "Robot Cafe Experience", Time = new TimeOnly(18, 0), Cost = 80.00m },

      // --- Rome (Holidays[3]) ---
      new DayAccommodation { HolidayDayId = holidayDays[6].Id, Name = "AirBnB near Colosseum", Time = new TimeOnly(14, 0) },
      new DayActivity { HolidayDayId = holidayDays[6].Id, Name = "Colosseum Underground Tour", Time = new TimeOnly(10, 0), Cost = 50.00m },
      new DayActivity { HolidayDayId = holidayDays[7].Id, Name = "Pasta Making Class", Time = new TimeOnly(17, 0), Cost = 90.00m },

      // --- Scotland (Holidays[4]) ---
      new DayAccommodation { HolidayDayId = holidayDays[8].Id, Name = "Highland Campsite", Time = new TimeOnly(16, 0), Location = "Glencoe" },
      new DayActivity { HolidayDayId = holidayDays[8].Id, Name = "Hiking Ben Nevis", Time = new TimeOnly(8, 0), Notes = "Bring waterproofs!" },
      new DayRestaurant { HolidayDayId = holidayDays[9].Id, Name = "Local Pub Dinner", Time = new TimeOnly(19, 0) }
    };

    context.DayItems.AddRange(items);
    context.SaveChanges();
  }
}