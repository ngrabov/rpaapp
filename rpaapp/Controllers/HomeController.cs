using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpaapp.Models;
using rpaapp.Data;

namespace rpaapp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private IWebHostEnvironment _environment;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _logger = logger;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var files = await _context.pdfs.ToListAsync();
        return View(files);
    }

    public async Task<FileResult> DownloadFile(string fileName)
    {
        string path = Path.Combine(_environment.WebRootPath) + "/" + fileName;

        byte[] bytes = System.IO.File.ReadAllBytes(path);

        var ftd = await _context.pdfs.FirstOrDefaultAsync(c => c.fullpath == fileName);
        _context.pdfs.Remove(ftd);
        await _context.SaveChangesAsync();
        System.IO.File.Delete(path);
        return File(bytes, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> ClearMe()
    {
        var pedefs = await _context.pdfs.ToListAsync();
        _context.pdfs.RemoveRange(pedefs);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Resolve()
    {
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Load(IFormFile file)
    {
        if(file != null)
        {
            try
            {
                using(StreamReader sr = new StreamReader(file.FileName))
                {
                    int i = 1;
                    string line;
                    List<string> arr = new List<string>();
                    while((line = sr.ReadLine()) != null)
                    {
                        if(i % 2 == 0)
                        {
                            arr.Add(line);
                        }
                        i++;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
