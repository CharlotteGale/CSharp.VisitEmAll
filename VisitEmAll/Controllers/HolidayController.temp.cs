// Holiday Model:
// Id,
// Title,
// Location,
// StartDate,
// EndDate,
// Accommodation,
// Cost,
// ThumbnailUrl,
// UserId,
// Desc

// <Activities List >

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;

namespace VisitEmAll.Controllers;

public class HolidayController : Controller
{
  private VisitEmAllDbContext _context;

  [Route("/holidays/{id:int}")]
  [HttpGet]
  public IActionResult GetHoliday(int id)
  {
    var holiday = _context.Holiday
      .Include(h => h.Holidays)
      .FirstOrDefault(h => h.Id == id);

    return View("/Views/Holiday/{id}/Index.cshtml", holiday);
  };

  //Edit View

  //Edit Post

  //Holiday Delete
}