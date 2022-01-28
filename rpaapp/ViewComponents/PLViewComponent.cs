using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpaapp.Models;
using rpaapp.Data;

namespace ViewComponentSample.ViewComponents
{
    public class PLViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;

        public PLViewComponent(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var col = await db.Layouts.FirstOrDefaultAsync(c => c.Id == 1);
            return View(col);
        }
    }
}