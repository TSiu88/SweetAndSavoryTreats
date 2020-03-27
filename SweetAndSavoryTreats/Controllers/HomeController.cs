using Microsoft.AspNetCore.Mvc;

namespace SweetSavoryTreats.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet("/")]
    public ActionResult Index() { return View(); }
  }
}