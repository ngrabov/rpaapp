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

    [Authorize(Roles = "Administrator,Manager")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Administrator,Manager")]
    [HttpPost]
    public async Task<IActionResult> Create([Bind("name")] ProcessType process)
    {
        await repository.AddProcessAsync(process);
        return RedirectToAction("Index", "Home");
    }
}