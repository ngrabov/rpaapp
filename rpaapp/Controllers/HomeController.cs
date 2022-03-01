using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpaapp.Models;
using rpaapp.Data;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace rpaapp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Writer> _userManager;
    private readonly SignInManager<Writer> _signInManager;
    private IWebHostEnvironment _environment;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment environment, SignInManager<Writer> signInManager, UserManager<Writer> userManager)
    {
        _context = context;
        _logger = logger;
        _environment = environment;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Index(string order, string search)
    {
        return await Dashboard(order, search);
    }
    
    [Authorize(Roles = "Administrator,Manager")]
    [Route("Repository")]
    public async Task<IActionResult> Repository()
    {
        var files = await _context.pdfs.Include(c => c.Writer).ToListAsync();
        return View(files);
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Clear()
    {
        if((await _context.Writers.CountAsync()) < 4)
        {/* 
            var wrt = await _context.Writers.Where(c => c.Id > 3).ToListAsync(); //pazi na broj korisnika
            _context.Writers.RemoveRange(wrt); */
            var pdfs = await _context.pdfs.ToListAsync();
            _context.pdfs.RemoveRange(pdfs);
            var txts = await _context.Txts.ToListAsync();
            _context.Txts.RemoveRange(txts);
            var docs = await _context.Documents.ToListAsync();
            _context.Documents.RemoveRange(docs);
            await _context.SaveChangesAsync();
            return Json("Database records deleted successfully.");
        }
        return Json("Could not delete db records.");
    }
    
    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DownloadFile(Guid gd) //pdfs
    {
        string path = Path.Combine(_environment.WebRootPath) + "/" + gd;

        byte[] bytes = System.IO.File.ReadAllBytes(path);

        var ftd = await _context.pdfs.FirstOrDefaultAsync(c => c.guid == gd);
        var fname = ftd.fname;
        var dname = gd.ToString() + ".pdf";
        
        ftd.isDownloaded = true;
        await _context.SaveChangesAsync();
        System.IO.File.Delete(path);
        return File(bytes, "application/octet-stream", dname);
    }

    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Download(Guid gd) // txts
    {
        string path = Path.Combine(_environment.WebRootPath) + "/Document/" + gd + "/" + gd + ".pdf";

        byte[] bytes = System.IO.File.ReadAllBytes(path);

        var ftd = await _context.Documents.FirstOrDefaultAsync(c => c.fguid == gd);
        var fname = ftd.fname;
        var dname = gd.ToString() + ".pdf";

        var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == gd);
        txt.isDownloaded = true;
        
        await _context.SaveChangesAsync();
        return File(bytes, "application/octet-stream", dname);
    }

    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DownloadFiles(Guid gd) //documents
    {
        string path = Path.Combine(_environment.WebRootPath) + "/Document/" + gd + "/" + gd + ".pdf";

        byte[] bytes = System.IO.File.ReadAllBytes(path);

        var fname = gd + ".pdf";
        var txt = await _context.Txts.Where(c => c.DocId == gd).FirstOrDefaultAsync();
        txt.isDownloaded = true;
        
        await _context.SaveChangesAsync();
        return File(bytes, "application/octet-stream", fname);
    }

    [Authorize(Roles = "Administrator,Manager")]
    [Route("Dashboard")]
    public async Task<IActionResult> Dashboard(string order, string search)
    {
        var docs = new List<Document>();

        if(String.IsNullOrEmpty(order))
        {
            ViewData["SortParm"] = "name_desc";
        }
        else
        {
            ViewData["SortParm"] = "";
        }
        ViewData["CurrentFilter"] = search;

        if(order == "Size")
        {
            ViewData["SizeParm"] = "size_desc";
        }
        else
        {
            ViewData["SizeParm"] = "Size";
        }

        if(order == "Upld")
        {
            ViewData["UpldParm"] = "upld_desc";
        }
        else
        {
            ViewData["UpldParm"] = "Upld";
        }

        if(order == "Time")
        {
            ViewData["TimeParm"] = "time_desc";
        }
        else
        {
            ViewData["TimeParm"] = "Time";
        }

        var group = from doc in await _context.Documents.AsQueryable().ToListAsync() 
                    group doc by doc.fguid into divdoc
                    select divdoc;

        foreach(var grouping in group)
        {
            var doc = grouping.FirstOrDefault();
            docs.Add(doc);
        }

        if(!String.IsNullOrEmpty(search))
        {
            docs = docs.Where(c => c.pdfname.Contains(search) || (c.RAC_number != null) && (c.RAC_number.Contains(search)) || (c.writername.ToUpper().Contains(search.ToUpper()))).ToList();
        }

        if(order == "name_desc")
        {
            docs = docs.OrderByDescending(c => c.pdfname).ToList();
        }
        else if (order == "time_desc")
        {
            docs = docs.OrderByDescending(c => c.uploaded).ToList();
        }
        else if (order == "upld_desc")
        {
            docs = docs.OrderByDescending(c => c.writername).ToList();
        }
        else if (order == "size_desc")
        {
            docs = docs.OrderByDescending(c => c.fsize).ToList();
        }
        else if (order == "Time")
        {
            docs = docs.OrderBy(c => c.uploaded).ToList();
        }
        else if (order == "Upld")
        {
            docs = docs.OrderBy(c => c.writername).ToList();
        }
        else if (order == "Size")
        {
            docs = docs.OrderBy(c => c.fsize).ToList();
        }
        else
        {
            docs = docs.OrderBy(c => c.pdfname).ToList();
        }

        return View(docs);
    }

    [Authorize(Roles = "Administrator")]
    [Route("Upload")] //files, get, web
    public IActionResult Upload()
    {
        return View();
    }

    [Route("Upload")] //Files, web
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        if(files.Count > 20)
        {
            return Json("You selected more than 20 files.");
        }
        await Complex(files);

        return RedirectToAction("Index", "Home");
    }

    //[Authorize(Roles = "Administrator")]
    [Route("UploadFiles")] //API endpoint
    [HttpPost]
    public async Task<IActionResult> UploadFiles()
    {
        var files = Request.Form.Files.ToList();
        if(files.Count > 20)
        {
            return Json("You selected more than 20 files.");
        }

        await Complex(files);

        return Ok();
    }

    [Route("DmsMove")]
    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DmsMove()
    {
        var txts = await _context.Txts.Where(c => c.isReviewed == true).Where(c => c.isDownloaded == false).ToListAsync();
        return Json(txts);
    }

    [Route("Archive")]
    [HttpPost]
    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> ArchiveFile(Guid gd, string rac)
    {
        var docs = await _context.Documents.Where(c => c.fguid == gd).ToListAsync();

        var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == gd);
        txt.isReviewed = false;
        foreach(var item in docs)
        {
            item.Status = Status.Archived;
            item.RAC_number = rac;
        }
        await _context.SaveChangesAsync();
        return Ok();
    }

    [Route("ReportProblem")]
    [HttpPost]
    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> ReportProblem(Guid? gd, string rac, string desc)
    {
        if(gd == Guid.Empty) return NotFound();
        var docs = await _context.Documents.Where(c => c.fguid == gd).ToListAsync();
        var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == gd); //cleanup
        if(docs == null || txt == null) return NotFound();
        txt.isDownloaded = true;
        foreach(var item in docs)
        {
            item.Status = Status.Problem;
            if(!String.IsNullOrEmpty(rac))
            {
                item.RAC_number = rac;
            }
            if(!String.IsNullOrEmpty(desc))
            {
                item.Description = desc;
            }
        }
        await _context.SaveChangesAsync();
        return Ok();
    }

    [Route("GetRac")]
    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetRac(Guid gd)
    {
        var rac = await _context.Documents.Where(c => c.Status == Status.Archived).Where(c => c.fguid == gd).FirstOrDefaultAsync();
        var nr = rac.RAC_number;
        return Json(nr);
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

    public async Task Complex(List<IFormFile> files)
    {
        string tgd = "";
        long sz = 0;
        int c = 0, k = 0;
        Guid fn = Guid.Empty;
        string pngs = "";
        foreach(var file in files)
        {
            string ext = Path.GetExtension(file.FileName);
            if(ext == ".pdf")
            {
                tgd = Path.GetFileNameWithoutExtension(file.FileName);
                fn = Guid.Parse(tgd);
                c++;
            }
            if(ext == ".png") //pazi
            {
                pngs += Path.GetFileName(file.FileName) + "|";
            }
            if(ext == ".txt")
            {
                k++;
            }
        }

        if(c != 0 && k != 0)
        {
            foreach(var file in files)
            {
                sz = file.Length;
                Document doc = new Document();
                string wbp = Path.GetFileName(file.FileName);
                string ext = Path.GetExtension(file.FileName);
                
                var pdf = await _context.pdfs.Include(c => c.Writer).FirstOrDefaultAsync(c => c.guid == fn);

                string fold = "./wwwroot/Document/" + tgd;
                if (!Directory.Exists(fold))
                {
                    Directory.CreateDirectory(fold);
                }

                using(var stream = System.IO.File.Create("./wwwroot/Document/" + tgd + "/" + wbp))
                {
                    await file.CopyToAsync(stream);
                }
                
                doc.writername = pdf.Writer.FullName;
                pdf.isUploaded = true;
                doc.fsize = sz;
                doc.fname = wbp;
                doc.pdfname = pdf.fname;
                doc.uploaded = DateTime.Now;
                doc.fguid = Guid.Parse(tgd);
                doc.Status = Status.Ready;
                await _context.Documents.AddAsync(doc);

                if(ext == ".txt")
                {
                    Txt text = new Txt();
                    using(StreamReader sr = new StreamReader("./wwwroot/Document/" + tgd + "/" + wbp))
                    {
                        string line;
                        while((line = sr.ReadLine()) != null) //parser
                        {
                            if(line == "Name:")
                            {
                                text.Name = sr.ReadLine();
                            }
                            if(line == "Client_code:")
                            {
                                text.ClientCode = sr.ReadLine();
                            }
                            if(line == "VAT_number:")
                            {
                                text.VAT = sr.ReadLine();
                            }
                            if(line == "Process:")
                            {
                                text.ProcessTypeId = Int32.Parse(sr.ReadLine());
                            }
                            if(line == "Currency:")
                            {
                                text.Currency = sr.ReadLine();
                            }
                            if(line == "Group:")
                            {
                                text.Group = sr.ReadLine();
                            }
                            if(line == "State:")
                            {
                                text.State = sr.ReadLine();
                            }
                            if(line == "Invoice_number:")
                            {
                                text.InvoiceNumber = sr.ReadLine();
                            }
                            if(line == "Invoice_date:")
                            {
                                var dat = sr.ReadLine();
                                if(DateTime.TryParse(dat, CultureInfo.CreateSpecificCulture("fr-FR"), DateTimeStyles.None,  out DateTime res))
                                {
                                    text.InvoiceDate = DateTime.Parse(dat, CultureInfo.CreateSpecificCulture("fr-FR"));
                                }
                                else
                                {
                                    var ts = DateTime.Now;
                                    text.InvoiceDate = ts;
                                }
                            }
                            if(line == "Invoice_duedate:")
                            {
                                var dt = sr.ReadLine();
                                if(DateTime.TryParse(dt, CultureInfo.CreateSpecificCulture("fr-FR"), DateTimeStyles.None,  out DateTime res))
                                {
                                    text.InvoiceDueDate = DateTime.Parse(dt, CultureInfo.CreateSpecificCulture("fr-FR"));
                                }
                                else
                                {
                                    var tst = DateTime.Now;
                                    text.InvoiceDueDate = tst;
                                }
                            }
                            if(line == "Neto:")
                            {
                                var cvt = sr.ReadLine().Replace(',','.');
                                if(double.TryParse(cvt, out double res))
                                {
                                    text.Neto = double.Parse(cvt);
                                }
                            }
                            if(line == "Bruto:")
                            {
                                var cvt2 = sr.ReadLine().Replace(',','.');
                                if(double.TryParse(cvt2, out double res2))
                                {
                                    text.Bruto = double.Parse(cvt2);
                                }
                            }
                            if(line == "Payment_number:")
                            {
                                text.ReferenceNumber = sr.ReadLine();
                            }
                            if(line == "Payment_reference:")
                            {
                                text.PaymentReference = sr.ReadLine();
                            }
                        }
                    }
                    if(pngs != "")
                    {
                        text.pngNames = pngs.Remove(pngs.Length - 1);
                    }
                    text.DocId = Guid.Parse(tgd);
                    text.isReviewed = false;
                    text.isDownloaded = false;
                    await _context.Txts.AddAsync(text);
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Delete(Guid? gd)
    {
        if(gd == null) return NotFound();
        var files = await _context.Documents.Where(c => c.fguid == gd).FirstOrDefaultAsync();
        if(files == null) return NotFound();
        return View(files);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> DeleteConfirmed(Guid gd)
    {
        var files = await _context.Documents.Where(c => c.fguid == gd).ToListAsync();
        var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == gd);
        _context.Txts.Remove(txt);
        foreach(var item in files)
        {
            _context.Documents.Remove(item);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Dashboard", "Home");
    }

    [HttpPost]
    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteDuplicate(Guid gd)
    {
        var files = await _context.Documents.Where(c => c.fguid == gd).ToListAsync();
        var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == gd);
        _context.Txts.Remove(txt);
        foreach(var item in files)
        {
            _context.Documents.Remove(item);
        }
        await _context.SaveChangesAsync();
        return Ok();
    }

    //[Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Cancel(Guid? gd)
    {
        if(gd == null) return NotFound();

        var docs = await _context.Documents.Where(c => c.fguid == gd).ToListAsync();

        foreach(var doc in docs)
        {
            doc.Status = Status.Confirmed;
            var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == gd);
            txt.isDownloaded = false;
        }
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    //[Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Resolve(Guid? gd)
    {
        if(gd == null) return NotFound();

        var docs = await _context.Documents.Where(c => c.fguid == gd).ToListAsync();

        foreach(var doc in docs)
        {
            doc.Status = Status.Resolved;
        }
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
