using System.Text.Json;
using System.Text.Json.Serialization;

namespace VisitEmAll.Models;

public static class DbSeeder
{
    public static void Seed(VisitEmAllDbContext context)
    {
        // Clear tables (order matters for FK constraints)
        context.Friendships.RemoveRange(context.Friendships);
        context.DayItems.RemoveRange(context.DayItems);
        context.HolidayDays.RemoveRange(context.HolidayDays);
        context.Holidays.RemoveRange(context.Holidays);
        context.Users.RemoveRange(context.Users);
        context.Countries.RemoveRange(context.Countries);
        context.SaveChanges();

        // === USERS ===
        var users = new List<User>
        {
            new() { Name = "Alice",   Email = "alice@email.com",   Password = "Password1!", HomeTown = "Manchester, UK" },
            new() { Name = "Brian",   Email = "brian@email.com",   Password = "Password1!", HomeTown = "Dublin, ROI" },
            new() { Name = "Charlie", Email = "charlie@email.com", Password = "Password1!", HomeTown = null },
            new() { Name = "Dave",    Email = "dave@email.com",    Password = "Password1!", HomeTown = "Miami, FL" },
            new() { Name = "Emily",   Email = "emily@email.com",   Password = "Password1!", HomeTown = "Paris, France" },
            new() { Name = "Frank",   Email = "frank@email.com",   Password = "Password1!", HomeTown = null },
            new() { Name = "Grace",   Email = "grace@email.com",   Password = "Password1!", HomeTown = "Spain" }
        };
        context.Users.AddRange(users);
        context.SaveChanges();

        // === COUNTRIES (full seed from json) ===
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "countries.json");
        if (!File.Exists(jsonPath))
            throw new FileNotFoundException($"countries.json not found at {jsonPath}");

        var json = File.ReadAllText(jsonPath);

        var dict = JsonSerializer.Deserialize<Dictionary<string, CountryRow>>(json)
                   ?? new Dictionary<string, CountryRow>();

        var toAdd = dict.Values
            .Where(x => !string.IsNullOrWhiteSpace(x.CountryName)
                     && !string.IsNullOrWhiteSpace(x.CountryCode2)
                     && !string.IsNullOrWhiteSpace(x.ContinentName))
            .Select(x => new Country
            {
                Name = NormalizeCountryName(x.CountryName.Trim()),
                Iso2 = x.CountryCode2.Trim().ToUpperInvariant(),
                Continent = x.ContinentName.Trim()
            })
            .GroupBy(c => c.Iso2) // safety: avoid duplicates
            .Select(g => g.First())
            .ToList();

        context.Countries.AddRange(toAdd);
        context.SaveChanges();

        // Build ISO2 -> Id lookup (case-insensitive)
        var countryIdByIso2 = context.Countries
            .ToDictionary(c => c.Iso2.ToUpperInvariant(), c => c.Id);

        int? PickCountryId(string? location)
        {
            if (string.IsNullOrWhiteSpace(location)) return null;
            var s = location.ToLowerInvariant();

            if (s.Contains("italy") || s.Contains("milan") || s.Contains("rome")) return countryIdByIso2["IT"];
            if (s.Contains("spain") || s.Contains("barcelona")) return countryIdByIso2["ES"];
            if (s.Contains("france") || s.Contains("chamonix") || s.Contains("paris")) return countryIdByIso2["FR"];
            if (s.Contains("usa") || s.Contains("united states") || s.Contains("new york") || s.Contains("san francisco")) return countryIdByIso2["US"];
            if (s.Contains("japan") || s.Contains("tokyo") || s.Contains("sapporo")) return countryIdByIso2["JP"];
            if (s.Contains("norway") || s.Contains("oslo")) return countryIdByIso2["NO"];
            if (s.Contains("greece") || s.Contains("santorini")) return countryIdByIso2["GR"];
            if (s.Contains("andorra")) return countryIdByIso2["AD"];
            if (s.Contains("scotland") || s.Contains("highlands") || s.Contains("uk") || s.Contains("united kingdom")) return countryIdByIso2["GB"];

            return null;
        }

    // === HOLIDAYS === \\
    var holidays = new List<Holiday>

    { new Holiday { UserId = users[0].Id, Title = "Summer in Milan", Location = "Milan, Italy", StartDate = new DateOnly(2027, 7, 10), EndDate = new DateOnly(2027, 7, 11), HeroImageUrl = "https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEisemxwmpiTl47rBO0_7NP4wV_VCBBKOTqtqnSg5gvxqfQQD42BSg6QbJ8v-P7gJm3jquguXAkLDzmuuJ2qbtR9azz7yvVstgIvt1o3BpPrWFmcppvnVF25OQVwFJZbYLM6j4hYvzNkUSk/w640-h480/ouael-ben-salah-0xe2FGo7Vc0-unsplash.jpg" },
      new Holiday { UserId = users[0].Id, Title = "Autumn in San Francisco", Location = "San Francisco, USA", StartDate = new DateOnly(2027, 9, 10), EndDate = new DateOnly(2027, 9, 15), HeroImageUrl = "https://www.extranomical.com/wp-content/uploads/2023/10/Background-2-min.jpg"},
      new Holiday { UserId = users[0].Id, Title = "Winter in Oslo", Location = "Oslo, Norway", StartDate = new DateOnly(2026, 12, 12), EndDate = new DateOnly(2026, 12, 20), HeroImageUrl = "https://www.visitoslo.com/cdn-cgi/image/width=1400,fit=contain,height=560,quality=75/contentassets/80f6dac873a74838906e8a1db8979380/vinter-i-oslo-roseslottet.jpg" },
      new Holiday { UserId = users[0].Id, Title = "Spring in Barcelona", Location = "Barcelona, Spain", StartDate = new DateOnly(2026, 4, 10), EndDate = new DateOnly(2026, 3, 21), HeroImageUrl = "https://www.keyinnapartments.com/blog/wp-content/uploads/elementor/thumbs/barcelona-en-abril-r2zppi1sw0joadkkbjq45xzn9nkezwmerzj3at9goo.jpg"},
      new Holiday { UserId = users[0].Id, Title = "Stephanie's wedding", Location = "Venice, Italy", StartDate = new DateOnly(2026, 5, 10), EndDate = new DateOnly(2026, 7, 11), HeroImageUrl = "https://ca-times.brightspotcdn.com/dims4/default/c88daa7/2147483647/strip/true/crop/911x664+0+0/resize/1200x875!/quality/75/?url=https%3A%2F%2Fcalifornia-times-brightspot.s3.amazonaws.com%2Fce%2F4a%2F760b0166432abb7344c95f8e054c%2Fvenice-venue-hero.jpg" },
      new Holiday { UserId = users[0].Id, Title = "South African safari", Location = "Cape Town, South Africa", StartDate = new DateOnly(2023, 2, 10), EndDate = new DateOnly(2023, 9, 15), HeroImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQuq5_Jbl0Qgu-t6nA3Oc-C_HfnFrd0kb1k5Q&s" },
      new Holiday { UserId = users[0].Id, Title = "Girls trip to Mallorca", Location = "Mallorca, Spain", StartDate = new DateOnly(2023, 8, 10), EndDate = new DateOnly(2023, 3, 21), HeroImageUrl = "https://mallorca.com/user/pages/04.travel-info/02.ports/02.mallorca-ports/hafen-mallorca.jpg"},
      new Holiday { UserId = users[0].Id, Title = "London Fashion Week", Location = "London, England", StartDate = new DateOnly(2025, 7, 10), EndDate = new DateOnly(2025, 7, 11) , HeroImageUrl = "https://media.timeout.com/images/106039811/image.jpg" },
      new Holiday { UserId = users[0].Id, Title = "Backpacking in Thailand", Location = "Bangkok, Thailand", StartDate = new DateOnly(2025, 9, 10), EndDate = new DateOnly(2025, 9, 15), HeroImageUrl = "https://cdn.sanity.io/images/nxpteyfv/goguides/ef78949372c00ebd066b2cc40557b5688f903890-1600x1067.jpg" },
      new Holiday { UserId = users[0].Id, Title = "Surfing in Australia", Location = "Bondi Beach, Australia", StartDate = new DateOnly(2025, 12, 12), EndDate = new DateOnly(2025, 12, 20) , HeroImageUrl = "https://upload.wikimedia.org/wikipedia/commons/8/86/Bells_beach_surfers.JPG"},
      new Holiday { UserId = users[0].Id, Title = "Watching a Broadway show, NYC", Location = "Manhattan, USA", StartDate = new DateOnly(2025, 3, 10), EndDate = new DateOnly(2025, 3, 21), HeroImageUrl = "https://blog.spothero.com/wp-content/uploads/2013/10/broadway-parking.jpg"},
      new Holiday { UserId = users[0].Id, Title = "Canal boating, Amsterdam", Location = "Amsterdam, The Netherlands", StartDate = new DateOnly(2024, 7, 10), EndDate = new DateOnly(2024, 7, 11), HeroImageUrl = "https://www.blueboat.nl/wp-content/uploads/2021/08/CCC_main-960x600.jpg"},
      new Holiday { UserId = users[0].Id, Title = "Guitar shopping in Sapporo", Location = "Sapporo, Japan", StartDate = new DateOnly(2024, 9, 10), EndDate = new DateOnly(2024, 9, 15), HeroImageUrl = "https://visit.sapporo.travel/wp2022/wp-content/uploads/2024/10/spring-s02-img01.jpg" },
      new Holiday { UserId = users[0].Id, Title = "Ski trip to Vancouver", Location = "Vancouver, Canada", StartDate = new DateOnly(2024, 12, 12), EndDate = new DateOnly(2024, 12, 20), HeroImageUrl = "https://www.vancouverplanner.com/wp-content/uploads/2020/04/grouse-mountain-2-1024x640.jpeg"},
      new Holiday { UserId = users[0].Id, Title = "Oktoberfest in Hamburg", Location = "Hamburg, Germany", StartDate = new DateOnly(2024, 3, 10), EndDate = new DateOnly(2024, 3, 21), HeroImageUrl = "https://tischreservierung-oktoberfest.de/wp-content/uploads/2018/01/human-3237513_1920.jpg" },
      new Holiday { UserId = users[1].Id, Title = "Skiing in the Alps", Location = "Chamonix, France", StartDate = new DateOnly(2024, 12, 15), EndDate = new DateOnly(2024, 12, 16), HeroImageUrl = "https://cdn.prod.website-files.com/5f5777504c01823e93e92c7b/619778bdcf885c9bb9739f57_AdobeStock_122648427.jpg" },
      new Holiday { UserId = users[3].Id, Title = "Tokyo Adventure", Location = "Japan", StartDate = new DateOnly(2025, 3, 5), EndDate = new DateOnly(2025, 3, 6), HeroImageUrl = "https://media.cntraveller.com/photos/6343df288d5d266e2e66f082/16:9/w_6000,h_3375,c_limit/tokyoGettyImages-1031467664.jpeg"  },
      new Holiday { UserId = users[4].Id, Title = "Weekend in Rome", Location = "Rome, Italy", StartDate = new DateOnly(2024, 5, 12), EndDate = new DateOnly(2024, 5, 13), HeroImageUrl = "https://a.loveholidays.com/media-library/~production/73842e65d182c5ddce219a66da2cf5d0036e7954-4683x3122.jpg"  },
      new Holiday { UserId = users[6].Id, Title = "Hiking in the Highlands", Location = "Scotland", StartDate = new DateOnly(2024, 8, 1), EndDate = new DateOnly(2024, 8, 2), HeroImageUrl = "https://wanderlusters.com/wp-content/uploads/2018/10/Scottish-Highlands-Hiking-Guide-1155x770.jpg"  }
    };
      
     // Set CountryId before saving holidays
     foreach (var h in holidays)
       h.CountryId = PickCountryId(h.Location);
      
    context.Holidays.AddRange(holidays);
    context.SaveChanges();

        // === HOLIDAY DAYS ===
        var holidayDays = new List<HolidayDay>();
        foreach (var h in holidays)
        {
            var start = h.StartDate ?? new DateOnly(2024, 1, 1);
            holidayDays.Add(new HolidayDay { HolidayId = h.Id, Date = start });
            holidayDays.Add(new HolidayDay { HolidayId = h.Id, Date = start.AddDays(1) });
        }
        context.HolidayDays.AddRange(holidayDays);
        context.SaveChanges();

        // === DAY ITEMS ===
        var items = new List<DayItem>
        {
            new DayAccommodation { HolidayDayId = holidayDays[0].Id, Name = "Blue Dome Suites", Time = new TimeOnly(14, 0), Location = "Oia" },
            new DayRestaurant    { HolidayDayId = holidayDays[0].Id, Name = "Sunset Dinner in Oia", Time = new TimeOnly(19, 30) },
            new DayActivity      { HolidayDayId = holidayDays[1].Id, Name = "Catamaran Sailing Tour", Time = new TimeOnly(10, 0), Cost = 150.00m },

            new DayAccommodation { HolidayDayId = holidayDays[2].Id, Name = "Alpine Lodge", Time = new TimeOnly(15, 0), Location = "Chamonix" },
            new DayActivity      { HolidayDayId = holidayDays[2].Id, Name = "Full Day Ski Pass", Time = new TimeOnly(8, 30), Cost = 65.00m },
            new DayRestaurant    { HolidayDayId = holidayDays[3].Id, Name = "Après-ski at La Folie Douce", Time = new TimeOnly(16, 0) },

            new DayAccommodation { HolidayDayId = holidayDays[4].Id, Name = "Shinjuku Park Hotel", Time = new TimeOnly(15, 0) },
            new DayRestaurant    { HolidayDayId = holidayDays[4].Id, Name = "Tsukiji Outer Market Breakfast", Time = new TimeOnly(7, 30) },
            new DayActivity      { HolidayDayId = holidayDays[5].Id, Name = "Robot Cafe Experience", Time = new TimeOnly(18, 0), Cost = 80.00m },

            new DayAccommodation { HolidayDayId = holidayDays[6].Id, Name = "AirBnB near Colosseum", Time = new TimeOnly(14, 0) },
            new DayActivity      { HolidayDayId = holidayDays[6].Id, Name = "Colosseum Underground Tour", Time = new TimeOnly(10, 0), Cost = 50.00m },
            new DayActivity      { HolidayDayId = holidayDays[7].Id, Name = "Pasta Making Class", Time = new TimeOnly(17, 0), Cost = 90.00m },

            new DayAccommodation { HolidayDayId = holidayDays[8].Id, Name = "Highland Campsite", Time = new TimeOnly(16, 0), Location = "Glencoe" },
            new DayActivity      { HolidayDayId = holidayDays[8].Id, Name = "Hiking Ben Nevis", Time = new TimeOnly(8, 0), Notes = "Bring waterproofs!" },
            new DayRestaurant    { HolidayDayId = holidayDays[9].Id, Name = "Local Pub Dinner", Time = new TimeOnly(19, 0) }
        };
        context.DayItems.AddRange(items);

        // === FRIENDSHIPS ===
        var friendships = new List<Friendship>
        {
            new() { RequesterId = users[0].Id, ReceiverId = users[1].Id, Status = FriendshipStatus.Accepted },
            new() { RequesterId = users[2].Id, ReceiverId = users[0].Id, Status = FriendshipStatus.Pending },
            new() { RequesterId = users[3].Id, ReceiverId = users[0].Id, Status = FriendshipStatus.Pending },
            new() { RequesterId = users[0].Id, ReceiverId = users[5].Id, Status = FriendshipStatus.Pending }
        };
        context.Friendships.AddRange(friendships);

        context.SaveChanges();
    }

    private sealed record CountryRow(
        [property: JsonPropertyName("country_name")] string CountryName,
        [property: JsonPropertyName("country_code2")] string CountryCode2,
        [property: JsonPropertyName("continent_name")] string ContinentName
    );

    private static string NormalizeCountryName(string name)
    {
        return name switch
        {
            "United Kingdom of Great Britain & Northern Ireland" => "United Kingdom",
            "United States of America" => "United States",
            "Russian Federation" => "Russia",
            "Viet Nam" => "Vietnam",
            "Iran (Islamic Republic of)" => "Iran",
            "Korea, Republic of" => "South Korea",
            "Korea (Republic of)" => "South Korea",
            "Korea, Democratic People's Republic of" => "North Korea",
            _ => name
        };
    }
}