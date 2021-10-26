using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsLy.Api.Models;

namespace NewsLy.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<ContactRequest> ContactRequests { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<MailingList> MailingLists { get; set; }


        public DbSet<TrackingUrl> TrackingUrls { get; set; }
    }
}