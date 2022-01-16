using Microsoft.AspNetCore.Mvc;
using rpaapp.Data;
using rpaapp.Models;

namespace rpaapp.Controllers;

public class ProcessController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProcessController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("name")] ProcessType process)
    {
        await _context.Processes.AddAsync(process);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }
}