using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using B08C14_InventoryManagement.Data;
using B08C14_InventoryManagement.Pagination;

namespace B08C14_InventoryManagement.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentSort"] = sortOrder;
            var applicationDbContext =await _context.Orders.Include(o => o.Customer).Include(o=>o.OrderDetails).ThenInclude(o=>o.Product).ToListAsync();
            ViewData["CurrentFilter"] = searchString;


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                applicationDbContext = applicationDbContext.Where(s => s.Customer.Name.ToLower().Contains(searchString.ToLower())
                                       ).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    applicationDbContext = applicationDbContext.OrderByDescending(s => s.Customer.Name).ToList();
                    break;
                case "Date":
                    applicationDbContext = applicationDbContext.OrderBy(s => s.OrderDate).ToList();
                    break;
                case "date_desc":
                    applicationDbContext = applicationDbContext.OrderByDescending(s => s.OrderDate).ToList();
                    break;
                default:
                    applicationDbContext = applicationDbContext.OrderBy(s => s.Customer.Name).ToList();
                    break;
            }
           // return View( applicationDbContext.ToList());
            int pageSize = 2;
            return View(await PaginatedList<Order>.CreateAsync(applicationDbContext, pageNumber ?? 1, pageSize));
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
            var order = new Order();
            ViewBag.ProductId = new SelectList(_context.Products.OrderBy(p => p.Name), "Id", "Name");
            order.OrderDetails = new List<OrderDetails> { 
                new OrderDetails() {ProductId=0,  Quantity=1, UnitPrice=0, TotalPrice=0  }
            };
            return View(order);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Order order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                string msg = "";
                foreach (var err in ModelState.Values) 
                {
                    foreach (var ms in err.Errors)
                    {
                        msg += $"{ms.ErrorMessage}\n";
                    }
                }
                ModelState.AddModelError("", msg);
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order =  _context.Orders.Include(o=>o.OrderDetails).Where(o=>o.Id.Equals(id)).FirstOrDefault();
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            ViewBag.ProductId = new SelectList(_context.Products.OrderBy(p => p.Name), "Id", "Name");
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( Order order)
        {
            

            if (ModelState.IsValid)
            {
                try
                {
                    order.UpdatedAt = DateTime.Now;
                    order.UpdatedBy = User.Identity.Name ?? "Default";
                    order.OrderDetails = order.OrderDetails;

                    var od = _context.OrderDetails.Where(o => o.OrderId.Equals(order.Id)).AsNoTracking();
                    //var rid= order.OrderDetails.Contains(od);
                    foreach (var item in od)
                    {
                        if (!order.OrderDetails.Any(o => o.Id == item.Id))
                        {
                            _context.OrderDetails.Remove(item);
                        }
                    }
                    _context.Update(order);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", e.InnerException.Message ?? e.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.Message ?? ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
