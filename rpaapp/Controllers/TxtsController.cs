using Microsoft.AspNetCore.Mvc;
using rpaapp.Data;
using rpaapp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace rpaapp.Controllers
{
    public class TxtsController : Controller
    {
        private ApplicationDbContext _context;
        private SignInManager<Writer> _signInManager;
        private UserManager<Writer> _userManager;

        public TxtsController(ApplicationDbContext context, SignInManager<Writer> signInManager, UserManager<Writer> userManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Repository", "Home");
        }

        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                if(id == null) return NotFound();

                var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == id);

                var lyt = await _context.Layouts.FirstOrDefaultAsync(c => c.Id == 1);
                if(lyt.isVisible == true)
                {
                    ViewBag.visible = "true";
                }
                else
                {
                    ViewBag.visible = "false";
                }

                if(txt == null) return NotFound();
                
                await populateProcess();
                await populatePeople();
                await populateInvoices();

                return View(txt);
            }
            catch(Exception e)
            {
                return Json(e.Message.ToString());
            }
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize(Roles = "Administrator,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(Guid? id)
        {
            try
            {
                if(id == null) return NotFound();

                var text = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == id);

                if(text != null)
                {
                    if(await TryUpdateModelAsync<Txt> (text, "", s => s.Bruto,  s => s.Currency, s => s.Comment, s => s.PreorderNumber,
                    s => s.Group,  s => s.InvoiceDate, s => s.InvoiceDueDate, s => s.PaymentReference,
                    s => s.InvoiceNumber,  s => s.Name, s => s.Neto, s => s.ReferenceNumber, s => s.ClientCode,
                    s => s.State, s => s.VAT, s => s.InvoiceTypeId, s => s.ProcessTypeId, s => s.PersonInChargeId))
                    { 
                        foreach(var doc in _context.Documents.Where(c => c.fguid == text.DocId))
                        {
                            doc.Status = Status.Confirmed;
                        }
                        text.isReviewed = true;
                        text.isDownloaded = false;
                        await _context.SaveChangesAsync();

                        var currentUser = await _userManager.GetUserAsync(User);
                        var dct = await _context.Documents.Where(c => c.Status == Status.Ready && (c.writername == currentUser.FullName || c.writername == "E-Račun ")).FirstOrDefaultAsync();
                        if(await _context.Documents.Where(c => c.Status == Status.Ready && (c.writername == currentUser.FullName || c.writername == "E-Račun ")).CountAsync() != 0)
                        {
                            return RedirectToAction("Details", "Txts", new{ id = dct.fguid});
                        } 
                        return RedirectToAction("Dashboard", "Home");
                    } 
                    await populateInvoices();
                    await populatePeople();
                    await populateProcess();
                    return View(text);
                }
                else
                {
                    return NotFound();
                } 
            }
            catch(Exception e)
            {
                return Json(e.Message.ToString());
            }
        }

        public async Task<IActionResult> SearchMe(string key)
        {
            var txts = new List<Txt>();
            if(!String.IsNullOrEmpty(key)) 
            {
                txts = await _context.Txts.Where(c => c.Name.ToUpper().Contains(key.ToUpper()) || c.InvoiceNumber.ToUpper().Contains(key.ToUpper()) || c.VAT.ToUpper().Contains(key.ToUpper()) ).ToListAsync();
            }
            return View(txts.OrderByDescending(c => c.InvoiceDate).Take(50));
        }

        public async Task<IActionResult> FileDetails(Guid? id)
        {
            if(id == null) return NotFound();

            var doc = await _context.Documents.FirstOrDefaultAsync(c => c.fguid == id);
            if(doc == null) return NotFound();

            return View(doc);
        }

        private async Task populateProcess(object selectedTeam = null)
        {
            var processes = await _context.Processes.ToArrayAsync();
            ViewBag.teams = new SelectList(processes, "ptid", "name", selectedTeam);
        }

        private async Task populatePeople(object selectedPerson = null)
        {
            var people = await _context.People.ToArrayAsync();
            ViewBag.people = new SelectList(people, "mfilesid", "fullname", selectedPerson);
        }

        private async Task populateInvoices(object selectedInvoice = null)
        {
            var invoices = await _context.Invoices.ToListAsync();
            ViewBag.invoices = new SelectList(invoices, "customid", "name", selectedInvoice);
        }
    }
}