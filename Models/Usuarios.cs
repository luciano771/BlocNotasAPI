using System.ComponentModel.DataAnnotations;

namespace BlocNotasAPI.Models
{

    public class Usuarios
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string? NombreUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string? CorreoElectronico { get; set; }

        [Required]
        [StringLength(100)]
        public string Contraseña { get; set; }

         

    }
}
