using Microsoft.AspNetCore.Mvc;
using rpaapp.Data;
using rpaapp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace rpaapp.Controllers
{
    public class TxtsController : Controller
    {
        private ApplicationDbContext _context;

        public TxtsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if(id == null) return NotFound();

            var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == id);

            var lyt = await _context.Layouts.FirstOrDefaultAsync(c => c.Id == 1);
            if(lyt.isVisible == true)
            {
                ViewData["visible"] = "true";
            }
            else
            {
                ViewData["visible"] = "false";
            }

            if(txt == null) return NotFound();
            populateProcess();
            populatePeople();
            populateInvoices();

            return View(txt);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(int? id)
        {
            if(id == null) return NotFound();

            var text = await _context.Txts.FirstOrDefaultAsync(c => c.Id == id);

            if(text != null)
            {
                if(await TryUpdateModelAsync<Txt> (text, "", s => s.Bruto,  s => s.Currency,
                s => s.Group,  s => s.InvoiceDate, s => s.InvoiceDueDate,
                s => s.InvoiceNumber,  s => s.Name, s => s.Neto, s => s.ReferenceNumber, s => s.ClientCode,
                s => s.State, s => s.VAT, s => s.InvoiceTypeId, s => s.ProcessTypeId, s => s.PersonInChargeId))
                { 
                    try
                    {
                        foreach(var doc in _context.Documents.Where(c => c.fguid == text.DocId))
                        {
                            doc.Status = Status.Confirmed;
                        }
                        text.isReviewed = true;
                        //text.ProcessType = await _context.Processes.FirstOrDefaultAsync(c => c.id == text.ProcessTypeId);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Dashboard", "Home");
                    }
                    catch(DbUpdateException)
                    {
                        ModelState.AddModelError("", "Could not save changes. Try again, and if the problem persists, contact your system administrator.");
                    }
                } 
            }
            ModelState.AddModelError("", "Please select a valid image file.");
            return View(text);
        }

        private async void populateProcess(object selectedTeam = null)
        {
            var processes = await _context.Processes.ToArrayAsync();
            ViewBag.teams = new SelectList(processes, "ptid", "name", selectedTeam);
        }

        private async void populatePeople(object selectedPerson = null)
        {
            var people = await _context.People.ToArrayAsync();
            ViewBag.people = new SelectList(people, "mfilesid", "fullname", selectedPerson);
        }

        private async void populateInvoices(object selectedInvoice = null)
        {
            var invoices = await _context.Invoices.ToListAsync();
            ViewBag.invoices = new SelectList(invoices, "customid", "name", selectedInvoice);
        }
    }
}