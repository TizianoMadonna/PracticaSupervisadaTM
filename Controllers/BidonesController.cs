using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PracticaSupervisada.Data;
using PracticaSupervisada.Models;

namespace PracticaSupervisada.Controllers
{
    public class BidonesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BidonesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Bidones
        public async Task<IActionResult> Index(int? Busqanio, int? Busqmes, int? pageNumber, int pageSize = 10)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (!Busqanio.HasValue)
            {
                Busqanio = DateTime.Now.Year;
            }
            if (!Busqmes.HasValue)
            {
                Busqmes = DateTime.Now.Month;
            }

            var bidonesEntregados = CalcularBidonesEntregados(Busqmes.Value, Busqanio.Value);

            IQueryable<Bidones> bidones = _context.Bidones.OrderByDescending(e => e.Fecha)
                                                          .Where(a => a.UserEmail == userEmail);


            if (Busqanio.Value != 0)
            {
                bidones = bidones.Where(e => e.Fecha.Year == Busqanio);
            }
            if (Busqmes.Value != 0)
            {
                bidones = bidones.Where(e => e.Fecha.Month == Busqmes);
            }

            var listaBidones = await bidones.ToListAsync();

            ViewBag.BidonesEntregados = bidonesEntregados;
            ViewBag.MesSeleccionado = Busqmes;
            ViewBag.AnioSeleccionado = Busqanio;
            ViewBag.Busqanio = Busqanio;
            ViewBag.Busqmes = Busqmes;

            return View(listaBidones);
        }
        [HttpPost]
        public async Task<IActionResult> ConsultarBidonesEntregados(int mes, int anio, int? Busqanio, int? Busqmes)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var bidonesEntregados = CalcularBidonesEntregados(mes, anio);
            ViewBag.BidonesEntregados = bidonesEntregados;
            ViewBag.MesSeleccionado = mes;
            ViewBag.AnioSeleccionado = anio;

            IQueryable<Bidones> bidones = _context.Bidones.OrderByDescending(e => e.Id)
                                                          .Where(a => a.UserEmail == userEmail);


            int totalBidones = await _context.Bidones.CountAsync();
            int currentPage = 1;
            int pageSize = 3;
            var pagedList = await bidones.Skip((currentPage - 1) * pageSize)
                                              .Take(pageSize)
                                              .ToListAsync();

            if (Busqanio.HasValue && Busqanio.Value != 0)
            {
                bidones = bidones.Where(e => e.Fecha.Year == Busqanio);
            }
            if (Busqmes.HasValue && Busqmes != 0)
            {
                bidones = bidones.Where(e => e.Fecha.Month == Busqmes);
            }

            var listaBidones = await bidones.ToListAsync();


            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalBidones / pageSize);

            return View("Index", listaBidones);
        }

        private int CalcularBidonesEntregados(int mes, int anio)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var bidones = _context.Bidones
                .Where(a => a.Fecha.Year == anio && a.Fecha.Month == mes)
                .Where(a => a.UserEmail == userEmail)
                .ToList();

            var bidonesEntregados = bidones
                .Sum(a => (a.Cantidad));

            return bidonesEntregados;
        }

        // GET: Bidones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bidones = await _context.Bidones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bidones == null)
            {
                return NotFound();
            }

            return View(bidones);
        }

        // GET: Bidones/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bidones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fecha,Cantidad,Observaciones")] Bidones bidones)
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);

                bidones.UserEmail = userEmail;

                _context.Add(bidones);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bidones);
        }

        // GET: Bidones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var bidones = await _context.Bidones.FindAsync(id);
            bidones.UserEmail = userEmail;

            if (bidones == null)
            {
                return NotFound();
            }
            return View(bidones);
        }

        // POST: Bidones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,Cantidad,Observaciones,UserEmail")] Bidones bidones)
        {
            if (id != bidones.Id)
            {
                return NotFound();
            }
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            bidones.UserEmail = userEmail;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bidones);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BidonesExists(bidones.Id))
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
            return View(bidones);
        }

        // GET: Bidones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bidones = await _context.Bidones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bidones == null)
            {
                return NotFound();
            }

            return View(bidones);
        }

        // POST: Bidones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bidones = await _context.Bidones.FindAsync(id);
            if (bidones != null)
            {
                _context.Bidones.Remove(bidones);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BidonesExists(int id)
        {
            return _context.Bidones.Any(e => e.Id == id);
        }
    }
}
