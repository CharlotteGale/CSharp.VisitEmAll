using System.ComponentModel.DataAnnotations;
namespace VisitEmAll.Models;
public class Activity
{
    public int Id { get; set; }

    [Required]
    public int HolidayId { get; set; }
    public Holiday Holiday { get; set; } = null!;

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;
}