using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;

namespace VisitEmAll.Controllers;

[Route("countries")]
public class CountriesController : Controller
{
    private readonly VisitEmAllDbContext _context;

    public CountriesController(VisitEmAllDbContext context)
    {
        _context = context;
    }

    // GET /countries/search?q=ja
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? q)
    {
        q = (q ?? "").Trim();

        if (q.Length < 1)
        {
            return Json(Array.Empty<object>());
        }

        // case-insensitive search on Name and Iso2
        var results = await _context.Countries
            .AsNoTracking()
            .Where(c =>
                EF.Functions.ILike(c.Name, $"%{q}%") ||
                EF.Functions.ILike(c.Iso2, $"{q}%"))
            .OrderBy(c => c.Name)
            .Take(12)
            .Select(c => new
            {
                id = c.Id,
                name = c.Name,
                iso2 = c.Iso2,
                continent = c.Continent
            })
            .ToListAsync();

        return Json(results);
    }
}