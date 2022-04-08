using Microsoft.AspNetCore.Mvc;
using rpaapp.Models;
using rpaapp.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace rpaapp.Controllers;

public class ProcessController : Controller
{
    private IProcessRepository repository;

    public ProcessController(IProcessRepository repository)
    {
        this.repository = repository;
    }

    [Authorize(Roles="Administrator")]
    public async Task<IActionResult> Index()
    {
        var processes = await repository.GetProcessesAsync();
        return View(processes);
    }

    [Authorize(Roles = "Administrator,Manager")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Administrator,Manager")]
    [HttpPost]
    public async Task<IActionResult> Create([Bind("name,ptid")] ProcessType process)
    {
        await repository.AddProcessAsync(process);
        return RedirectToAction("Index", "Home");
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int? id)
    {
        if(id == null) return NotFound();

        var process = await repository.GetProcessTypeAsync(id);
        if(process == null) return NotFound();

        await repository.RemoveProcessAsync(process);
        return RedirectToAction(nameof(Index));
    }
}