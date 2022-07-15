using Microsoft.AspNetCore.Mvc;
using rpaapp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using rpaapp.Models;
using Microsoft.AspNetCore.Identity;

namespace rpaapp.Controllers
{
    public class WritersController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<Writer> _userManager;
        private readonly SignInManager<Writer> _signInManager;

        public WritersController(ApplicationDbContext context, SignInManager<Writer> signInManager, UserManager<Writer> userManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index() //list of writers / users
        {
            var writers = await _context.Writers.ToListAsync();
            return View(writers);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create() //returns html for writer creation from Views
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email")]Writer writer, string pw, bool admin) //POST action for writer creation
        {
            var wrtr = await _context.Writers.FirstOrDefaultAsync(c => c.Email == writer.Email);
            if(await _context.Writers.ContainsAsync(wrtr))
            {
                return Json("There is already a writer with the same email. Try again.");
            }
            try
            {
                writer.UserName = writer.Email;
                await _context.Writers.AddAsync(writer);
                var result = await _userManager.CreateAsync(writer, pw);
                await _userManager.AddToRoleAsync(writer, "Manager");
                if(admin == true)
                {
                    await _userManager.AddToRoleAsync(writer, "Administrator");
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes.");
            }
            return View(writer);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id) //returns html for edit writer action
        {
            if(id == null)
            {
                return NotFound();
            }

            var writer = await _context.Writers.FirstOrDefaultAsync(c => c.Id == id);
            if(writer == null) return NotFound();

            return View(writer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditPost(int? id) //POST Action for editing writers
        {
            if(id == null) return NotFound();

            var writer = await _context.Writers.FirstOrDefaultAsync(c => c.Id == id);

            if(writer != null)
            {
                if(await TryUpdateModelAsync<Writer> (writer, "", s => s.FirstName,  s => s.LastName))
                { 
                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "Writers");
                    }
                    catch(DbUpdateException)
                    {
                        ModelState.AddModelError("", "Could not save changes. Try again, and if the problem persists, contact your system administrator.");
                    }
                } 
            }
            ModelState.AddModelError("", "Error updating the writer.");
            return View(writer);
        }
        
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ChangeWriter() //set invoice uploader to be administrator, one time function
        {
            var cnt = await _context.pdfs.Where(c => c.Writer.FirstName == null).CountAsync();
            if(cnt != 0)
            {
                var pdfs = await _context.pdfs.Where(c => c.Writer.FirstName == null).ToListAsync();
                var superman = await _context.Writers.FirstOrDefaultAsync(c => c.Id == 1);
                foreach(var item in pdfs)
                {
                    item.Writer = superman;
                }
                await _context.SaveChangesAsync();
                return Json("Successfully changed writer.");
            }
            return Json("Nothing to change.");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteWriters() //delete writers whose first name is null, one time function
        {
            try
            {
            var writers = await _context.Writers.Where(c => c.FirstName == null).ToListAsync();
            _context.Writers.RemoveRange(writers);
            await _context.SaveChangesAsync();
            return Json("200");
            }
            catch(Exception e)
            {
                return Json(e.Message.ToString());
            }
        }
    
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id) //returns html for writer deletion from Views
        {
            if(id == null) return NotFound();
            var writer = await _context.Writers.FirstOrDefaultAsync(c => c.Id == id);
            if(writer == null) return NotFound();
            return View(writer);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int? id) // POST Action for user deletion
        {
            if(id == null) return NotFound();

            var pdfs = await _context.pdfs.Where(c => c.Writer.Id == id).CountAsync();
            if(pdfs == 0)
            {
                var writer = await _context.Writers.FirstOrDefaultAsync(c => c.Id == id);
                _context.Writers.Remove(writer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Writers");
            }
            return Json("Selected writer has some data associated with them and cannot be deleted.");
        }
    }
}