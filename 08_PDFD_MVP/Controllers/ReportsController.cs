using Microsoft.AspNetCore.Mvc;
using VisitorLog_PDFD.Data;

namespace VisitorLog_PDFD.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var reports = _context.Reports.ToList(); // Fetch data from the view
            return View(reports);
        }
    }
}
