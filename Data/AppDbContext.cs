using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UspgPOS.Models;

namespace UspgPOS.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Marcas> Marcas { get; set; }
        public DbSet<Clasificaciones> Clasificaciones { get; set; }
        public DbSet<Detalles_Venta> Detalles_Venta { get; set; }
        public DbSet<Productos> Productos { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        
    }
}
