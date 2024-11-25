using Microsoft.AspNetCore.Mvc;
using InventoryGudang.Data;
using InventoryGudang.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class FungsiBagianController : Controller
{
    private readonly ApplicationDbContext _context;

    public FungsiBagianController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Index()
    {
        return View(await _context.FungsiBagians.ToListAsync());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(FungsiBagian fungsiBagian)
    {
        if (ModelState.IsValid)
        {
            _context.Add(fungsiBagian);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(fungsiBagian);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var fungsiBagian = await _context.FungsiBagians.FindAsync(id);
        if (fungsiBagian == null)
        {
            return NotFound();
        }
        return View(fungsiBagian);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, FungsiBagian fungsiBagian)
    {
        if (id != fungsiBagian.FungsiBagianId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(fungsiBagian);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FungsiBagianExists(fungsiBagian.FungsiBagianId))
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
        return View(fungsiBagian);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var fungsiBagian = await _context.FungsiBagians
            .FirstOrDefaultAsync(m => m.FungsiBagianId == id);
        if (fungsiBagian == null)
        {
            return NotFound();
        }

        return View(fungsiBagian);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var fungsiBagian = await _context.FungsiBagians.FindAsync(id);
        _context.FungsiBagians.Remove(fungsiBagian);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool FungsiBagianExists(int id)
    {
        return _context.FungsiBagians.Any(e => e.FungsiBagianId == id);
    }
}