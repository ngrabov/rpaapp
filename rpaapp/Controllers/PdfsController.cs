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

    [HttpPost]
    public async Task<IActionResult> UploadFiles()
    {
        var files = Request.Form.Files;

        var filePath = Path.GetTempFileName();

        foreach(var filez in files)
        {
            Pdf pdf = new Pdf();
            var ts = DateTime.Now.ToString("yyyyMMddhhmmssfff");
            string wbp = ts + "_" + Path.GetFileName(filez.FileName);
            if (filez.Length > 0)
                using (var stream = System.IO.File.Create("./wwwroot/" + wbp))
                    filez.CopyTo(stream);

            pdf.fullpath = wbp;
            await _context.pdfs.AddAsync(pdf);
        }

        await _context.SaveChangesAsync();
        return Ok();
    }


    public IActionResult UploadMe()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadMe(List<IFormFile> files)
    {
        foreach(var file in files)
        {
            Pdf pdf = new Pdf();
            var ts = DateTime.Now.ToString("yyyyMMddhhmmssfff");
            string wbp = ts + "_" + Path.GetFileName(file.FileName);

            using(var stream = System.IO.File.Create("./wwwroot/" + wbp))
            {
                await file.CopyToAsync(stream);
            }
            pdf.fullpath = wbp;
            await _context.pdfs.AddAsync(pdf);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}