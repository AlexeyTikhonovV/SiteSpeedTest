using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParserSitemap.Infrastructure;
using ParserSitemap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ParserSitemap.Controllers
{
    public class HomeController : Controller
    {
        private UrlContext db;
        private double t;
        const string Urlset = "urlset";
        const string Sitemapindex = "sitemapindex";

        public HomeController(UrlContext context)
        {
            db = context;
        }

        public IActionResult Index(UrlSite urlSites, DataUrl dataUrl, string siteUrl) 
        {      
            var xmlDoc = new XmlDocument();
            var time = new List<double>();

            if (siteUrl == null) return View();
            string url = Helpers.GetSiteMapUrl(siteUrl);

            try
            {
                xmlDoc.Load(url);
            }
            catch (Exception)
            {
                return BadRequest("Sorry, but we can't get the site map for this web site.");
            }

            HttpContext.Session.SetString("Url", siteUrl);

            urlSites.SiteUrl = siteUrl;
            urlSites.Date = DateTime.Now;
            db.UrlSites.Add(urlSites);
            db.SaveChanges();

            Parser(urlSites, xmlDoc, time);

            return RedirectToAction("Result");
        }

        public IActionResult Result(int? id)      
        {
            var url = HttpContext.Session.GetString("Url");

            if (id != null)
            {
                var data = db.DataUrls
                    .Where(s => s.UrlSiteId == id);

                var height = data.Count() * 5 + 20;
                ViewBag.h = height;

                return View(data);
            }

            if (url != null)
            {
                var sort = db.UrlSites
                    .Select(s => s.Id).LastOrDefault();

                var data = db.DataUrls
                    .Where(s => s.UrlSiteId == sort);

                var height = data.Count() * 5 + 20;
                ViewBag.h = height;

                return View(data);
            }

            return View();
        }

        public IActionResult History() 
        {
            var distinctPeople = db.UrlSites
                .GroupBy(p => p.SiteUrl)
                .Select(g => g.FirstOrDefault())
                .OrderByDescending(h => h.Date)
                .ToList();

            return View(distinctPeople);
        }

        public IActionResult Details(string siteUrl, int type)    
        {
            if (siteUrl != null)
            {
                HttpContext.Session.SetString("SiteUrl", siteUrl);
            }

            var currentUrl = HttpContext.Session.GetString("SiteUrl"); 

            switch (type)
            {
                case 0:
                {
                    var allTime = db.UrlSites
                        .Where(x => x.SiteUrl == currentUrl)
                        .OrderByDescending(h => h.Date);

                    return View(allTime);
                }
                case 1:
                    t = 0;
                    break;
                case 2:
                    t = -6;
                    break;              
            }

            var dt = DateTime.Now.Date.AddDays(t);

            var result = db.UrlSites
                .Where(x => x.Date >= dt && x.SiteUrl == currentUrl)
                .OrderByDescending(h => h.Date); 

            return View(result);
        }

        private void Parser(UrlSite urlSites, XmlDocument xmlDoc, List<double> time)
        {
            foreach (XmlNode topNode in xmlDoc.ChildNodes)
            {
                if (topNode.Name.ToLower() != Urlset && topNode.Name.ToLower() != Sitemapindex) continue;
                var xml = new XmlNamespaceManager(xmlDoc.NameTable);
                xml.AddNamespace("ns", topNode.NamespaceURI);

                foreach (XmlNode urlNode in topNode.ChildNodes)
                {
                    var locNode = urlNode.SelectSingleNode("ns:loc", xml); 
                    if (locNode != null)
                    {
                        var timeSpan = Helpers.CheckLoadTime(locNode.InnerText);
                        time.Add(timeSpan.TotalMilliseconds);
                        var responseTime = timeSpan.TotalMilliseconds;

                        var add = new DataUrl
                        {
                            MapLink = locNode.InnerText,
                            Speed = responseTime,
                            Date = DateTime.Now,
                            UrlSiteId = urlSites.Id
                        };
                        db.DataUrls.Add(add);
                    }
                }
                db.SaveChanges();
            }
        }
    }
}