using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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