using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace ParserSitemap.Infrastructure
{
    public class Helpers 
    {
        public static string GetSiteMapUrl(string siteUrl) 
        {
            const string siteMapPath = @"/sitemap.xml";
            const string pattern = @"\/$";
            const string target = "";

            var regex = new Regex(pattern);
            var result = regex.Replace(siteUrl, target); 
            return result + siteMapPath;
        }

        public static TimeSpan CheckLoadTime(string url)  
        {
            var timer = new Stopwatch();
            var request = WebRequest.Create(url);
            TimeSpan responseTime;

            try
            {
                timer.Start();
                request.GetResponse();
                timer.Stop();
                responseTime = timer.Elapsed;
            }
            catch (Exception)
            {
                responseTime = TimeSpan.FromMilliseconds(0);
            }
            return responseTime;
        }
    }
}