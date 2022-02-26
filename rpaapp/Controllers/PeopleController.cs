using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using rpaapp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using rpaapp.Models;

namespace rpaapp.Controllers;

public class PeopleController : Controller
{
    private ApplicationDbContext context;

    public PeopleController(ApplicationDbContext context)
    {
        this.context = context;
    }

    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Index()
    {
        var people = await context.People.ToListAsync();
        return View(people);
    }

    //[Authorize(Roles = "Administrator")]
    public IActionResult Create()
    {
        return View();
    }

    //[Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,mfilesid")]PersonInCharge person)
    {
        try
        {
            await context.People.AddAsync(person);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch(DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes.");
        }
        return View(person);
    }
}