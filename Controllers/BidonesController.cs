using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
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

        // GET: Bidones
        public async Task<IActionResult> Index(int? Busqanio, int? Busqmes, int? pageNumber, int pageSize = 3)
        {

            IQueryable<Bidones> bidones = _context.Bidones.OrderByDescending(e => e.Id).Take(800);

            if (Busqanio.HasValue && Busqanio!= 0)
            {
                bidones = bidones.Where(e => e.Fecha.Year == Busqanio);
            }
            if (Busqmes.HasValue && Busqmes!= 0)
            {
                bidones = bidones.Where(e => e.Fecha.Month == Busqmes);
            }

            int totalBidones = await bidones.CountAsync();

            int currentPage = pageNumber ?? 1;
            var pagedList = await bidones.Skip((currentPage - 1) * pageSize)
                                                      .Take(pageSize)
                                                      .ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalBidones / pageSize);

            ViewBag.Busqanio = Busqanio;
            ViewBag.Busqmes = Busqmes;

            return View(pagedList);
        }
        [HttpPost]
        public async Task<IActionResult> ConsultarBidonesEntregados(int mes, int anio)
        {
            var bidonesEntregados = CalcularBidonesEntregados(mes, anio);
            ViewBag.BidonesEntregados = bidonesEntregados;
            ViewBag.MesSeleccionado = mes;
            ViewBag.AnioSeleccionado = anio;

            IQueryable<Bidones> bidones = _context.Bidones.OrderByDescending(e => e.Id).Take(800);
            int totalBidones = await _context.Bidones.CountAsync();
            int currentPage = 1;
            int pageSize = 3;
            var pagedList = await bidones.Skip((currentPage - 1) * pageSize)
                                              .Take(pageSize)
                                              .ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalBidones / pageSize);

            return View("Index", pagedList);
        }

        private int CalcularBidonesEntregados(int mes, int anio)
        {
            var bidones = _context.Bidones
                .Where(a => a.Fecha.Year == anio && a.Fecha.Month == mes)
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

            var bidones = await _context.Bidones.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,Cantidad,Observaciones")] Bidones bidones)
        {
            if (id != bidones.Id)
            {
                return NotFound();
            }

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
