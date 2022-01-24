using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpaapp.Models;
using rpaapp.Data;
using Microsoft.AspNetCore.Identity;

namespace rpaapp.Controllers;

public class PdfsController : Controller
{
    private ApplicationDbContext _context;
    private SignInManager<Writer> _signInManager;
    private UserManager<Writer> _userManager;
    public PdfsController(ApplicationDbContext context, SignInManager<Writer> signInManager, UserManager<Writer> userManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        var pdfs = await _context.pdfs.Where(c => c.isDownloaded == false).ToListAsync();
        return Json(pdfs);
    }

    [HttpPost] //API endpoint za PDFove
    public async Task<IActionResult> UploadFiles()
    {
        var files = Request.Form.Files.ToList();

        await Complex(files);
        return Ok();
    }

    public IActionResult Upload() 
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        await Complex(files);

        return RedirectToAction("Repository", "Home");
    }

    public async Task Complex(List<IFormFile> files)
    {
        var currentuser = await _userManager.GetUserAsync(User);
        foreach(var file in files)
        {
            Pdf pdf = new Pdf();
            var gd = Guid.NewGuid();
            string wbp = Path.GetFileName(file.FileName);

            if(currentuser != null)
            {
                pdf.Writer = currentuser;
            }

            using(var stream = System.IO.File.Create("./wwwroot/" + gd))
            {
                await file.CopyToAsync(stream);
            }
            pdf.fname = wbp;
            pdf.fsize = file.Length;
            pdf.uploaded = DateTime.Now;
            pdf.isDownloaded = false;
            pdf.isUploaded = false;
            pdf.guid = gd;
            await _context.pdfs.AddAsync(pdf);
        }

        await _context.SaveChangesAsync();
    }
}