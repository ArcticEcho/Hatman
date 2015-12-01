using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    public class CopyTroll : ITrigger
    {
        private readonly Regex ptn = new Regex(@"(?i)^(un)?troll \w+$", Extensions.RegOpts);
        private List<string> users = new List<string>();

        public void AttachEvents(ChatEventRouter router) => router.RegisterTriggerEvent(EventType.MessagePosted, this);

        public bool HandleEvent(object sender, ChatEventArgs e)
        {
            if (e.Type == EventType.MessagePosted)
                ProcessMessage(e);

            return false;
        }

        public bool ProcessMessage(ChatEventArgs e)
        {
            var curMsg = e.Message.Content.ToLowerInvariant();

            if (curMsg.StartsWith("https"))
                curMsg = curMsg.Remove(4, 1);

            if (ptn.IsMatch(e.Message.Content))
            {
                var user = new string(e.Message.Content.ToLowerInvariant().Replace("troll", "").Where(c => !char.IsWhiteSpace(c)).ToArray());

                if (e.Message.Content.ToLowerInvariant().StartsWith("un"))
                {
                    for (var f = users.Count; f > 0; --f)
                    {
                        if (users[f].StartsWith(user))
                        {
                            users.RemoveAt(f);
                        }
                    }

                    return false;
                }

                var n = new byte[4];
                Extensions.RNG.GetBytes(n);
                var i = BitConverter.ToUInt32(n, 0);

                if (i % 3 == 0)
                {
                    if (!users.Contains(user))
                    {
                        users.Add(user);
                    }
                }

                return false;
            }

            if (users.Any(u => e.User.Name.ToLowerInvariant().StartsWith(u)))
            {
                e.Room.PostMessageFast(curMsg);
            }

            return false;
        }
    }
}
