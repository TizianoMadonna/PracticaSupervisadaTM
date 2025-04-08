using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PracticaSupervisada.Models
{
    public class Bidones
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha")]
        public DateOnly Fecha { get; set; }
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Display(Name = "Cantidad de Bidones")]
        public int Cantidad { get; set; }
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        public string? UserEmail { get; set; }

    }
}