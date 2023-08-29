using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using BlocNotasAPI.Models;
namespace BlocNotasAPI.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }
 

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Notas> Notas { get; set; }
      
    }
}
