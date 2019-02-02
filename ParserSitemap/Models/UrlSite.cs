using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ParserSitemap.Models
{
    public enum TimeOfDay
    {
        [Display(Name = "Day")]
        Day = 1,
        [Display(Name = "Week")]
        Week = 2,
        [Display(Name = "All time")]
        AllTime = 0
    }

    public class UrlSite  
    {
        public int Id { get; set; }
        public string SiteUrl { get; set; }
        public DateTime Date { get; set; }

        public List<DataUrl> DataUrls { get; set; } 
    }
}