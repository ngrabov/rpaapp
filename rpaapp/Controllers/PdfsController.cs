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

    public async Task<IActionResult> Index()
    {
        var pdfs = await _context.pdfs.ToListAsync();
        return Json(pdfs);
    }

    [HttpPost] //API endpoint za PDFove
    public async Task<IActionResult> UploadFiles()
    {
        var files = Request.Form.Files;

        var filePath = Path.GetTempFileName();

        foreach(var filez in files)
        {
            Pdf pdf = new Pdf();
            var gd = Guid.NewGuid();

            string wbp = Path.GetFileName(filez.FileName);
            if (filez.Length > 0)
                using (var stream = System.IO.File.Create("./wwwroot/" + gd))
                    filez.CopyTo(stream);

            pdf.fname = wbp;
            pdf.guid = gd;
            await _context.pdfs.AddAsync(pdf);
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    public IActionResult Upload() 
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        foreach(var file in files)
        {
            Pdf pdf = new Pdf();
            var gd = Guid.NewGuid();
            string wbp = Path.GetFileName(file.FileName);

            using(var stream = System.IO.File.Create("./wwwroot/" + gd))
            {
                await file.CopyToAsync(stream);
            }
            pdf.fname = wbp;
            pdf.guid = gd;
            await _context.pdfs.AddAsync(pdf);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Repository", "Home");
    }
}