using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpaapp.Models;
using rpaapp.Data;

namespace rpaapp.Controllers;

public class PdfsController : Controller
{
    private ApplicationDbContext _context;
    public PdfsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var pdfs = _context.pdfs.ToListAsync();
        return Json(pdfs);
    }

    public IActionResult UploadMe()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadMe(Pdf pdf, IFormFile file)
    {
        string wbp = Path.GetFileName(file.FileName);

        using(var stream = System.IO.File.Create("./wwwroot/" + wbp))
        {
            await file.CopyToAsync(stream);
        }
        pdf.fullpath = wbp;
        await _context.pdfs.AddAsync(pdf);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}