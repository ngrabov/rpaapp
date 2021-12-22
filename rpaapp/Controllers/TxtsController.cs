using Microsoft.AspNetCore.Mvc;
using rpaapp.Data;
using rpaapp.Models;
using Microsoft.EntityFrameworkCore;

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

            var txt = await _context.Txts.FirstOrDefaultAsync(c => c.DocumentId == id);

            if(txt == null) return NotFound();

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
                if(await TryUpdateModelAsync<Txt> (text, "", s => s.BillingGroup, s => s.Bruto1, s => s.Bruto2, s => s.Currency,
                s => s.Group, s => s.IBAN, s => s.InvoiceDate1, s => s.InvoiceDate2, s => s.InvoiceDueDate1, s => s.InvoiceDueDate2,
                s => s.InvoiceNumber1, s => s.InvoiceNumber2, s => s.Name, s => s.Neto1, s => s.Neto2, s => s.ReferenceNumber1,
                s => s.ReferenceNumber2, s => s.State, s => s.VAT, s => s.VATobligation))
                { 
                    try
                    {
                        text.isReviewed = true;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Repository", "Home");
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
    }
}