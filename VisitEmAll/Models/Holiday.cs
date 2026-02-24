using System.ComponentModel.DataAnnotations;

public class Holiday
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Location { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    [MaxLength(200)]
    public string? Accommodation { get; set; }

    public decimal? Cost { get; set; }

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}