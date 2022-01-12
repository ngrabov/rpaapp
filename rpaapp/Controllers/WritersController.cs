using Microsoft.AspNetCore.Mvc;
using rpaapp.Data;
using Microsoft.AspNetCore.Authorization;
using rpaapp.Models;

namespace rpaapp.Controllers
{
    public class WritersController : Controller
    {
        private ApplicationDbContext _context;

        public WritersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("FirstName,LastName")]Writer writer)
        {
            await _context.Writers.AddAsync(writer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}