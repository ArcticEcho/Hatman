using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Should : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^(?!why|how|wh(e(n|re)|at)).*\?$", Extensions.RegOpts);
        private readonly string[] phrases = new[]
        {
            "42",
            "No.",
            "Yes.",
            "Yup",
            "Nope",
            "Indubitably.",
            "No. Never. Ever. *EVER*.",
            "I'll tell ya, only if I get my coffee.",
            "Nah.",
            "Try speaking to my face, it helps.",
            "Keyboard mashing is not a valid way to talk to people.",
            "Ask The Skeet.",
            "... do I look like I know everything?",
            "Ask me no questions, and I shall tell no lies.",
            "*I'm sorry, the person you have called is away. Please leave a message after the \"beep\"...*",
            "Yeah.",
            "Ofc.",
            "NOOOOOOOOOOOOOOOO",
            "Sure...",
            "If you seriously can't remember my previous answer, you need help.",
            "*sigh*",
            "Le sigh",
            "Hmm",
            "Ask {0}.",
            "Ugh, no more questions, please.",
            "*wants to say 42*",
            "Before I answer yet another dumb question, let me just mash my keyboardsreljkgbsveiluytwklvndlkjhbvkd",
            "Please, just give me 5 minutes of peace and quiet. Could you do that for me? Honestly, it's like I'm surrounded by children."
        };

        public Regex CommandPattern => ptn;

        public string Description =>"Decides whether or not something should happen.";

        public string Usage => "(sh|[wc])ould|will|did and many words alike";



        public void ProcessMessage(Message msg, ref Room rm)
        {
            var phrase = phrases.PickRandom();
            var users = rm.GetCurrentUsers();
            var user = users.PickRandom();

            while (user == rm.Me)
            {
                user = users.PickRandom();
            }

            rm.PostReplyFast(msg, string.Format(phrase, user));
        }
    }
}
