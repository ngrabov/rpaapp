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

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var files = await _context.pdfs.ToListAsync();
        return View(files);
    }

    public IActionResult Resolve()
    {
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Load(IFormFile file)
    {
        //Datoteka fl = new Datoteka(); 
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
                    //fl.BrojRacuna = arr[0];
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
