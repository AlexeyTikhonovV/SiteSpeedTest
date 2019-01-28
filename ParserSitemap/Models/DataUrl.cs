using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserSitemap.Models
{
    public class DataUrl
    {
        public int Id { get; set; }
        public string MapLink { get; set; }
        public double Speed { get; set; } 
        public DateTime Date { get; set; }

        public int UrlSiteId { get; set; }  
        public UrlSite UrlSite { get; set; } 
    }
}