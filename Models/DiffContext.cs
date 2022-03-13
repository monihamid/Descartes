using Microsoft.EntityFrameworkCore;

namespace DiffingAPI.Models
{
    public class DiffContext: DbContext
    {
        public DiffContext(DbContextOptions<DiffContext> options)
            : base(options)
        {
        }
        public DbSet<Diff> Diff { get; set; }
        
    }
}