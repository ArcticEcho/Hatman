using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Panic : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)panic[!.1]*?", Extensions.RegOpts);
        private readonly string[] pics = new[]
        {
            "http://rack.0.mshcdn.com/media/ZgkyMDEzLzA2LzE4LzdjL0JlYWtlci4zOWJhOC5naWYKcAl0aHVtYgkxMjAweDk2MDA-/4a93e3c4/4a4/Beaker.gif",
            "http://i1094.photobucket.com/albums/i442/PeetaEverdeen/OHSHITRUNAROUND.gif",
            "http://rack.0.mshcdn.com/media/ZgkyMDEzLzA2LzE4L2I2L0pvaG5ueURlcHBwLmM1YjNkLmdpZgpwCXRodW1iCTEyMDB4OTYwMD4/70417de1/fe5/Johnny-Depp-panics.gif",
            "http://tech.graze.com/content/images/2014/Apr/colbert-panic.gif",
            "http://media.giphy.com/media/HZs7JJYJ6rdqo/giphy.gif",
            "http://38.media.tumblr.com/2d95777547966a733ccdfb3e34afaacc/tumblr_nnirepYsSe1rouu9to1_250.gif"
        };

        public Regex CommandPattern => ptn;

        public string Description => "EVERYTHING IS UNDER CONTROL";

        public string Usage =>"panic!1!1!!!!11!!!";



        public void ProcessMessage(Message msg, ref Room rm) => rm.PostReplyFast(msg, pics.PickRandom());
    }
}
