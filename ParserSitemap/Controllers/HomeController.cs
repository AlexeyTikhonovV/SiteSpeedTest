using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParserSitemap.Infrastructure;
using ParserSitemap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace ParserSitemap.Controllers
{
    public class HomeController : Controller
    {
        private UrlContext db;
        private double filter; 
        private string ModUrl { get; set; }
        const string pattern = @"^\w+\:(?!\/)\#*.*$";
        const string pattern1 = @"\s*#.+";
        const string pattern2 = @"\/$";

        public HomeController(UrlContext context)
        {
            db = context;
        }

        public IActionResult Index(UrlSite urlSites, DataUrl dataUrl, string siteUrl)
        {
            if (siteUrl == null) return View();

            var regex = new Regex(pattern2);
            var remove = regex.Replace(siteUrl, string.Empty);
            HttpContext.Session.SetString("Url", remove);

            urlSites.SiteUrl = remove;
            urlSites.Date = DateTime.Now;
            db.UrlSites.Add(urlSites);
            db.SaveChanges();

            string[] urls = BuildSitemap(urlSites);
            RequestsUrl(urlSites, urls);

            return RedirectToAction("Result");
        }

        public IActionResult Result(int? id)      
        {
            var url = HttpContext.Session.GetString("Url");

            if (id != null)
            {
                var data = db.DataUrls 
                    .Where(s => s.UrlSiteId == id)
                    .GroupBy(p => p.MapLink)
                    .Select(g => g.FirstOrDefault());
                
                var height = data.Count() * 5 + 20;
                ViewBag.h = height;

                return View(data);
            }

            if (url != null)
            {
                var sort = db.UrlSites
                    .Select(s => s.Id).LastOrDefault();

                var data = db.DataUrls
                    .Where(s => s.UrlSiteId == sort)
                    .GroupBy(p => p.MapLink)
                    .Select(g => g.FirstOrDefault());

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
                    filter = 0;
                    break;
                case 2:
                    filter = -6;
                    break;              
            }

            var datetime = DateTime.Now.Date.AddDays(filter); 

            var result = db.UrlSites
                .Where(x => x.Date >= datetime && x.SiteUrl == currentUrl)
                .OrderByDescending(h => h.Date); 

            return View(result);
        }
        #region Helpers
        private void ExtractAllAHrefTags(UrlSite urlSites, string urlToCheck, List<string> allurls)
        {
            var currentUrl = HttpContext.Session.GetString("Url");
            var regex = new Regex(pattern);
            var regex1 = new Regex(pattern1); 

            var pageContent = Requests.LoadPage(urlToCheck); 
            if (pageContent != null)
            {
                var document = new HtmlDocument();
                document.LoadHtml(pageContent);

                try
                {
                    foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
                    {
                        HtmlAttribute att = link.Attributes["href"];
                        MatchCollection matches = regex.Matches(att.Value);

                        if (matches.Count <= 0)
                        {
                            var remove = regex1.Replace(att.Value, string.Empty);
                            ModUrl = Helpers.GetSiteMapUrl(remove, currentUrl); 
                            if (!allurls.Contains(ModUrl))
                            {
                                allurls.Add(ModUrl);
                            }
                        }
                    }
                }
                catch
                {
                    allurls.Add(string.Empty);
                }              
            }
        }

        private string[] BuildSitemap(UrlSite urlSites)
        {
            var currentUrl = HttpContext.Session.GetString("Url");
            List<string> allurls = new List<string>();
            allurls.Add(currentUrl);
            for (int i = 0; i < allurls.Count ; i++)
            {
                if(allurls.Count <= 300)
                {
                    string urlToCheck = allurls[i];
                    ExtractAllAHrefTags(urlSites, urlToCheck, allurls);
                }
            }
            return allurls.ToArray();
        }

        private void RequestsUrl(UrlSite urlSites, string[] urls)
        {
            foreach (string url in urls)
            {
                var timeSpan = Requests.CheckLoadTime(url);
                SaveUrl(urlSites, url, timeSpan);
            }
        }

        private void SaveUrl(UrlSite urlSites, string RelativeToAbsolute, TimeSpan timeSpan)
        {
            if (RelativeToAbsolute != "")
            {
                var responseTime = timeSpan.TotalMilliseconds;

                var add = new DataUrl
                {
                    MapLink = RelativeToAbsolute,
                    Speed = responseTime,
                    Date = DateTime.Now,
                    UrlSiteId = urlSites.Id
                };
                db.DataUrls.Add(add);
                db.SaveChanges();
            }
        }
        #endregion 
    }
}