namespace VisitEmAll.Models;

public class HolidayImage
{
    public int Id { get; set; }
    public string FilePath { get; set; } 
    public int HolidayId { get; set; }
    public Holiday Holiday { get; set; }
}