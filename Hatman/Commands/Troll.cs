using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Troll : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^troll \w+$", Extensions.RegOpts);
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

        public Regex CommandPattern => ptn;

        public string Description => " Here, have some more.";

        public string Usage => "Words go here";



        public void ProcessMessage(Message msg, ref Room rm)
        {
            var ping = "@" + new string(msg.Content.ToLowerInvariant().Replace("troll", "").Where(c => !char.IsWhiteSpace(c)).ToArray());
            var a = "";
            var b = new byte[1];

            for (var i = 0; i < 20; i++)
            {
                Extensions.RNG.GetBytes(b);
                a += b[0] % 10;
            }

            var url = links.PickRandom();
            var shortUrl = new WebClient().DownloadString($"http://tinyurl.com/create.php?source=indexpage&url{url}&submit=Make+TinyURL%21&alias={a}");
            var outMsg = $"{ping} [{phrases.PickRandom()}]({shortUrl})";

            rm.PostMessageFast(outMsg);
        }
    }
}
