using Microsoft.AspNetCore.Mvc;
using InventoryGudang.Data;
using InventoryGudang.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class UnitController : Controller
{
    private readonly ApplicationDbContext _context;

    public UnitController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Units.ToListAsync());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Unit unit)
    {
        if (ModelState.IsValid)
        {
            _context.Add(unit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(unit);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var unit = await _context.Units.FindAsync(id);
        if (unit == null)
        {
            return NotFound();
        }
        return View(unit);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Unit unit)
    {
        if (id != unit.UnitId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(unit);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnitExists(unit.UnitId))
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
        return View(unit);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var unit = await _context.Units
            .FirstOrDefaultAsync(m => m.UnitId == id);
        if (unit == null)
        {
            return NotFound();
        }

        return View(unit);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var unit = await _context.Units.FindAsync(id);
        _context.Units.Remove(unit);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UnitExists(int id)
    {
        return _context.Units.Any(e => e.UnitId == id);
    }
}