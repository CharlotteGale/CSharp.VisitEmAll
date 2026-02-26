using System.ComponentModel.DataAnnotations;
namespace VisitEmAll.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }

    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required, MinLength(8)]
    public required string Password { get; set; } = null!;
    public string? HomeTown { get; set; } = null!;
    public string? ProfileImg {get; set; } = null!;

    public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}