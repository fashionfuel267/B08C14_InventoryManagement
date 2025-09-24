using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using B08C14_InventoryManagement.Data;

namespace B08C14_InventoryManagement.Controllers
{
    public class CategotiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategotiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categoties
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categoties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoty = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoty == null)
            {
                return NotFound();
            }

            return View(categoty);
        }

        // GET: Categoties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categoties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Categoty categoty)
        {
            if (ModelState.IsValid)
            {
                categoty.CreatedAt = DateTime.Now;
                categoty.CreatedBy = User.Identity.Name??"Default";
                categoty.IsActive = true;
                _context.Add(categoty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                string msg = "";
                foreach(var err in ModelState.Values)
                {
                 foreach(var ms in err.Errors)
                    {
                        msg += $"{ms.ErrorMessage}\n";
                    }
                }
                ModelState.AddModelError("", msg);
            }
            return View(categoty);
        }

        // GET: Categoties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoty = await _context.Categories.FindAsync(id);
            if (categoty == null)
            {
                return NotFound();
            }
            return View(categoty);
        }

        // POST: Categoties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Id,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy,IsActive")] Categoty categoty)
        {
            if (id != categoty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategotyExists(categoty.Id))
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
            return View(categoty);
        }

        // GET: Categoties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoty = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoty == null)
            {
                return NotFound();
            }

            return View(categoty);
        }

        // POST: Categoties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoty = await _context.Categories.FindAsync(id);
            if (categoty != null)
            {
                _context.Categories.Remove(categoty);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategotyExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
