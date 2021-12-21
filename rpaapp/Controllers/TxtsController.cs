using Microsoft.AspNetCore.Mvc;
using rpaapp.Data;
using rpaapp.Models;
using Microsoft.EntityFrameworkCore;

namespace rpaapp.Controllers
{
    public class TxtsController : Controller
    {
        private ApplicationDbContext _context;

        public TxtsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if(id == null) return NotFound();

            var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocumentId == id);

            if(txt == null) return NotFound();

            return View(txt);
        }
    }
}