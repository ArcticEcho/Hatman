using System;
using System.Globalization;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class When : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^when", Extensions.RegOpts);
        private readonly Random r = new Random(DateTime.UtcNow.Millisecond);
        private readonly string[] phrases = new[]
        {
            "Tomorrow.",
            "Yesterday.",
            "Within a week.",
            "Within a month.",
            "Next year.",
            "In 3... 2... 1...",
            "Yes.",
            "In 6 to 8 moons.",
            "When ---Shog--- the Unicorns say so.",
            "Whenever I finally get my waffles.",
            "Never.",
            "When hell freezes over.",
            "Soon™.",
            "When Jon Skeet stops making rep.",
            "When you finish the time machine."
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
                return "Randomly selects a date (handy for knowing when to get more coffee).";
            }
        }

        public string Usage
        {
            get
            {
                return "When [...]";
            }
        }

        public void ProcessMessage(Message msg, ref Room rm)
        {
            var n = new byte[4];
            Extensions.RNG.GetBytes(n);
            var message = "";

            if (BitConverter.ToUInt32(n, 0) % 100 > 50)
            {
                var lwBound = -3652;
                if (msg.Content.ToLowerInvariant().StartsWith("when will"))
                {
                    lwBound = 0;
                }

                // Pick any date within 10 years from now.
                var date = DateTime.UtcNow.Add(TimeSpan.FromDays(r.Next(lwBound, 3652)));
                message = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else
            {
                message = phrases.PickRandom();
            }

            rm.PostReplyFast(msg, message);
        }
    }
}
