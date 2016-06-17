using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Blame : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^(blame\??|(wh(o|ich (user|(one )?(of us|here)))))", Extensions.RegOpts);
        private readonly string[] phrases = new[]
        {
            "{0}.",
            "{0}!",
            "{0}?",
            "*points finger at {0}*",
            "*looks at {0}*",
            "Blame {0}!",
            "It's definitely {0}.",
            "...{0}.",
            "I'm guessing it's {0}.",
            "Smells like {0}...",
            "It's {0}!",
            "Either {0} or {1}...",
            "*{0} and {1} both look suspicious...*",
            "It's {0}! Blame {0}! No, wait. It's {1}!",
            "{0} and {1}.",
            "{0} or {1}.",
            "Everyone in the room, except {0}.",
            "Everyone in the room, except {0} and {1}.",
            "Blame {0} and {1}!",
            "*{0} secretly thinks it's {1}*",
            "Jon Skeet",
            "Shog",
            "Everyone.",
            "No one.",
            "HIS NAME IS JOHHHHN CEEEEEEENA",
            "Your Mom",
            "Your Dad",
            "You",
            "Yo mama"
        };

        public Regex CommandPattern => ptn;

        public string Description => "Picks a user that caused the end of the world.";

        public string Usage => "Blame|Who [...]";



        public void ProcessMessage(Message msg, ref Room rm)
        {
            var users = rm.CurrentUsers;
            var userX = users.PickRandom().Name;
            var userY = users.PickRandom().Name;
            while (userX == userY)
            {
                userY = users.PickRandom().Name;
            }

            if (userX == rm.Me.Name)
            {
                userX = "me";
            }

            if (userY == rm.Me.Name)
            {
                userY = "me";
            }

            var message = string.Format(phrases.PickRandom(), userX, userY);

            rm.PostReplyLight(msg, message);
        }
    }
}
