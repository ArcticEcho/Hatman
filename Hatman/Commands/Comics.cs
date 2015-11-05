using System;
using System.Linq;
using ChatExchangeDotNet;
using System.Text.RegularExpressions;
using System.Net;

namespace Hatman.Commands
{
    public class Xkcd : ICommand
    {
        private readonly Regex pattern = new Regex("(?i:xkcd)", Extensions.RegOpts);
        private readonly Regex commandParser = new Regex(@"\b(\d+?)\b", Extensions.RegOpts);
        private readonly Regex latestComicIdParser = new Regex(@"(?<=http://xkcd\.com/)\d+", Extensions.RegOpts);
        private DateTime lastFetch = DateTime.MinValue;
        private int latestComicId;

        public Regex CommandPattern
        {
            get { return pattern; }
        }

        public string Description
        {
            get { return "Gets an XKCD comic."; }
        }

        public string Usage
        {
            get { return "xkcd [###]"; }
        }

        public void ProcessMessage(Message msg, ref Room rm)
        {
            if ((DateTime.UtcNow - lastFetch).TotalHours > 1)
            {
                var html = new WebClient().DownloadString("http://xkcd.com/");
                var strId = latestComicIdParser.Match(html).Value;

                if (!int.TryParse(strId, out latestComicId))
                {
                    latestComicId = 1599;
                }

                lastFetch = DateTime.UtcNow;
            }

            int comicNumber = Extensions.PickRandom(Enumerable.Range(1, latestComicId));
            if (commandParser.IsMatch(msg.Content))
            {
                try
                {
                    comicNumber = int.Parse(commandParser.Match(msg.Content).Value);
                }
                catch { /* Laziest way to do this ever. Why validate parameters when you can just do it. */ }
            }
            rm.PostReplyFast(msg, String.Format("http://www.xkcd.com/{0}/", comicNumber));
        }
    }
}
