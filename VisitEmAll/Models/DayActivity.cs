namespace VisitEmAll.Models;

public class DayActivity : DayItem { 
    public decimal? Cost { get; set; }
    public override string ItemType => "Activity"; 
}