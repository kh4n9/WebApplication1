using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AplicationUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AplicationUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.AplicationUser.ToListAsync());
        }

        /*// GET: AplicationUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aplicationUser = await _context.AplicationUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aplicationUser == null)
            {
                return NotFound();
            }

            return View(aplicationUser);
        }

        // GET: AplicationUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AplicationUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Email,PasswordHash,Name")] AplicationUser aplicationUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aplicationUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aplicationUser);
        }

        // GET: AplicationUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aplicationUser = await _context.AplicationUser.FindAsync(id);
            if (aplicationUser == null)
            {
                return NotFound();
            }
            return View(aplicationUser);
        }

        // POST: AplicationUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Email,PasswordHash,Name")] AplicationUser aplicationUser)
        {
            if (id != aplicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aplicationUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AplicationUserExists(aplicationUser.Id))
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
            return View(aplicationUser);
        }

        // GET: AplicationUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aplicationUser = await _context.AplicationUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aplicationUser == null)
            {
                return NotFound();
            }

            return View(aplicationUser);
        }

        // POST: AplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aplicationUser = await _context.AplicationUser.FindAsync(id);
            if (aplicationUser != null)
            {
                _context.AplicationUser.Remove(aplicationUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AplicationUserExists(int id)
        {
            return _context.AplicationUser.Any(e => e.Id == id);
        }*/
    }
}
