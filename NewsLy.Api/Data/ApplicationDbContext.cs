using Microsoft.EntityFrameworkCore;
using NewsLy.Api.Models;

namespace NewsLy.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<ContactRequest> ContactRequests { get; set; }
    }
}