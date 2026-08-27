using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tasks = _context.Tasks.OrderBy(t => t.CreatedAt).ToList();
            return View("~/Views/Kanban/Index.cshtml", tasks);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
