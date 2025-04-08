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
        [Authorize]
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

            var horasTrabajadas = CalcularHorasTrabajadas(Busqmes.Value, Busqanio.Value);

            IQueryable<Asistencia> asistencias = _context.Asistencias.OrderByDescending(e => e.Id)
                                                                     .Where(a => a.UserEmail == userEmail);

            if (Busqanio.Value != 0)
            {
                asistencias = asistencias.Where(e => e.Fecha.Year == Busqanio);
            }
            if (Busqmes.Value != 0)
            {
                asistencias = asistencias.Where(e => e.Fecha.Month == Busqmes);
            }

            var listaAsistencias = await asistencias.ToListAsync();

            ViewBag.HorasTrabajadas = horasTrabajadas;
            ViewBag.MesSeleccionado = Busqmes;
            ViewBag.AnioSeleccionado = Busqanio;
            ViewBag.Busqanio = Busqanio;
            ViewBag.Busqmes = Busqmes;

            return View(listaAsistencias);
        }
        [HttpPost]
        public async Task<IActionResult> ConsultarHorasTrabajadas(int mes, int anio, string? Busqnombre, int? Busqanio, int? Busqmes)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var horasTrabajadas = CalcularHorasTrabajadas(mes, anio);
            ViewBag.HorasTrabajadas = horasTrabajadas;
            ViewBag.MesSeleccionado = mes;
            ViewBag.AnioSeleccionado = anio;

            IQueryable<Asistencia> asistencias = _context.Asistencias.OrderByDescending(e => e.Id)  
                                                                     .Where(a => a.UserEmail == userEmail);

            if (Busqanio.HasValue && Busqanio.Value != 0)
            {
                asistencias = asistencias.Where(e => e.Fecha.Year == Busqanio);
            }
            if (Busqmes.HasValue && Busqmes != 0)
            {
                asistencias = asistencias.Where(e => e.Fecha.Month == Busqmes);
            }

            var listaAsistencias = await asistencias.ToListAsync();

            ViewBag.Busqanio = Busqanio;
            ViewBag.Busqmes = Busqmes;
            ViewBag.Busqnombre = Busqnombre;

            return View("Index", listaAsistencias);
        }

        private TimeSpan CalcularHorasTrabajadas(int mes, int anio)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var asistencias = _context.Asistencias
                                .Where(a => a.Fecha.Year == anio && a.Fecha.Month == mes)
                                .Where(a => a.UserEmail == userEmail)
                                .ToList();

            var horasTrabajo = asistencias
                .Sum(a => (a.Tiempo_Salida.Ticks - a.Tiempo_Entrada.Ticks));


            if (horasTrabajo > TimeSpan.MaxValue.Ticks)
            {
                horasTrabajo = TimeSpan.MaxValue.Ticks;
            }


            TimeSpan timeSpan = TimeSpan.FromTicks(horasTrabajo);

            return timeSpan;
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
        [Authorize]
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
                var userEmail = User.FindFirstValue(ClaimTypes.Email);

                asistencia.UserEmail = userEmail;

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
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var asistencia = await _context.Asistencias.FindAsync(id);
            asistencia.UserEmail = userEmail;

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre_Apellido,Fecha,Tiempo_Entrada,Tiempo_Salida, UserEmail")] Asistencia asistencia)
        {
            if (id != asistencia.Id)
            {
                return NotFound();
            }
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            asistencia.UserEmail = userEmail;

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
