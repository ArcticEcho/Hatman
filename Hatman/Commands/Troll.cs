using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Troll : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^troll \w+$", Extensions.RegOpts);
        private readonly Regex tinyUrl = new Regex(@"<b>(http://tinyurl.*)</b>", Extensions.RegOpts);
        private readonly string[] links = new[]
        {
            "http://www.angelfire.com/super/badwebs/",
            "http://nyanit.com/nyanit.com/",
            "https://www.youtube.com/watch?v=BROWqjuTM0g",
            "https://www.youtube.com/watch?v=gvYfRiJQIX8",
            "http://jwars.weebly.com/index.html",
            "http://justgottrolled.com/",
            "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
        };
        private readonly string[] phrases = new[]
        {
            "If you say so...",
            "No way.",
            "Fine.",
            "Sure thing.",
            "Nope",
            "Alright",
            "All right",
            "That'll be fun >:D",
            "Yes master.",
            "Nuh uh"
        };
        private readonly string[] hellos = new[]
        {
            "Hi",
            "Heya",
            "Yo",
            "Sup",
            "Plop",
            "Hiya",
            "Sup",
            "What's up?",
            "Morning!",
            "Afternoon!",
            "Night!"
        };

        public Regex CommandPattern => ptn;

        public string Description => " Here, have some more.";

        public string Usage => "Words go here";



        public void ProcessMessage(Message msg, ref Room rm)
        {
            var n = new byte[4];
            Extensions.RNG.GetBytes(n);
            var i = BitConverter.ToUInt32(n, 0);
            var ping = "@" + new string(msg.Content.ToLowerInvariant().Replace("troll", "").Where(c => !char.IsWhiteSpace(c)).ToArray());


            if (i % 2 == 0)
            {
                var a = "";
                var b = new byte[1];

                for (var j = 0; j < 20; j++)
                {
                    Extensions.RNG.GetBytes(b);
                    a += b[0] % 10;
                }

                var url = Uri.EscapeDataString(links.PickRandom());
                var shortUrl = tinyUrl.Match(new WebClient().DownloadString($"http://tinyurl.com/create.php?source=indexpage&url={url}&submit=Make+TinyURL%21&alias={a}")).Groups[1].Value;
                var outMsg = $"{ping} [{phrases.PickRandom()}]({shortUrl})";

                rm.PostMessageFast(outMsg);
            }
            else
            {
                var r = rm;
                Task.Run(() =>
                {
                    for (var x = 0; x < 5; x++)
                    {
                        Thread.Sleep(10000);
                        r.PostMessageFast(ping + " " + hellos.PickRandom());
                    }
                });
            }
        }
    }
}
