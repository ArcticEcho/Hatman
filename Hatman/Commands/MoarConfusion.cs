using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;
using ChatterBotAPI;
using ServiceStack.Text;

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
            string message = null;

            if (!quotaReached || (quotaReached && (DateTime.UtcNow - lastTry).TotalHours > 1))
            {
                message = GetMessageNewMethod(msg);
            }

            if (message == null)
            {
                message = GetMessageOldMethod(msg, rm.ID);
            }

            rm.PostReplyFast(msg, message);
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

        private string GetMessageOldMethod(Message msg, int roomID)
        {
            var n = new byte[4];
            var message = "";
            Extensions.RNG.GetBytes(n);
            var i = BitConverter.ToUInt32(n, 0);

            if (i % 10 == 0 && !string.IsNullOrWhiteSpace(msg.Content))
            {
                var b = new byte[1];
                var a = "";
                for (var j = 0; j < 20; j++)
                {
                    Extensions.RNG.GetBytes(b);
                    a += b[0] % 10;
                }

                var urlData = "";
                var k = new byte[4];
                Extensions.RNG.GetBytes(k);

                if (BitConverter.ToUInt32(k, 0) % 10 == 0)
                {
                    urlData = new WebClient().DownloadString($"http://tinyurl.com/create.php?source=indexpage&url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DdQw4w9WgXcQ&submit=Make+TinyURL%21&alias={a}");
                }
                else
                {
                    var url = "http://lmgtfy.com/?q=" + Uri.EscapeDataString(ping.Replace(reply.Replace(msg.Content, ""), "").Trim());
                    urlData = new WebClient().DownloadString($"http://tinyurl.com/create.php?source=indexpage&url={url}&submit=Make+TinyURL%21&alias={a}");
                }

                message = tinyUrl.Match(urlData).Groups[1].Value;
            }
            else
            {
                var jsonStr = Encoding.UTF8.GetString(new WebClient().UploadValues($"http://chat.stackoverflow.com/chats/{roomID}/events", new NameValueCollection
                {
                    { "since", "0" },
                    { "mode", "Messages" },
                    { "msgCount", "150" },
                    { "fkey", fkey }
                }));

                var msgIDs = new HashSet<int>();
                var json = JsonSerializer.DeserializeFromString<Dictionary<string, Dictionary<string, object>[]>>(jsonStr);

                foreach (var m in json["events"])
                {
                    var id = -1;

                    if (int.TryParse((string)m["message_id"], out id))
                    {
                        msgIDs.Add(id);
                    }
                }

                message = Message.GetMessageContent(msg.Host, msgIDs.PickRandom());
            }

            return message;
        }
    }
}
