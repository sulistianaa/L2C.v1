using Microsoft.AspNetCore.Mvc;
using InventoryGudang.Data;
using InventoryGudang.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ReasonController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReasonController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Reasons.ToListAsync());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Reason reason)
    {
        if (ModelState.IsValid)
        {
            _context.Add(reason);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(reason);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reason = await _context.Reasons.FindAsync(id);
        if (reason == null)
        {
            return NotFound();
        }
        return View(reason);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Reason reason)
    {
        if (id != reason.ReasonId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(reason);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReasonExists(reason.ReasonId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(reason);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reason = await _context.Reasons
            .FirstOrDefaultAsync(m => m.ReasonId == id);
        if (reason == null)
        {
            return NotFound();
        }

        return View(reason);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var reason = await _context.Reasons.FindAsync(id);
        _context.Reasons.Remove(reason);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ReasonExists(int id)
    {
        return _context.Reasons.Any(e => e.ReasonId == id);
    }
}