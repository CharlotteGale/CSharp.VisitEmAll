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

builder.Services.AddScoped<VisitEmAll.ActionFilters.AuthenticationFilter>();

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
