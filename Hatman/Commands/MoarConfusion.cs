using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;
using ServiceStack.Text;

namespace Hatman.Commands
{
    class MoarConfusion : ICommand
    {
        private readonly Regex reply = new Regex(@"^:\d+\s", Extensions.RegOpts);
        private readonly Regex ping = new Regex(@"(?i)@hat\w*", Extensions.RegOpts);
        private readonly Regex tinyUrl = new Regex(@"<b>(http://tinyurl.*)</b>", Extensions.RegOpts);
        private readonly Regex ptn = new Regex("");
        private readonly string fkey;



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

                if (BitConverter.ToUInt32(k, 0) % 5 == 0)
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
                var jsonStr = Encoding.UTF8.GetString(new WebClient().UploadValues($"http://chat.stackoverflow.com/chats/{rm.ID}/events", new NameValueCollection
                {
                    { "since", "0" },
                    { "mode", "Messages" },
                    { "msgCount", "150" },
                    { "fkey", fkey }
                }));

                var msgIDs = new HashSet<string>();
                var json = JsonSerializer.DeserializeFromString<Dictionary<string, Dictionary<string, object>[]>>(jsonStr);

                foreach (var m in json["events"])
                {
                    msgIDs.Add((string)m["message_id"]);
                }

                message = ping.Replace(reply.Replace(WebUtility.HtmlDecode(new WebClient().DownloadString($"http://chat.stackoverflow.com/message/{msgIDs.PickRandom()}?plain=true")), ""), "");
            }

            rm.PostReplyFast(msg, message);
        }
    }
}
