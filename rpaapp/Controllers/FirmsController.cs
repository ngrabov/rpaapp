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
    public async Task<IActionResult> Index() //List of companies / firms
    {
        try
        {
            var items = await _context.Firms.ToListAsync();
            return View(items);
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }

    [ApiKey]
    public async Task<IActionResult> GetUpdatedTable() //get new/updated entries from setup table
    {
        var items = await _context.Firms.ToListAsync();
        return Json(items);
    }

    [Authorize(Roles = "Administrator,Manager")]
    public IActionResult Create() //Get html from Views, Create a firm action 
    {
        return View();
    }

    [Authorize(Roles = "Administrator,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,VAT,Keyword,isTrained,DueDate,ProcessTypeId,Currency,Group,State,ClientCode,AltVAT,InvoiceType")]Firm firm) //POST for Create a firm action
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
    public async Task<IActionResult> Edit(int? id) //Get html from Views, Edit firm action 
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
    public async Task<IActionResult> EditPost(int? id) //POST for Edit firm action
    {
        if(id == null) return NotFound();

        var firm = await _context.Firms.FirstOrDefaultAsync(c => c.Id == id);

        if(firm != null)
        {
            if(await TryUpdateModelAsync<Firm>(firm, "", c => c.AltVAT, c => c.ClientCode, c => c.State, c => c.InvoiceType, c => c.Name, c => c.VAT, c => c.Keyword, c => c.Currency, c => c.DueDate, c => c.Group, c => c.isTrained, c => c.ProcessTypeId))
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
    public async Task<IActionResult> Clear() //Delete all firms, returns status 200 if deleted
    {
        try
        {
            var firms = await _context.Firms.ToListAsync();
            _context.Firms.RemoveRange(firms);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }

    [ApiKey]
    public async Task<IActionResult> Parser(int? id) //One time action for parsing a list of firms from Excel file
    {
        if(id == null)
        {
            return Json("Please select id.");
        }
        try
        {
            if(id == 1) //tokic
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
            else //bartog
            {
                WorkBook workbook = WorkBook.Load(_environment.WebRootPath + "/bartog.xlsx"); //hardcode
            
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
                            if(arr.Length > 2)
                            {
                                firm.AltVAT = arr[2];
                            }
                        }  
                        if(cell.ColumnIndex == 1 && cell.TryGetValue(out string name))
                        {
                            firm.Name = name;
                        } 
                        if(cell.ColumnIndex == 2 && cell.TryGetValue(out string cc))
                        {
                            firm.ClientCode = cc;
                        } 
                        if(cell.ColumnIndex == 3 && cell.TryGetValue(out string grp))
                        {
                            firm.Group = grp;
                        }  
                        if(cell.ColumnIndex == 4 && cell.TryGetValue(out string stt))
                        {
                            firm.State = stt;
                        } 
                        if(cell.ColumnIndex == 5 && cell.TryGetValue(out string inv))
                        {
                            firm.InvoiceType = inv;
                        }  
                        if(cell.ColumnIndex == 6 && cell.TryGetValue(out int dd))
                        {
                            firm.DueDate = dd;
                        }
                        if(cell.ColumnIndex == 7 && cell.TryGetValue(out string ptid))
                        {
                            firm.ProcessTypeId = ptid;
                        }  
                        if(cell.ColumnIndex == 8 && cell.TryGetValue(out string it))
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
        }
        catch(Exception e)
        {
            return Json(e.Message.ToString());
        }
    }
}