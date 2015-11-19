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
                msgIDs.Add((string)m["message_id"]);

            var message = ping.Replace(reply.Replace(WebUtility.HtmlDecode(new WebClient().DownloadString($"http://chat.stackoverflow.com/message/{msgIDs.PickRandom()}?plain=true")), ""), "");

            rm.PostReplyFast(msg, message);
        }
    }
}
