namespace VisitEmAll.ViewModels;

public class HolidayDetailsViewModel
{
    public int HolidayId { get; set; }
    public int OwnerUserId { get; set; }
    public string? Title { get; set; }
    public string? Location { get; set; }
    public string? HeroImageUrl { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? TotalCost { get; set; }

    public List<HolidayDayViewModel> Days { get; set; } = new();

    public List<IFormFile> ImageFiles { get; set; } = new();

    // NEW — tells the view which inline form to show
    public string? AddType { get; set; }
    public int? AddDayId { get; set; }
}
