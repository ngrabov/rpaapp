using rpaapp.Data;
using rpaapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using IronXL;

namespace rpaapp.Controllers;

public class FirmsController : Controller
{
    private readonly ApplicationDbContext _context;
    private IWebHostEnvironment _environment;

    public FirmsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Index()
    {
        var items = await _context.Firms.ToListAsync();
        return View(items);
    }

    [Authorize(Roles = "Administrator,Manager")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Administrator,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,VAT,Keyword,isTrained,DueDate,ProcessTypeId,Currency,Group")]Firm firm)
    {
        try
        {
            await _context.Firms.AddAsync(firm);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch(DbUpdateException)
        {
            ModelState.AddModelError("", "");
        }
        return View(firm);
    }

    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Edit(int? id)
    {
        if(id == null) return NotFound();

        var firm = await _context.Firms.FirstOrDefaultAsync(c => c.Id == id);
        if(firm == null) return NotFound();

        return View(firm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Edit")]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> EditPost(int? id)
    {
        if(id == null) return NotFound();

        var firm = await _context.Firms.FirstOrDefaultAsync(c => c.Id == id);

        if(firm != null)
        {
            if(await TryUpdateModelAsync<Firm>(firm, "", c => c.Name, c => c.VAT, c => c.Keyword, c => c.Currency, c => c.DueDate, c => c.Group, c => c.isTrained, c => c.ProcessTypeId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Firms");
                }
                catch(DbUpdateException)
                {
                    ModelState.AddModelError("", "Could not save changes. Try again, and if the problem persists, contact your system administrator.");
                }
            }
        }
        ModelState.AddModelError("", "Error updating the firm.");
        return View(firm);
    }

    [ApiKey]
    public async Task<IActionResult> Clear()
    {
        var firms = await _context.Firms.ToListAsync();
        _context.Firms.RemoveRange(firms);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [ApiKey]
    public async Task<IActionResult> Parser() 
    {
        try
        {
            WorkBook workbook = WorkBook.Load(_environment.WebRootPath + "/tokic.xlsx"); //hardcode
            WorkSheet sheet = workbook.WorkSheets.First();
            var rows = sheet.Rows.Skip(1);

            foreach(var row in rows)
            {
                Firm firm = new Firm();
                foreach(var cell in row)
                {
                    if(cell.ColumnIndex == 0 && cell.TryGetValue(out string vat))
                    {
                        var arr = vat.Split('$');
                        firm.VAT = arr[0];
                        if(arr.Length > 1)
                        {
                            firm.Keyword = arr[1];
                        }
                    }  
                    if(cell.ColumnIndex == 1 && cell.TryGetValue(out string name))
                    {
                        firm.Name = name;
                    } 
                    if(cell.ColumnIndex == 2 && cell.TryGetValue(out string cur))
                    {
                        firm.Currency = cur;
                    } 
                    if(cell.ColumnIndex == 3 && cell.TryGetValue(out string grp))
                    {
                        firm.Group = grp;
                    } 
                    if(cell.ColumnIndex == 4 && cell.TryGetValue(out int dd))
                    {
                        firm.DueDate = dd;
                    }
                    if(cell.ColumnIndex == 5 && cell.TryGetValue(out string ptid))
                    {
                        firm.ProcessTypeId = ptid;
                    }  
                    if(cell.ColumnIndex == 6 && cell.TryGetValue(out string it))
                    {
                        if(it.ToUpper().Contains("YES"))
                        {
                            firm.isTrained = true;
                        }
                        else
                        {
                            firm.isTrained = false;
                        }
                    }  
                }
                await _context.Firms.AddAsync(firm); 
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }
}