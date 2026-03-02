using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;
using VisitEmAll.ViewModels;

namespace VisitEmAll.Controllers;

public class DayItemsController : Controller
{
    private readonly VisitEmAllDbContext _db;

    public DayItemsController(VisitEmAllDbContext db)
    {
        _db = db;
    }

//Activities CRUD 
    [Route("/day-items/{dayId}/activities/create")]
    [HttpPost]
    public IActionResult CreateActivity(int dayId)
        {
            return RedirectToAction("holidays", "{id:int}");
        }

    [Route("/day-items/{id}/activities/edit")]
    [HttpPost]
    public IActionResult EditActivity(int dayId)
        {
            return RedirectToAction("holidays", "{id:int}");
        }


    [Route("/day-items/{id}/activities/delete")]
    [HttpPost]
    public IActionResult DeleteActivity(int dayId)
    {
        return RedirectToAction("holidays", "{id:int}");
    }

//Accomodations CRUD 

    [Route("/day-items/{dayId}/accomodations/create")]
    [HttpPost]
    public IActionResult CreateAccomodation(int dayId)
        {
            return RedirectToAction("holidays", "{id:int}");
        }

    [Route("/day-items/{id}/accomodations/edit")]
    [HttpPost]
    public IActionResult EditAccomodation(int dayId)
        {
            return RedirectToAction("holidays", "{id:int}");
        }


    [Route("/day-items/{id}/accomodationss/delete")]
    [HttpPost]
    public IActionResult DeleteAccomodation(int dayId)
    {
        return RedirectToAction("holidays", "{id:int}");
    }

//Restaurants CRUD 


    [Route("/day-items/{dayId}/restaurants/create")]
    [HttpPost]
    public IActionResult CreateRestaurant(int dayId)
        {
            return RedirectToAction("holidays", "{id:int}");
        }

    [Route("/day-items/{id}/restaurants/edit")]
    [HttpPost]
    public IActionResult EditRestaurant(int dayId)
        {
            return RedirectToAction("holidays", "{id:int}");
        }


    [Route("/day-items/{id}/restaurants/delete")]
    [HttpPost]
    public IActionResult DeleteRestaurant(int dayId)
    {
        return RedirectToAction("holidays", "{id:int}");
    }


}