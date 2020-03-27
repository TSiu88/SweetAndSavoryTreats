using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SweetSavoryTreats.Models;
using System.Threading.Tasks;
using SweetSavoryTreats.ViewModels;

namespace SweetSavoryTreats.Controllers
{
  public class AccountController : Controller
  {
    private readonly SweetSavoryTreatsContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController (UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, SweetSavoryTreatsContext db)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _db = db;
    }

    public ActionResult Index()
    {
      if(TempData["logoutMessage"] != null)
      {
        ViewBag.Message = TempData["logoutMessage"].ToString();
      }
      return View();
    }

    public IActionResult Register()
    {
      if(TempData["messageR"] != null)
      {
        ViewBag.Message = TempData["messageR"].ToString();
      }
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Register (RegisterViewModel model)
    {
      if(TempData["messageR"] != null)
      {
        ViewBag.Message = TempData["messageR"].ToString();
      }
      var user = new ApplicationUser { UserName = model.Email };
      if(model.Password != null)
      {
        IdentityResult result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          return RedirectToAction("Index");
        }
        else
        {
          TempData["messageR"] = "Please Fill in Required Fields!";
          return View();
        }
      }
      else
      {
        TempData["messageR"] = "Please Fill in Required Fields!";
        return View();
      }
    }

    public ActionResult Login()
    {
      if(TempData["message"] != null)
      {
        ViewBag.Message = TempData["message"].ToString();
      }
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(LoginViewModel model)
    {
      Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
      if (result.Succeeded)
      {
        return RedirectToAction("Index");
      }
      else
      {
        TempData["message"] = "Incorrect email or password!";
        return Login();
      }
    }

    [HttpPost]
    public async Task<ActionResult> LogOff()
    {
      TempData["logoutMessage"] = "You have been logged off.";
      await _signInManager.SignOutAsync();
      return RedirectToAction("Index");
    }
  }
}