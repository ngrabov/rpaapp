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
        public async Task<IActionResult> Index()
        {
            var writers = await _context.Writers.ToListAsync();
            return View(writers);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email")]Writer writer, string pw, bool admin)
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
    }
}