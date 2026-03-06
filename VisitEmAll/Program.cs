using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

using VisitEmAll.Models;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(600);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = 100_000_000; });

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options => { options.MultipartBodyLengthLimit = 100_000_000; });

builder.Services.AddScoped<VisitEmAll.ActionFilters.AuthenticationFilter>();
builder.Services.AddScoped<VisitEmAll.Services.FriendshipService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'. Check your Environment Variables.");
}
builder.Services.AddDbContext<VisitEmAllDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

// === DB SEEDER === \\
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<VisitEmAllDbContext>();

    try
    {
        logger.LogInformation("Attempting to apply database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");

        if (!context.Countries.Any())
        {
            logger.LogInformation("Countries table is empty, seeding countries...");
            var env = services.GetRequiredService<IWebHostEnvironment>();
            CountriesSeeder.Seed(context, env.WebRootPath);
            logger.LogInformation("Countries seeded successfully.");
        }

        if (args.Contains("--seed"))
        {
            logger.LogWarning("Seed flag detected. Wiping and reseeding database...");
            DbSeeder.Seed(context);
            logger.LogInformation("Database seeding completed successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Database initialization failed.");

        throw;
    }
}

app.Run();

public static class CountriesSeeder
{
    public static void Seed(VisitEmAllDbContext context, string webRootPath)
    {
        var jsonPath = Path.Combine(webRootPath, "data", "countries.json");
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
            .GroupBy(c => c.Iso2)
            .Select(g => g.First())
            .ToList();

        context.Countries.AddRange(toAdd);
        context.SaveChanges();
    }

    private sealed record CountryRow(
    [property: JsonPropertyName("country_name")]
        string CountryName,
    [property: JsonPropertyName("country_code2")]
        string CountryCode2,
    [property: JsonPropertyName("continent_name")]
        string ContinentName
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