using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatExchangeDotNet;
using System.Text.RegularExpressions;

namespace Hatman.Commands
{
    public class Xkcd : ICommand
    {
        private static readonly Regex pattern = new Regex("(?i:xkcd)", Extensions.RegOpts);
        private static readonly Regex commandParser = new Regex(@"\b(\d+?)\b");

        public System.Text.RegularExpressions.Regex CommandPattern
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
            int comicNumber = Extensions.PickRandom(Enumerable.Range(1, 1599));
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
