using Microsoft.AspNetCore.Mvc;
using rpaapp.Data;
using rpaapp.Models;
using rpaapp.Repositories;

namespace rpaapp.Controllers;

public class ProcessController : Controller
{
    //private readonly ApplicationDbContext _context;
    private IProcessRepository repository;

    public ProcessController(/* ApplicationDbContext context */IProcessRepository repository)
    {
        this.repository = repository;
        //_context = context;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("name")] ProcessType process)
    {
        await repository.AddProcessAsync(process);
        return RedirectToAction("Index", "Home");
    }
}