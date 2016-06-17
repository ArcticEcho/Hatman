using System;
using System.Collections.Generic;
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

        public Regex CommandPattern => pattern;

        public string Description => "Gets an XKCD comic."; 

        public string Usage => "xkcd [###]";



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
            rm.PostReplyLight(msg, string.Format("http://www.xkcd.com/{0}/", comicNumber));
        }
    }

    public class Comic : ICommand
    {
        private readonly Regex pattern = new Regex("(?i:comic)", Extensions.RegOpts);

        
        Random rng = new Random();
        Dictionary<string, string> knownComics = new Dictionary<string, string>();

        public Comic()
        {
            knownComics.Add("pearls", "pearlsbeforeswine");
            knownComics.Add("garfield", "garfield");
            knownComics.Add("foxtrot", "foxtrot");
            knownComics.Add("getfuzzy", "getfuzzy");
        }

        public Regex CommandPattern => pattern;

        public string Description =>  "Gets a Comic.";

        public string Usage => "comic [name]|comic add [shortcut] [name]|comic remove [shortcut]|comic list";

        public void ProcessMessage(Message msg, ref Room rm)
        {
            string response = "";

            string[] commandParts = msg.Content.ToLowerInvariant().Replace("comic", "").Trim().Split(' ');
            if (commandParts.Length == 0)
            {
                string selectedComic = knownComics.Values.PickRandom();
                response = GetComic(selectedComic);
            }
            else if (commandParts[0].ToLowerInvariant().Trim() == "add")
            {
                if (commandParts.Length < 3)
                {
                    response = "Not enough args";
                }
                else
                {
                    knownComics.Add(commandParts[1].Trim(), commandParts[2].Trim());
                    response = "Ok.";
                }
            }
            else if (commandParts[0].ToLowerInvariant().Trim() == "remove")
            {
                if (commandParts.Length < 2)
                {
                    response = "Not enough args";
                }
                else if (!knownComics.ContainsKey(commandParts[1].Trim()))
                {
                    response = "Not found.";
                }
                else
                {
                    knownComics.Remove(commandParts[1].Trim());
                    response = "Done.";
                }
            }
            else if (commandParts[0].ToLowerInvariant().Trim() == "list")
            {
                response = string.Join(", ", knownComics.Keys);   
            }
            else
            {
                if (!knownComics.ContainsKey(commandParts[0]))
                {
                    response = "Unknown comic";
                }
                else
                {
                    response = GetComic(knownComics[commandParts[0]]);
                    if (response == null)
                    {
                        response = "Comic was unsupported, and has been terminated as such.";
                        knownComics.Remove(commandParts[0]);
                    }
                }
            }

            rm.PostReplyLight(msg, response);
        }

        private string GetComic(string selectedComic)
        {
            try
            {
                WebClient client = new WebClient();

                DateTime comicDate = DateTime.Now;
                int days = rng.Next(365 * 8); // Looks like there are 10 years available, but there are some gaps beyond 8.

                comicDate = comicDate.Subtract(new TimeSpan(days, 0, 0, 0));

                string comicURL = string.Format(@"http://www.gocomics.com/{0}/{1}/{2}/{3}", selectedComic, comicDate.Year, comicDate.Month, comicDate.Day);
                string content = client.DownloadString(comicURL);

                // :D
                Regex htmlParser = new Regex("(?<=src=\")([^\"]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                int index = content.IndexOf("class=\"strip\"");

                if (index < 0) return null;

                return htmlParser.Match(content, index).Value + ".gif";
            }
            catch
            {
                return null;
            }
        }
    }
}
