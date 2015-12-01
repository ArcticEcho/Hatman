using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Hat : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)\bhats?\b", Extensions.RegOpts);
        private readonly HashSet<string> hats;

        public Regex CommandPattern => ptn;

        public string Description => "Hats, hats, hats, hats, more hats with a side order of hats.";

        public string Usage => "Hat";



        public Hat()
        {
            hats = new GoogleImg("silly hats").GetPicUrls();
            var moreHats = new GoogleImg("funny hats").GetPicUrls();
            foreach (var hat in moreHats)
            {
                hats.Add(hat);
            }

            hats.Add("http://i.stack.imgur.com/I8zdQ.jpg");
        }



        public void ProcessMessage(Message msg, ref Room rm)
        {
            var hat = "";

            while (string.IsNullOrWhiteSpace(hat))
            {
                try
                {
                    var url = hats.PickRandom();
                    new WebClient().DownloadData(url);
                    hat = url;
                }
                catch
                {
                    hats.Remove(hat);
                    hat = null;
                }
            }

            rm.PostReplyFast(msg, hat);
        }
    }
}
