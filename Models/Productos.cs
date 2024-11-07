using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UspgPOS.Models
{
    public class Productos
    {
        [Column("id")]
        public long? Id { get; set; }

        [Column("codigo")]
        [MaxLength(100)]
        public string? Codigo { get; set; }

        [StringLength(255)]
        [Required]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required]
        [Column("precio")]
        [Range(0.01, 100000)]
        public decimal Precio { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("marca_id")]
        public long MarcaId { get; set; }

        [Column("clasificacion_id")]
        public long ClasificacionesId { get; set; }

        [Column("img_url")]
        public string? ImgUrl { get; set; }

        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        public Marcas? Marca { get; set; }
        public Clasificaciones? Clasificaciones { get; set; }

    }
}
