using System;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Dice : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^d(?![io])", Extensions.RegOpts);
        private readonly string[] errorPhrases = new[]
        {
            "https://i.imgflip.com/ucj8n.jpg",
            "Nope",
            "Wrong again...",
            "Try typing with your hands, I hear it helps.",
            "*now ignoring {0}*",
            "I know you're just testing me."
        };

        public Regex CommandPattern => ptn;

        public string Description => "Throws a die.";

        public string Usage => "d<number>";



        public void ProcessMessage(Message msg, ref Room rm)
        {
            var up = msg.Content.Remove(0, 1);
            ulong upInt = 0;

            if (!ulong.TryParse(up, out upInt) || upInt == 0)
            {
                rm.PostReplyFast(msg, string.Format(errorPhrases.PickRandom(), msg.Author.Name));
                return;
            }

            var nBytes = new byte[8];
            Extensions.RNG.GetBytes(nBytes);
            var n = (BitConverter.ToUInt64(nBytes, 0) % upInt) + 1;

            if (n % 25 == 0)
            {
                rm.PostReplyFast(msg, "http://imgs.xkcd.com/comics/random_number.png");
                return;
            }

            if (upInt == 100)
            {
                rm.PostReplyFast(msg, n == 1 ? "**CRITICAL SUCCESS**" : n == 100 ? "**CRITICAL FAILURE**" : n.ToString());
            }
            else
            {
                rm.PostReplyFast(msg, n);
            }
        }
    }
}
