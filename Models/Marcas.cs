﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UspgPOS.Models
{
    public class Marcas
    {

        [Column("id")]
        public long? Id { get; set; }

        [StringLength(255)]
        [Required]
        [Column("nombre")]
        public string Nombre { get; set; }
    }
}