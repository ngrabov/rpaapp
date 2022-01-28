using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpaapp.Data;
using rpaapp.Models;

namespace rpaapp.Controllers;

public class LayoutController : Controller 
{
    private ApplicationDbContext _context;

    public LayoutController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Details()
    {
        var layouts = await _context.Layouts.FirstOrDefaultAsync(c => c.Id == 1);
        return View(layouts);
    }

    [HttpPost]
    public async Task<IActionResult> Edit()
    {
        var lyt = await _context.Layouts.FirstOrDefaultAsync(c => c.Id == 1);

        if(lyt != null)
        {
            if(await TryUpdateModelAsync<LayoutConfig> (lyt, "", s => s.isVisible, s => s.Color))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Dashboard", "Home");
                }
                catch(DbUpdateException)
                {
                    ModelState.AddModelError("", "Could not save changes.");
                }
            }
        }
        ModelState.AddModelError("", "Could not save changes. Try again.");
        return View(lyt);
    }
}