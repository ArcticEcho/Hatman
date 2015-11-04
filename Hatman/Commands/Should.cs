using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Should : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)could|should|would|can|will", Extensions.RegOpts);
        private readonly string[] phrases = new[]
        {
            "No.",
            "Yes.",
            "Yup.",
            "Nope.",
            "Indubitably",
            "Never. Ever. *EVER*.",
            "Only if I get my coffee.",
            "Nah.",
            "Ask The Skeet.",
            "... do I look like I know everything?",
            "Ask me no questions, and I shall tell no lies.",
            "Sure, when it rains imaginary internet points.",
            "Yeah.",
            "Ofc.",
            "NOOOOOOOOOOOOOOOO",
            "Sure, ok."
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
                return "(sh|[wc])ould|will|can";
            }
        }

        public void ProcessMessage(Message msg, ref Room rm)
        {
            rm.PostReplyFast(msg, phrases.PickRandom());
        }
    }
}
