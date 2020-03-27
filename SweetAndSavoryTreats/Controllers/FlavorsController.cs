using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SweetSavoryTreats.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SweetSavoryTreats.Controllers
{
  public class FlavorsController : Controller
  {
    private readonly SweetSavoryTreatsContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public FlavorsController(UserManager<ApplicationUser> userManager,SweetSavoryTreatsContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public ActionResult Index()
    {
      return View(_db.Flavors.OrderBy(x => x.Title).ToList());
    }

    [Authorize]
    public ActionResult Create()
    {
      if(TempData["message"] != null)
      {
        ViewBag.Message = TempData["message"].ToString();
      }
      return View();
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(Flavor flavor)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      flavor.User = currentUser;
      if(flavor.Title == null)
      {
        TempData["message"] = "Title required! Please enter title to continue.";
        return Create();
      }
      else if (_db.Flavors.FirstOrDefault(f => f.Title.ToLower() == flavor.Title.ToLower()) != null)
      {
        TempData["message"] = "Flavor already exists!";
        return Create();
      }
      else
      {
        _db.Flavors.Add(flavor);
        _db.SaveChanges();
        return RedirectToAction("Index");  
      }
      
    }

    public ActionResult Details(int id)
    {
      var thisFlavor = _db.Flavors
        .Include(flavor => flavor.Treats)
        .ThenInclude(join => join.Treat)
        .FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    [Authorize]
    public ActionResult Edit(int id)
    {
      if(TempData["message"] != null)
      {
        ViewBag.Message = TempData["message"].ToString();
      }
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(Flavor flavor)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      flavor.User = currentUser;
      if(flavor.Title == null)
      {
        TempData["message"] = "Title required! Please enter title to continue.";
        return RedirectToAction("Edit");
      }
      else
      {
        _db.Entry(flavor).State = EntityState.Modified;
        _db.SaveChanges();
        return RedirectToAction("Index");  
      }
    }

    [Authorize]
    public ActionResult Delete(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      thisFlavor.User = currentUser;
      _db.Flavors.Remove(thisFlavor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}