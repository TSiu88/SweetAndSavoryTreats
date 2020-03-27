using Microsoft.AspNetCore.Mvc;
using SweetSavoryTreats.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SweetSavoryTreats.Controllers
{
  public class TreatsController : Controller
  {
    private readonly SweetSavoryTreatsContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public TreatsController(UserManager<ApplicationUser> userManager,SweetSavoryTreatsContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public ActionResult Index()
    {
      return View(_db.Treats.OrderBy(x => x.Name).ToList());
    }

    [Authorize]
    public ActionResult Create()
    {
      if(TempData["message"] != null)
      {
        ViewBag.Message = TempData["message"].ToString();
      }
      ViewBag.FlavorId = new SelectList(_db.Flavors.OrderBy(x => x.Title), "FlavorId", "Title");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Treat treat, int FlavorId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      treat.User = currentUser;
      if(treat.Name == null)
      {
        TempData["message"] = "Name required! Please enter name to continue.";
        return Create();
      }
      else if (_db.Treats.FirstOrDefault(t => t.Name.ToLower() == treat.Name.ToLower()) != null)
      {
        TempData["message"] = "Treat already exists!";
        return Create();
      }
      else
      {
        _db.Treats.Add(treat);
        if (FlavorId != 0)
        {
          _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = treat.TreatId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");  
      }
    }

    public ActionResult Details(int id)
    {
      var thisTreat = _db.Treats
        .Include(treat => treat.Flavors)
        .ThenInclude(join => join.Flavor)
        .FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }

    [Authorize]
    public ActionResult Edit(int id)
    {
      if(TempData["message"] != null)
      {
        ViewBag.Message = TempData["message"].ToString();
      }
      var thisTreat = _db.Treats.FirstOrDefault(treats => treats.TreatId == id);
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Title");
      return View(thisTreat);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(Treat treat, int FlavorId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      treat.User = currentUser;
      if(treat.Name == null)
      {
        TempData["message"] = "Name required! Please enter Name to continue.";
        return RedirectToAction("Edit");
      }
      else
      {
        if (FlavorId != 0)
        {
          _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = treat.TreatId });
        }
        _db.Entry(treat).State = EntityState.Modified;
        _db.SaveChanges();
        return RedirectToAction("Index");  
      }
      
    }

    [Authorize]
    public ActionResult Delete(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treats => treats.TreatId == id);
      return View(thisTreat);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      
      var thisTreat = _db.Treats.FirstOrDefault(treats => treats.TreatId == id);
      thisTreat.User = currentUser;
      _db.Treats.Remove(thisTreat);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [Authorize]
    public ActionResult AddFlavor(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treats => treats.TreatId == id);
      List<Flavor> selectedFlavors =  _db.Flavors.OrderBy(x => x.Title).ToList();
      var TreatFlavors = _db.FlavorTreat.Where(c => c.TreatId == id).ToList();
      foreach(FlavorTreat element in TreatFlavors)
      {
        Flavor thisFlavor = _db.Flavors.Find(element.FlavorId);
        selectedFlavors.Remove(thisFlavor);
      }
      ViewBag.FlavorId = new SelectList(selectedFlavors, "FlavorId", "Title");
      return View(thisTreat);
    }

    [HttpPost]
    public async Task<ActionResult> AddFlavor(Treat treat, int FlavorId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      treat.User = currentUser;
      if (FlavorId != 0)
      {
      _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = treat.TreatId });
      }
      _db.SaveChanges();
      return RedirectToAction("Details", new {id=treat.TreatId});
    }

    [HttpPost]
    public ActionResult DeleteFlavor(int joinId)
    {
      var joinEntry = _db.FlavorTreat.FirstOrDefault(entry => entry.FlavorTreatId == joinId);
      _db.FlavorTreat.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new {id=joinEntry.TreatId});
    }
  }
}