using System.ComponentModel.DataAnnotations;
namespace VisitEmAll.Models;

public class Holiday
{
    public int Id { get; set; }
    [Required]
    public int UserId { get; set; }
    public User? User { get; set; }
    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Location { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? TotalCost { get; set; } //Maybe compute at a later stage?
    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<HolidayDay> Days { get; set; } = new List<HolidayDay>();
}