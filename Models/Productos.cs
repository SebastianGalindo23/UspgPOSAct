using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UspgPOS.Models
{
    public class Productos
    {
        [Column("id")]
        public long? Id { get; set; }

        [StringLength(255)]
        [Required]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required]
        [Column("precio")]
        public decimal Precio { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("marca_id")]
        public long MarcaId { get; set; }

        [Column("clasificacion_id")]
        public long ClasificacionesId { get; set; }

        public Marcas? Marca { get; set; }
        public Clasificaciones? Clasificaciones { get; set; }

    }
}
