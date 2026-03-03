namespace VisitEmAll.ViewModels;

public class HolidayDetailsViewModel
{
    public int HolidayId { get; set; }
    public int OwnerUserId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Location { get; set; }
    public decimal? TotalCost { get; set; }

    public DateOnly? StartDate { get; set; }  
    public DateOnly? EndDate { get; set; }  
    public string? HeroImageUrl { get; set; }

    public List<HolidayDayViewModel> Days { get; set; } = new();
}
