using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlocNotasAPI.Models
{
    
    public class Notas
    {
        [Key]
        public int NotaId { get; set; }
        public int NroNota { get; set; }
        public int UsuarioId { get; set; }
        [JsonIgnore]
        [ForeignKey("UsuarioId")]
        public Usuarios? Usuarios { get; set; }

        [StringLength(1000)]
        public string Titulo { get; set; }
        [StringLength(500)]
        public string Contenido { get; set; }

        [Column(TypeName = "date")] // Esto especifica que el campo en la base de datos es de tipo "date"
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; }


        [StringLength(200)]
        public string? Etiquetas { get; set; }
         
    }
}
