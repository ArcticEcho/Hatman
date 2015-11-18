using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class How : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^how|why", Extensions.RegOpts);
        private readonly string[] phrases = new[]
        {
            "42",
            "There's only one possible explanation. Waffles.",
            "I'm too tired atm, go ask {0}.",
            "Well, you see. The Unicorns are to blame here.",
            "No idea.",
            "I dunno.",
            "Haven't a clue.",
            "Magic.",
            "The butterflies stole my keyboard. Can't answer that.",
            "{0} knows. (I kinda don't.)"
        };

        public Regex CommandPattern
        {
            get
            {
                return ptn;
            }
        }

        public string Description
        {
            get
            {
                return "\"Why did that happen?\" You ask. Well, I probably know the answer.";
            }
        }

        public string Usage
        {
            get
            {
                return "how|what|why [...]";
            }
        }

        public void ProcessMessage(Message msg, ref Room rm)
        {
            var users = rm.GetCurrentUsers();
            var usr = users.PickRandom();

            while (usr == rm.Me)
            {
                usr = users.PickRandom();
            }

            var returnMsg = string.Format(phrases.PickRandom(), usr);

            rm.PostReplyFast(msg, returnMsg);
        }
    }
}
