using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpaapp.Models;
using System.Linq;
using rpaapp.Data;
using Microsoft.AspNetCore.Authorization;

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
    
    [Authorize(Roles = "Administrator")]
    [Route("Repository")] //authorize
    public async Task<IActionResult> Repository()
    {
        var files = await _context.pdfs.ToListAsync();
        return View(files);
    }

    public async Task<IActionResult> DownloadFile(Guid gd)
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

    //[Authorize(Roles = "Administrator")]
    [Route("Dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var docs = new List<Document>();

        var group = from doc in await _context.Documents.AsQueryable().ToListAsync() 
                    group doc by doc.fguid into divdoc
                    select divdoc;

        foreach(var grouping in group)
        {
            foreach(var doc in grouping)
            {
                docs.Add(doc);
            }
        }

        //var docs = await _context.Documents.ToListAsync();
        //var q = await docs.AsQueryable().GroupBy(c => c.fguid).ToListAsync();
        return View(docs);
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        if(id == null) return NotFound();

        var folder = await _context.Documents.Where(c => c.fguid == id).ToListAsync();
        if(folder == null) return NotFound();
        return View(folder);
    }

    [Route("Upload")]
    public IActionResult Upload()
    {
        return View();
    }

    [Route("Upload")]
    [HttpPost]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        var tgd = Guid.NewGuid();
        foreach(var file in files)
        {
            Document doc = new Document();
            string wbp = Path.GetFileName(file.FileName);

            string fold = "./wwwroot/Document/" + tgd;
            if (!Directory.Exists(fold))
            {
                Directory.CreateDirectory(fold);
            }

            using(var stream = System.IO.File.Create("./wwwroot/Document/" + tgd + "/" + wbp))
            {
                await file.CopyToAsync(stream);
            }
            doc.fname = wbp;
            doc.fguid = tgd;
            await _context.Documents.AddAsync(doc);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
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
