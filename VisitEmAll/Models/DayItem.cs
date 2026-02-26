using System.ComponentModel.DataAnnotations;
namespace VisitEmAll.Models;

public abstract class DayItem
{
    public int Id { get; set; }
    public int HolidayDayId { get; set; }
    public HolidayDay HolidayDay { get; set; } = null!;
    [Required]
    public string Name { get; set; } = string.Empty;
    public TimeOnly? Time { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public abstract string ItemType { get; }
}