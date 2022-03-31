#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zorro.WebApplication.Data;
using Zorro.WebApplication.Models;

namespace Zorro.WebApplication.Controllers
{
    public class BillPaysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillPaysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BillPays
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BillPay.Include(b => b.Account);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BillPays/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billPay = await _context.BillPay
                .Include(b => b.Account)
                .FirstOrDefaultAsync(m => m.BillPayID == id);
            if (billPay == null)
            {
                return NotFound();
            }

            return View(billPay);
        }

        // GET: BillPays/Create
        public IActionResult Create()
        {
            ViewData["AccountNumber"] = new SelectList(_context.Set<Account>(), "AccountNumber", "AccountNumber");
            return View();
        }

        // POST: BillPays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillPayID,AccountNumber,PayeeId,Amount,ScheduleTimeUtc,PaymentFrequency,BillState")] BillPay billPay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(billPay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountNumber"] = new SelectList(_context.Set<Account>(), "AccountNumber", "AccountNumber", billPay.AccountNumber);
            return View(billPay);
        }

        // GET: BillPays/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billPay = await _context.BillPay.FindAsync(id);
            if (billPay == null)
            {
                return NotFound();
            }
            ViewData["AccountNumber"] = new SelectList(_context.Set<Account>(), "AccountNumber", "AccountNumber", billPay.AccountNumber);
            return View(billPay);
        }

        // POST: BillPays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BillPayID,AccountNumber,PayeeId,Amount,ScheduleTimeUtc,PaymentFrequency,BillState")] BillPay billPay)
        {
            if (id != billPay.BillPayID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billPay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillPayExists(billPay.BillPayID))
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
            ViewData["AccountNumber"] = new SelectList(_context.Set<Account>(), "AccountNumber", "AccountNumber", billPay.AccountNumber);
            return View(billPay);
        }

        // GET: BillPays/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billPay = await _context.BillPay
                .Include(b => b.Account)
                .FirstOrDefaultAsync(m => m.BillPayID == id);
            if (billPay == null)
            {
                return NotFound();
            }

            return View(billPay);
        }

        // POST: BillPays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var billPay = await _context.BillPay.FindAsync(id);
            _context.BillPay.Remove(billPay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillPayExists(string id)
        {
            return _context.BillPay.Any(e => e.BillPayID == id);
        }
    }
}
