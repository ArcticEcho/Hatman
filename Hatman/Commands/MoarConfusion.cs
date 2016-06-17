using System;
using System.Net;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;
using ChatterBotAPI;

namespace Hatman.Commands
{
    class MoarConfusion : ICommand
    {
        private static readonly ChatterBotFactory factory = new ChatterBotFactory();
        private static readonly ChatterBot bot = factory.Create(ChatterBotType.CLEVERBOT);
        private readonly Regex badMessages = new Regex(@"(?i)\bapp\b|\.com|clever\w+", Extensions.RegOpts);
        private readonly Regex reply = new Regex(@"^:\d+\s", Extensions.RegOpts);
        private readonly Regex ping = new Regex(@"(?i)@hat\w*", Extensions.RegOpts);
        private readonly Regex tinyUrl = new Regex(@"<b>(http://tinyurl.*)</b>", Extensions.RegOpts);
        private readonly Regex ptn = new Regex("");
        private readonly string fkey;
        private ChatterBotSession botSession = bot.CreateSession();
        private DateTime lastTry;
        private bool quotaReached;



        public MoarConfusion()
        {
            // Yay, HTML paring with regex.
            var html = new WebClient().DownloadString("http://chat.stackoverflow.com");
            fkey = Regex.Match(html, "(?i)id=\"fkey\".*value=\"([a-z0-9]+)\"").Value;
        }



        public Regex CommandPattern => ptn;

        public string Description => @"ͭ̎̈̄ͥ̓ͮ͂͑̌̾̈̒̔͐͛̍̚҉҉̢̮̭̣̫̩̝̪̲̮̩̥̜̮͖͉͓̭͚̼
̴̢̛̛̤̤̗̻̲̙͓̦̻̹̲͇̰̣̙̟̥̓ͦͥ̏͂́";

        public string Usage => "";



        public void ProcessMessage(Message msg, ref Room rm)
        {
            if (!quotaReached || (quotaReached && (DateTime.UtcNow - lastTry).TotalHours > 1))
            {
                var message = GetMessageNewMethod(msg);
                rm.PostReplyLight(msg, message);
            }
        }

        private string GetMessageNewMethod(Message msg)
        {
            lastTry = DateTime.UtcNow;
            string message = null;

            try
            {
                while (true)
                {
                    message = WebUtility.HtmlDecode(botSession.Think(msg.Content));

                    if (badMessages.IsMatch(message))
                    {
                        Console.WriteLine("Skipped thought: " + message);
                    }
                    else
                    {
                        quotaReached = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("403"))
                {
                    Console.WriteLine("API quota reached.");
                    quotaReached = true;
                }
                else
                {
                    Console.WriteLine(ex);
                }
                return null;
            }

            return message;
        }
    }
}
