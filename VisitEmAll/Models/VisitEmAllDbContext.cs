using Microsoft.EntityFrameworkCore;

namespace VisitEmAll.Models;

public class VisitEmAllDbContext : DbContext
{

  private readonly IConfiguration _configuration;
  // ==== Model Fields === \\
  public DbSet<User> Users => Set<User>();
  public DbSet<Holiday> Holidays => Set<Holiday>();
  public DbSet<HolidayDay> HolidayDays => Set<HolidayDay>();
  public DbSet<DayItem> DayItems => Set<DayItem>();
  public DbSet<Friendship> Friendships => Set<Friendship>();

  public string? DbPath { get; }

  public string? GetDatabaseName()
  {
    string? DatabaseNameArg = Environment.GetEnvironmentVariable("DATABASE_NAME");

    if (DatabaseNameArg == null)
    {
      System.Console.WriteLine(
          "DATABASE_NAME is null. Defaulting to test database."
      );
      return "visitemall_csharp_test";
    }
    else
    {
      System.Console.WriteLine(
          "Connecting to " + DatabaseNameArg
      );
      return DatabaseNameArg;
    }
  }

  public VisitEmAllDbContext() { }

  public VisitEmAllDbContext(DbContextOptions<VisitEmAllDbContext> options, IConfiguration configuration) : base(options)
  {
    _configuration = configuration;
  }

  // ==== Orig config === \\
  // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  //     => optionsBuilder.UseNpgsql(@"Host=localhost;Username=<YOUR_USERNAME>;Password=<YOUR_PASSWORD>;Database=" + GetDatabaseName());

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    var connectionString = _configuration.GetConnectionString("DefaultConnection");
    optionsBuilder.UseNpgsql(connectionString);
  }
  // ==== Models to be tweaked === \\
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>()
      .HasMany(u => u.Holidays)
      .WithOne(h => h.User)
      .HasForeignKey(h => h.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Holiday>()
      .HasMany(h => h.Days)
      .WithOne(d => d.Holiday)
      .HasForeignKey(d => d.HolidayId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<HolidayDay>()
        .HasMany(d => d.TimelineItems)
        .WithOne(i => i.HolidayDay)
        .HasForeignKey(i => i.HolidayDayId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<DayItem>(entity =>
    {
      entity.Ignore(i => i.ItemType);

      entity.HasDiscriminator<string>("Item") 
        .HasValue<DayActivity>("Activity")
        .HasValue<DayRestaurant>("Restaurant")
        .HasValue<DayAccommodation>("Accommodation");
    });

    modelBuilder.Entity<DayActivity>()
        .Property(a => a.Cost)
        .HasPrecision(18, 2);

    modelBuilder.Entity<Friendship>(entity =>
    {
      entity.ToTable("Friendships");

      entity.HasKey(f => f.Id);

      entity.Property(f => f.Status)
            .HasConversion<string>();

      entity.Property(f => f.CreatedAt)
            .HasDefaultValueSql("NOW()");

      entity.HasOne(f => f.Requester)
            .WithMany()
            .HasForeignKey(f => f.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(f => f.Receiver)
            .WithMany()
            .HasForeignKey(f => f.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasIndex(f => new { f.RequesterId, f.ReceiverId })
            .IsUnique();
    });
  }
  // protected override void OnModelCreating(ModelBuilder modelBuilder)
  // {
  //     modelBuilder.Entity<Post>()
  //       .Navigation(post => post.User)
  //       .AutoInclude();
  // }
}