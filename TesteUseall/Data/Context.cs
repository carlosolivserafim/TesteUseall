using Microsoft.EntityFrameworkCore;
using TesteUseall.Models;

namespace TesteUseall.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }

        public DbSet<Cliente> Cliente { get; set; }
    }
}