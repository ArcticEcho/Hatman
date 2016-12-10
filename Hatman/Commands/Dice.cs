using System;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Dice : ICommand
    {
        private readonly Regex ptn = new Regex(@"^(\d*)[dD](\d+)$", Extensions.RegOpts);
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

        public string Usage => "<optional number>d<number>";



        public void ProcessMessage(Message msg, ref Room rm)
        {
            var match = ptn.Match(msg.Content);
            if(!match.Success)
            {
                /* technically shold be dead code becuase we know the expression matched */
                rm.PostReplyLight(msg, string.Format(errorPhrases.PickRandom(), msg.Author.Name));
                return;
            }
            
            string diceCountStr = match.Groups[1].Value;
            ulong diceCount = 0;
            if (diceCountStr == "")
            {
                diceCount = 1;
            }
            else if (!ulong.TryParse(diceCountStr, out diceCount) || diceCount == 0)
            {
                rm.PostReplyLight(msg, string.Format(errorPhrases.PickRandom(), msg.Author.Name));
                return;
            }
            
            string edgeCountStr = match.Groups[2].Value;
            ulong edgeCount = 0;
            if (!ulong.TryParse(edgeCountStr, out edgeCount) || edgeCount == 0)
            {
                rm.PostReplyLight(msg, string.Format(errorPhrases.PickRandom(), msg.Author.Name));
                return;
            }
            
            /* Protection against too large values. */
            /* Do we need it? */
            if (diceCount > 1000)
            {
                rm.PostReplyLight(msg, "I don't have that many dices");
                return;
            }

            var nBytes = new byte[8 * diceCount];
            Extensions.RNG.GetBytes(nBytes);

            long sum = 0;
            for (int i = 0; i < diceCount; ++i)
            {
                sum += (BitConverter.ToUInt64(nBytes, 8 * i) % edgeCount) + 1;
            }
            
            rm.PostReplyLight(msg, sum);
        }
    }
}
