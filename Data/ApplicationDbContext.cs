using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PracticaSupervisada.Models;

namespace PracticaSupervisada.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Asistencia> Asistencias { get; set; }
    public DbSet<Bidones> Bidones { get; set; }
}
