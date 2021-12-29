﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpaapp.Models;
using rpaapp.Data;
using System.Globalization;

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
    
    //[Authorize(Roles = "Administrator")]
    [Route("Repository")]
    public async Task<IActionResult> Repository()
    {
        var files = await _context.pdfs.ToListAsync();
        return View(files);
    }

    public async Task<IActionResult> DownloadFile(Guid gd) //pdfs
    {
        string path = Path.Combine(_environment.WebRootPath) + "/" + gd;

        byte[] bytes = System.IO.File.ReadAllBytes(path);

        var ftd = await _context.pdfs.FirstOrDefaultAsync(c => c.guid == gd);
        var fname = ftd.fname;
        var dname = gd.ToString() + ".pdf";
        
        /* _context.pdfs.Remove(ftd);
        await _context.SaveChangesAsync();
        System.IO.File.Delete(path); */
        return File(bytes, "application/octet-stream", dname);
    }

    public async Task<IActionResult> DownloadFiles(Guid gd, string filename) //documents
    {
        string path = Path.Combine(_environment.WebRootPath) + "/Document/" + gd + "/" + filename;

        byte[] bytes = System.IO.File.ReadAllBytes(path);

        var ftd = await _context.Documents.Where(c => c.fguid == gd).Where(c => c.fname == filename).FirstOrDefaultAsync();
        var fname = ftd.fname;
        /* 
        _context.pdfs.Remove(ftd);
        await _context.SaveChangesAsync();
        System.IO.File.Delete(path); */
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
            var doc = grouping.FirstOrDefault();
            docs.Add(doc);
        }
        //var docs = await _context.pdfs.ToListAsync();
        return View(docs);
    }
    
    public async Task<IActionResult> Details(Guid? id) //izbrisi kasnije
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

    [Route("Upload")] //Files
    [HttpPost]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        await Complex(files);

        return RedirectToAction("Index", "Home");
    }

    [Route("UploadFiles")] //API endpoint
    [HttpPost]
    public async Task<IActionResult> UploadFiles()
    {
        var files = Request.Form.Files.ToList();

        await Complex(files);

        return Ok();
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
        Guid fn = Guid.Empty;
        string pngs = "";
        foreach(var file in files)
        {
            string ext = Path.GetExtension(file.FileName);
            if(ext == ".pdf")
            {
                tgd = Path.GetFileNameWithoutExtension(file.FileName);
                fn = Guid.Parse(tgd);
            }
            if(ext == ".png") //pazi
            {
                pngs += Path.GetFileName(file.FileName) + "|";
            }
        }

        foreach(var file in files)
        {
            sz = file.Length;
            Document doc = new Document();
            string wbp = Path.GetFileName(file.FileName);
            string ext = Path.GetExtension(file.FileName);
            
            var pdf = await _context.pdfs.FirstOrDefaultAsync(c => c.guid == fn);

            string fold = "./wwwroot/Document/" + tgd;
            if (!Directory.Exists(fold))
            {
                Directory.CreateDirectory(fold);
            }

            using(var stream = System.IO.File.Create("./wwwroot/Document/" + tgd + "/" + wbp))
            {
                await file.CopyToAsync(stream);
            }
            doc.fsize = sz;
            doc.fname = wbp;
            doc.pdfname = pdf.fname;
            doc.uploaded = DateTime.Now;
            doc.fguid = Guid.Parse(tgd);
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
                        if(line == "VAT_number:")
                        {
                            text.VAT = sr.ReadLine();
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
                        if(line == "Billing_group:")
                        {
                            text.BillingGroup = sr.ReadLine();
                        }
                        if(line == "IBAN:")
                        {
                            text.IBAN = sr.ReadLine();
                        }
                        if(line == "VAT_obligation:")
                        {
                            text.VATobligation = sr.ReadLine();
                        }
                        if(line == "Invoice_number1:")
                        {
                            text.InvoiceNumber1 = sr.ReadLine();
                        }
                        if(line == "Invoice_date1:")
                        {
                            text.InvoiceDate1 = DateTime.Parse(sr.ReadLine(), CultureInfo.CreateSpecificCulture("fr-FR"));
                        }
                        if(line == "Invoice_duedate1:")
                        {
                            text.InvoiceDueDate1 = DateTime.Parse(sr.ReadLine(), CultureInfo.CreateSpecificCulture("fr-FR"));
                        }
                        if(line == "Neto1:")
                        {
                            var cvt = sr.ReadLine().Replace(',','.');
                            text.Neto1 = double.Parse(cvt);
                        }
                        if(line == "Bruto1:")
                        {
                            var cvt2 = sr.ReadLine().Replace(',','.');
                            text.Bruto1 = double.Parse(cvt2);
                        }
                        if(line == "Reference_number1:")
                        {
                            text.ReferenceNumber1 = sr.ReadLine();
                        }
                        if(line == "Invoice_number2:")
                        {
                            text.InvoiceNumber2 = sr.ReadLine();
                        }
                        if(line == "Invoice_date2:")
                        {
                            text.InvoiceDate2 = DateTime.Parse(sr.ReadLine(), CultureInfo.CreateSpecificCulture("fr-FR"));
                        }
                        if(line == "Invoice_duedate2:")
                        {
                            text.InvoiceDueDate2 = DateTime.Parse(sr.ReadLine(), CultureInfo.CreateSpecificCulture("fr-FR"));
                        }
                        if(line == "Neto2:")
                        {
                            var cvt3 = sr.ReadLine().Replace(',','.');
                            text.Neto2 = double.Parse(cvt3);
                        }
                        if(line == "Bruto2:")
                        {
                            var cvt4 = sr.ReadLine().Replace(',','.');
                            text.Bruto2 = double.Parse(cvt4);
                        }
                        if(line == "Reference_number2:")
                        {
                            text.ReferenceNumber2 = sr.ReadLine();
                        }
                    }
                }
                text.pngNames = pngs.Remove(pngs.Length - 1);
                text.DocumentId = Guid.Parse(tgd);
                text.isReviewed = false;
                await _context.Txts.AddAsync(text);
            }
        }

        await _context.SaveChangesAsync();
    }
}
