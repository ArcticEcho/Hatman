using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Hatman
{
    class GoogleImg
    {
        private readonly Regex picUrl = new Regex("(?i)imgres\\?imgurl=https?://(.(?!http))*?\\.(png|jpg|gif)", Extensions.RegOpts);
        private readonly Regex wordOfTheDay = new Regex("word-and-pronunciation\">\\s+<h1>(\\S+)</h1>", Extensions.RegOpts);
        private readonly string srchTrms;



        public GoogleImg(string searchTerms, bool addWordOfTheDay = true)
        {
            srchTrms = $"\"{searchTerms}\" {(addWordOfTheDay ? $" {GetWordOfTheDay()}" : "")}";
        }



        public HashSet<string> GetPicUrls()
        {
            var w = new WebClient();
            w.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.80 Safari/537.36");
            var html = w.DownloadString("http://www.google.com/search?tbm=isch&safe=strict&q=" + Uri.EscapeUriString(srchTrms));
            var ms = picUrl.Matches(html);
            var urls = new HashSet<string>();

            foreach (Match m in ms)
            {
                if (m.Value.Length < 10 || m.Value.Length > 300 || urls.Count > 4) continue;

                urls.Add(m.Value.Remove(0, 14));
            }

            return urls;
        }

        private string GetWordOfTheDay()
        {
            var html = new WebClient().DownloadString("http://www.merriam-webster.com/word-of-the-day");
            return wordOfTheDay.Match(html).Groups[1].Value;
        }
    }
}
