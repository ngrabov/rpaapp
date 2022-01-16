using Microsoft.AspNetCore.Mvc;
using rpaapp.Data;
using rpaapp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace rpaapp.Controllers
{
    public class TxtsController : Controller
    {
        private ApplicationDbContext _context;

        public TxtsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if(id == null) return NotFound();

            var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocId == id);

            if(txt == null) return NotFound();
            populateProcess();

            return View(txt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(int? id)
        {
            if(id == null) return NotFound();

            var text = await _context.Txts.FirstOrDefaultAsync(c => c.Id == id);

            if(text != null)
            {
                if(await TryUpdateModelAsync<Txt> (text, "", s => s.BillingGroup, s => s.Bruto,  s => s.Currency,
                s => s.Group, s => s.IBAN, s => s.InvoiceDate, s => s.InvoiceDueDate,
                s => s.InvoiceNumber,  s => s.Name, s => s.Neto, s => s.ReferenceNumber,
                 s => s.State, s => s.VAT, s => s.VATobligation, s => s.ProcessTypeId))
                { 
                    try
                    {
                        foreach(var doc in _context.Documents.Where(c => c.fguid == text.DocId))
                        {
                            doc.Status = Status.Confirmed;
                        }
                        text.isReviewed = true;
                        text.ProcessType = await _context.Processes.FirstOrDefaultAsync(c => c.id == text.ProcessTypeId);
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
            ViewBag.teams = new SelectList(processes, "id", "name", selectedTeam);
        }
    }
}