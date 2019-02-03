using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace ParserSitemap.Infrastructure
{
    public class Requests 
    {
        public static TimeSpan CheckLoadTime(string url)
        {
            TimeSpan responseTime;

            try
            {
                var timer = new Stopwatch();
                var request = WebRequest.Create(url);

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

        public static string LoadPage(string url)
        {
            try
            {
                var result = "";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "[request]";
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var receiveStream = response.GetResponseStream();
                    if (receiveStream != null)
                    {
                        StreamReader readStream;
                        if (response.CharacterSet == null)
                            readStream = new StreamReader(receiveStream);
                        else
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                        result = readStream.ReadToEnd();
                        readStream.Close();
                    }
                    response.Close();
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}