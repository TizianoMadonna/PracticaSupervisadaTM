using System.ComponentModel.DataAnnotations;

namespace PracticaSupervisada.Models
{
    public class Asistencia
    {
        public int Id { get; set; }
        [Display(Name = "Nombre y apellido")]
        [Required(ErrorMessage = "El nombre y apellido es requerido")]
        public string Nombre_Apellido { get; set; }
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "La fecha es requerida")]
        public DateOnly Fecha { get; set; }
        [Display(Name = "Hora de entrada")]
        [Required(ErrorMessage = "La hora de entrada es requerida")]
        public TimeOnly Tiempo_Entrada { get; set; }
        [Display(Name = "Hora de salida")]
        [Required(ErrorMessage = "La hora de salida es requerida")]
        public TimeOnly Tiempo_Salida { get; set; }
    }
}
