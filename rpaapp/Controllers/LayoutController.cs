using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpaapp.Data;
using rpaapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace rpaapp.Controllers;

public class LayoutController : Controller 
{
    private ApplicationDbContext _context;
    private readonly SignInManager<Writer> _signInManager;
    private readonly UserManager<Writer> _userManager;

    public LayoutController(ApplicationDbContext context, SignInManager<Writer> signInManager, UserManager<Writer> userManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Details() // layout dashboard
    {
        var layouts = await _context.Layouts.FirstOrDefaultAsync(c => c.Id == 1);
        return View(layouts);
    }

    [Authorize(Roles = "Administrator,Manager")]
    public IActionResult Docs() //returns html for documentation from Views
    { 
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit() //POST action for editing the layout
    {
        var lyt = await _context.Layouts.FirstOrDefaultAsync(c => c.Id == 1);

        if(lyt != null)
        {
            if(await TryUpdateModelAsync<LayoutConfig> (lyt, "", s => s.isVisible, s => s.Color, s => s.pngname))
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