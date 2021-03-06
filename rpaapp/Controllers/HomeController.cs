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
    public async Task<IActionResult> Index(string order, string search, DateTime? date) //redirects to Dashboard
    {
        return await Dashboard(order, search, date);
    }
    
    [Authorize(Roles = "Administrator,Manager")]
    [Route("Repository")]
    public async Task<IActionResult> Repository() //returns a list of pdfs
    {
        var files = await _context.pdfs.Include(c => c.Writer).Where(c => c.isUploaded == false).ToListAsync();
        return View(files);
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Clear() //ClearDB; deletes all invoice data, pdfs, documents and txts from Database. Use carefully
    {
        if((await _context.Writers.CountAsync()) < 8)
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
    
    [ApiKey]
    public async Task<IActionResult> DownloadFile(Guid gd) //Download pdfs
    {
        try
        {
            string path = Path.Combine(_environment.WebRootPath) + "/" + gd;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            var ftd = await _context.pdfs.FirstOrDefaultAsync(c => c.guid == gd);
            if(ftd == null)
            {
                return NotFound();
            }
            var fname = ftd.fname;
            var dname = gd.ToString() + ".pdf";
            
            ftd.isDownloaded = true;
            await _context.SaveChangesAsync();
            return File(bytes, "application/octet-stream", dname);
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Retry(int? id) //Return pdfs from 'Waiting' to 'Files to download' in Repository
    {
        try
        {
            if(id == null) return NotFound();

            var pdf = await _context.pdfs.FirstOrDefaultAsync(c => c.Id == id);
            if(pdf == null) return NotFound();

            pdf.isDownloaded = false;
            pdf.isUploaded = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Repository));
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Push(int? id) //Clears the pdfs from Repository by setting isDownloaded and isUploaded to true
    {
        try
        {
            if(id == null) return NotFound();
            var pdf = await _context.pdfs.FirstOrDefaultAsync(c => c.Id == id);
            if(pdf == null) return NotFound();

            pdf.isDownloaded = true;
            pdf.isUploaded = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Repository));
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }

    [ApiKey]
    public async Task<IActionResult> Download(Guid gd) // Download reuploaded pdf
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

    [ApiKey]
    public async Task<IActionResult> DownloadFiles(Guid gd) //Duplicate function, similar to Download
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
    public async Task<IActionResult> Dashboard(string order, string search, DateTime? date) //Returns a table of invoices' documents divided in tabs (Ready, Confirmed, Archived, Problem)
    {
        try
        {
            var docs = new List<Document>();
            ViewData["CurrentFilter"] = search;

            if(date == null)
            { 
                ViewData["CurrentTime"] = DateTime.Now.Date.ToString("yyyy-MM-dd");
                date = DateTime.Now.Date;
            }
            else
            {
                ViewData["CurrentTime"] = date.Value.ToString("yyyy-MM-dd");
            }

            var group = from doc in await _context.Documents.ToListAsync()
                        group doc by doc.fguid into divdoc
                        select divdoc;

            foreach(var grouping in group)
            {
                var doc = grouping.FirstOrDefault();
                docs.Add(doc);
            }

            var prblms = from prblm in await _context.Documents.Where(c => c.Status == Status.Problem).ToListAsync()
                        group prblm by prblm.uploaded.Date into divprb
                        select divprb;

            var ready = from rdy in await _context.Documents.Where(c => c.Status == Status.Ready).ToListAsync()
                        group rdy by rdy.uploaded.Date into divrdy
                        select divrdy;

             var lyt = await _context.Layouts.FirstOrDefaultAsync(c => c.Id == 1);
            if(lyt.isVisible == true)
            {
                ViewData["visible"] = "true";
            }
            else
            {
                ViewData["visible"] = "false";
            }

            ViewData["Ready"] = ready;
            ViewData["Problem"] = prblms;

            if(!String.IsNullOrEmpty(search))
            {
                docs = docs.Where(c => c.pdfname.Contains(search) || (c.RAC_number != null) && (c.RAC_number.Contains(search)) || (c.writername.ToUpper().Contains(search.ToUpper()))).ToList();
            }
            docs = docs.Where(c => c.uploaded.Date == date).OrderByDescending(c => c.uploaded).ToList();
            
            return View(docs);
        }
        catch(Exception e)
        {
            var currentuser = await _userManager.GetUserAsync(User);
            if(currentuser.Id == 1)
            {
                return Json(e.Message.ToString());
            }
            else return Json("An error occurred. Please contact administrator.");
        }

    }

    [Authorize(Roles = "Administrator")]
    [Route("Upload")] //Documents, get, web
    public IActionResult Upload()
    {
        return View();
    }

    [Route("Upload")] //Documents, web, post
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        if(files.Count > 20)
        {
            return Json("You selected more than 20 files.");
        }
        try
        {
            await Complex(files);
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }

        return RedirectToAction("Index", "Home");
    }

    [ApiKey]
    [Route("UploadFiles")] //API endpoint for Upload files
    [HttpPost]
    public async Task<IActionResult> UploadFiles()
    {
        var files = Request.Form.Files.ToList();
        if(files.Count > 20)
        {
            return Json("You selected more than 20 files.");
        }

        try
        {
            await Complex(files);
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }

        return Ok();
    }

    [Route("DmsMove")]
    [ApiKey]
    public async Task<IActionResult> DmsMove() //get json data for txts
    {
        var txts = await _context.Txts.Where(c => c.isReviewed == true).Where(c => c.isDownloaded == false).ToListAsync();
        return Json(txts);
    }

    [Route("Archive")]
    [HttpPost]
    [ApiKey]
    public async Task<IActionResult> ArchiveFile(Guid gd, string rac) //put invoices into 'Archived' state
    {
        var docs = await _context.Documents.Where(c => c.fguid == gd).ToListAsync();

        var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == gd);
        if(txt == null)
        {
            return NotFound();
        }
        txt.isReviewed = false;
        foreach(var item in docs)
        {
            item.Status = Status.Archived;
            item.RAC_number = rac;
        }
        await _context.SaveChangesAsync();

        string path = Path.Combine(_environment.WebRootPath) + "/Document/" + txt.DocId;
        if(Directory.Exists(path))
        {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach(var f in di.EnumerateFiles())
            {
                f.Delete();
            }
            System.IO.Directory.Delete(path);
        }
        return Ok();
    }

    [Route("ReportProblem")]
    [HttpPost]
    [ApiKey]
    public async Task<IActionResult> ReportProblem(Guid? gd, string rac, string desc) //put invoices into 'Problem' state
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
    [ApiKey]
    public async Task<IActionResult> GetRac(Guid gd) //get rac number
    {
        var rac = await _context.Documents.Where(c => c.Status == Status.Archived).Where(c => c.fguid == gd).FirstOrDefaultAsync();
        var nr = rac.RAC_number;
        return Json(nr);
    }

    public IActionResult Privacy() //not used
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() //for production errors, .net generated
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize(Roles = "Administrator,Manager")] //search by RAC number
    public async Task<IActionResult> GoFindMe(string rac) 
    {
        var docs = new List<Document>();
        if(!String.IsNullOrEmpty(rac)) 
        {
            docs = await _context.Documents.Where(c => c.RAC_number.ToUpper().Contains(rac.ToUpper())).ToListAsync();
        }
        return View(docs);
    }

    public async Task Complex(List<IFormFile> files) //auxiliary function
    {
        string tgd = "";
        long sz = 0;
        int c = 0, k = 0, n = 0;
        Guid fn = Guid.Empty;
        Txt text = new Txt();
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
            if(ext == ".png")
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
                    using(StreamReader sr = new StreamReader("./wwwroot/Document/" + tgd + "/" + wbp))
                    {
                        string line;
                        text.InvoiceDate = DateTime.Now.Date;
                        text.InvoiceDueDate = DateTime.Now.Date;
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
                                n = 1;
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
                            if(line == "Order_number:")
                            {
                                text.ReferenceNumber = sr.ReadLine();
                            }
                            if(line == "Payment_reference:")
                            {
                                text.PaymentReference = sr.ReadLine();
                            }
                            if(line == "Preorder_number:")
                            {
                                text.PreorderNumber = sr.ReadLine();
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
        else
        {
            throw new Exception("Txt or pdf file not provided.");
        }

        await _context.SaveChangesAsync(); 

        if(fn != Guid.Empty)
        {
            string path = Path.Combine(_environment.WebRootPath) + "/" + fn;
            System.IO.File.Delete(path);
            if(n == 1)
            {
                await DirectResolveAsync(fn);
            }
        }  
    }

    public async Task DirectResolveAsync(Guid? gd) // used for invoices that go straight into confirmed state
    {
        var dcmnts = await _context.Documents.Where(c => c.fguid == gd).ToListAsync();
        foreach(var f in dcmnts)
        {
            f.Status = Status.Confirmed;
        }

        var text = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == gd);
        text.isReviewed = true;
        await _context.SaveChangesAsync();
    }

    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Delete(Guid? gd) //gets html from Views for document deletion
    {
        if(gd == null) return NotFound();
        var files = await _context.Documents.Where(c => c.fguid == gd).FirstOrDefaultAsync();
        if(files == null) return NotFound();
        return View(files);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> DeleteConfirmed(Guid gd) //post action for document deletion
    {
        try
        {
            var files = await _context.Documents.Where(c => c.fguid == gd).ToListAsync();
            var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == gd);
            _context.Txts.Remove(txt);
            foreach(var item in files)
            {
                _context.Documents.Remove(item);
            }
            string path = Path.Combine(_environment.WebRootPath) + "/Document/" + gd;
            if(Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach(var f in di.EnumerateFiles())
                {
                    f.Delete();
                }
                System.IO.Directory.Delete(path);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard", "Home");
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }

    [HttpPost]
    [ApiKey]
    public async Task<IActionResult> DeleteDuplicate(Guid gd) //POST action for deletion of duplicate documents, rarely used
    {
        try
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
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }

    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Cancel(Guid? gd) //function for returning problem invoices into confirmed state
    {
        try
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
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }
    
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Resolve(Guid? gd) // function for resolving problem invoices
    {
        try
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
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }

    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> MassDelete(List<Guid> Del) //action for deletion of multiple problem invoices
    {
        try
        {
            var tmp = HttpContext.Request.Form["ccl"];
            if(!String.IsNullOrEmpty(HttpContext.Request.Form["ccl"].ToString()))
            {
                foreach(var item in Del)
                {
                    await Cancel(item);
                }
            }
            else
            {
                foreach(var item in Del)
                {
                    var doc = await _context.Documents.Where(c => c.fguid == item).ToListAsync();
                    _context.Documents.RemoveRange(doc);
                    var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == item);
                    _context.Txts.Remove(txt);
                    string path = Path.Combine(_environment.WebRootPath) + "/Document/" + item;
                    if(Directory.Exists(path))
                    {
                        DirectoryInfo di = new DirectoryInfo(path);
                        foreach(var f in di.EnumerateFiles())
                        {
                            f.Delete();
                        }
                        System.IO.Directory.Delete(path);
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
        return RedirectToAction("Index", "Home");
    }
}
