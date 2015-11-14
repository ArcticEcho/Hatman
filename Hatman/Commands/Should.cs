using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Should : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^((?<!why|how|wh(e(n|re)|at)).)*\?", Extensions.RegOpts);
        private readonly string[] phrases = new[]
        {
            "No.",
            "Yes.",
            "Yup",
            "Nope",
            "Indubitably.",
            "Never. Ever. *EVER*.",
            "I'll tell ya, only if I get my coffee.",
            "Nah.",
            "Ask The Skeet.",
            "... do I look like I know everything?",
            "Ask me no questions, and I shall tell no lies.",
            "*I'm sorry, the person you have called is away. Please leave a message after the \"beep\"...*",
            "Yeah.",
            "Ofc.",
            "NOOOOOOOOOOOOOOOO",
            "Sure..."
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
                return "Decides whether or not something should happen.";
            }
        }

        public string Usage
        {
            get
            {
                return "(sh|[wc])ould|will|did and many words alike";
            }
        }

        public void ProcessMessage(Message msg, ref Room rm)
        {
            rm.PostReplyFast(msg, phrases.PickRandom());
        }
    }
}
