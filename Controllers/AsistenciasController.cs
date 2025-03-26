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
using X.PagedList.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PracticaSupervisada.Controllers
{
    public class AsistenciasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AsistenciasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Asistencias
        public async Task<IActionResult> Index(string? Busqnombre, int? Busqanio, int? Busqmes, int? pageNumber, int pageSize = 3)
        {
            IQueryable<Asistencia> asistencias = _context.Asistencias.OrderByDescending(e => e.Id).Take(400);
            if (Busqnombre != null)
            {
                asistencias = asistencias.Where(e => e.Nombre_Apellido.Contains(Busqnombre));
            }
            if (Busqanio.HasValue && Busqanio.Value != 0)
            {
                asistencias = asistencias.Where(e => e.Fecha.Year == Busqanio);
            }
            if (Busqmes.HasValue && Busqmes != 0)
            {
                asistencias = asistencias.Where(e => e.Fecha.Month == Busqmes);
            }


            int totalAsistencias = await asistencias.CountAsync();

            int currentPage = pageNumber ?? 1;
            var pagedList = await asistencias.Skip((currentPage - 1) * pageSize)
                                                      .Take(pageSize)
                                                      .ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalAsistencias / pageSize);

            ViewBag.Busqanio = Busqanio; 
            ViewBag.Busqmes = Busqmes;
            ViewBag.Busqnombre = Busqnombre;

            return View(pagedList);
        }
        [HttpPost]
        public async Task<IActionResult> ConsultarHorasTrabajadas(int mes, int anio)
        {
            var horasTrabajadas = CalcularHorasTrabajadas(mes, anio);
            ViewBag.HorasTrabajadas = horasTrabajadas;
            ViewBag.MesSeleccionado = mes;
            ViewBag.AnioSeleccionado = anio;

            IQueryable<Asistencia> asistencias = _context.Asistencias.OrderByDescending(e => e.Id).Take(400);
            int totalAsistencias = await _context.Asistencias.CountAsync();
            int currentPage = 1; 
            int pageSize = 3; 
            var pagedList = await asistencias.Skip((currentPage - 1) * pageSize)
                                              .Take(pageSize)
                                              .ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalAsistencias / pageSize);

            return View("Index", pagedList);
        }

        private TimeOnly CalcularHorasTrabajadas(int mes, int anio)
        {
            var asistencias = _context.Asistencias
                .Where(a => a.Fecha.Year == anio && a.Fecha.Month == mes)
                .ToList();

            var horasTrabajadas = asistencias
                .Sum(a => (a.Tiempo_Salida - a.Tiempo_Entrada).TotalHours);

            TimeSpan timeSpan = TimeSpan.FromHours(horasTrabajadas);

            TimeOnly timeOnly = TimeOnly.FromTimeSpan(timeSpan);

            return timeOnly;
        }
    

        // GET: Asistencias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // GET: Asistencias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Asistencias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre_Apellido,Fecha,Tiempo_Entrada,Tiempo_Salida")] Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asistencia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(asistencia);
        }

        // GET: Asistencias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencias.FindAsync(id);
            if (asistencia == null)
            {
                return NotFound();
            }
            return View(asistencia);
        }

        // POST: Asistencias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre_Apellido,Fecha,Tiempo_Entrada,Tiempo_Salida")] Asistencia asistencia)
        {
            if (id != asistencia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asistencia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsistenciaExists(asistencia.Id))
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
            return View(asistencia);
        }

        // GET: Asistencias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // POST: Asistencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asistencia = await _context.Asistencias.FindAsync(id);
            if (asistencia != null)
            {
                _context.Asistencias.Remove(asistencia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsistenciaExists(int id)
        {
            return _context.Asistencias.Any(e => e.Id == id);
        }
    }
}
