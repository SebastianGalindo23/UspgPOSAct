using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UspgPOS.Models
{
    public class Detalles_Venta
    {
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("precio_unitario")]
        public decimal Precio { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("Producto_Id")]
        public long Productoid {get; set; }

        [Column("venta_Id")]
        public long Ventaid { get; set; }
        public Productos? Producto { get; set; }

        public Venta? Venta { get; set; }
    }
}
