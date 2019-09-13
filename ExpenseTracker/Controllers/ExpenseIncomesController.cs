using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class ExpenseIncomesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseIncomesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExpenseIncomes
        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 5;
            var expenseIncomes = _context.ExpensesIncomes.Where(x=>x.UserId==User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(await PaginatedList<ExpenseIncome>.CreateAsync(expenseIncomes,pageNumber ?? 1,pageSize));
        }

        // GET: ExpenseIncomes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseIncome = await _context.ExpensesIncomes
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expenseIncome == null)
            {
                return NotFound();
            }

            return View(expenseIncome);
        }

        // GET: ExpenseIncomes/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreateIncome()
        {
            return View();
        }

        // POST: ExpenseIncomes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Amount,Description,Tag,EntryDate,IsIncome")] ExpenseIncome expenseIncome)
        {
            if (ModelState.IsValid)
            {
                expenseIncome.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(expenseIncome);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", expenseIncome.UserId);
            return View(expenseIncome);
        }

        // GET: ExpenseIncomes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseIncome = await _context.ExpensesIncomes.FindAsync(id);
            if (expenseIncome == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", expenseIncome.UserId);
            return View(expenseIncome);
        }

        // POST: ExpenseIncomes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,Description,Tag,EntryDate,IsIncome,UserId")] ExpenseIncome expenseIncome)
        {
            if (id != expenseIncome.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expenseIncome);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseIncomeExists(expenseIncome.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", expenseIncome.UserId);
            return View(expenseIncome);
        }

        // GET: ExpenseIncomes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseIncome = await _context.ExpensesIncomes
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expenseIncome == null)
            {
                return NotFound();
            }

            return View(expenseIncome);
        }

        // POST: ExpenseIncomes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expenseIncome = await _context.ExpensesIncomes.FindAsync(id);
            _context.ExpensesIncomes.Remove(expenseIncome);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseIncomeExists(int id)
        {
            return _context.ExpensesIncomes.Any(e => e.Id == id);
        }
    }
}
