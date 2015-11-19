using System;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    class WhatIsLove : ITrigger
    {
        private readonly Regex ptn = new Regex(@"(?i)^what is love\??$", Extensions.RegOpts);



        public void AttachEvents(ChatEventRouter router)
        {
            router.RegisterTriggerEvent(EventType.MessagePosted, this);
        }

        public bool HandleEvent(object sender, ChatEventArgs e)
        {
            if (ptn.IsMatch(e.Message.Content))
            {
                var n = new byte[4];
                Extensions.RNG.GetBytes(n);

                if (BitConverter.ToUInt32(n, 0) % 10 == 0)
                {
                    e.Room.PostMessageFast("https://media.giphy.com/media/12mgpZe6brh2nu/giphy.gif");
                }
                else
                {
                    e.Room.PostMessageFast("Baby don't hurt me.");
                }

                return true;
            }

            return false;
        }
    }

    class DontHurtMe : ITrigger
    {
        private readonly Regex ptn = new Regex(@"(?i)^don'?t hurt me\.?$", Extensions.RegOpts);



        public void AttachEvents(ChatEventRouter router)
        {
            router.RegisterTriggerEvent(EventType.MessagePosted, this);
        }

        public bool HandleEvent(object sender, ChatEventArgs e)
        {
            if (ptn.IsMatch(e.Message.Content))
            {
                e.Room.PostMessageFast("No more.");
                return true;
            }

            return false;
        }
    }
}
