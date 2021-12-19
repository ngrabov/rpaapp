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

    public IActionResult Index()
    {
        return View();
    }
    
    [Route("Repository")]
    public async Task<IActionResult> Repository()
    {
        var files = await _context.pdfs.ToListAsync();
        return View(files);
    }

    public async Task<IActionResult> DownloadFile(string fileName, Guid gd)
    {
        string path = Path.Combine(_environment.WebRootPath) + "/" + gd;

        byte[] bytes = System.IO.File.ReadAllBytes(path);

        var ftd = await _context.pdfs.FirstOrDefaultAsync(c => c.guid == gd);
        var fname = ftd.fname;
        
        _context.pdfs.Remove(ftd);
        await _context.SaveChangesAsync();
        System.IO.File.Delete(path);
        return File(bytes, "application/octet-stream", fname);
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
