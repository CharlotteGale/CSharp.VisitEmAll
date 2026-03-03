using System.ComponentModel.DataAnnotations;
namespace VisitEmAll.Models;


public class UserLikedHoliday
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int HolidayId { get; set; }
    public Holiday Holiday { get; set; }
}
