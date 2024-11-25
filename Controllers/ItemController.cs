// Controllers/ItemController.cs
using Microsoft.AspNetCore.Mvc;
using InventoryGudang.Data;
using InventoryGudang.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
public class ItemController : Controller
{
    private readonly ApplicationDbContext _context;

    public ItemController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Index()
    {
        var items = await _context.Items.Include(i => i.Supplier).ToListAsync();
        return View(items);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name");
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Item item)
    {
        if (ModelState.IsValid)
        {
            // Set default values for non-nullable properties if they are not set
            item.TransactionType = item.TransactionType ?? "DefaultTransactionType";
            item.Supplier = _context.Suppliers.FirstOrDefault(s => s.SupplierId == item.SupplierId);

            _context.Add(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", item.SupplierId);
        return View(item);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", item.SupplierId);
        return View(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Item item)
    {
        if (id != item.ItemId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Set default values for non-nullable properties if they are not set
                item.TransactionType = item.TransactionType ?? "DefaultTransactionType";
                item.Supplier = _context.Suppliers.FirstOrDefault(s => s.SupplierId == item.SupplierId);

                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(item.ItemId))
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
        ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", item.SupplierId);
        return View(item);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var item = await _context.Items
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(m => m.ItemId == id);
        if (item == null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Items.FindAsync(id);
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ItemExists(int id)
    {
        return _context.Items.Any(e => e.ItemId == id);
    }
}