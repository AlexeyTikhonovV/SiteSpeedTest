using Microsoft.EntityFrameworkCore;

namespace ParserSitemap.Models
{
    public class UrlContext : DbContext
    {
        public DbSet<UrlSite> UrlSites { get; set; } 
        public DbSet<DataUrl> DataUrls { get; set; }

        public UrlContext(DbContextOptions<UrlContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}