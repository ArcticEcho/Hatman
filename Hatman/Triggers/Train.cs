using System;
using System.Security.Cryptography;
using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    public class Train : ITrigger
    {
        private readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private string lastMsg = "";
        private string lastPostedMessage = "";

        public void AttachEvents(ChatEventRouter router)
        {
            router.RegisterTriggerEvent(EventType.MessagePosted, this);
        }
        
        public bool HandleEvent(object sender, ChatEventArgs e)
        {
            if (e.Type == EventType.MessagePosted)
            {
                ProcessMessage(e);
            }

            return false;
        }

        public bool ProcessMessage(ChatEventArgs e)
        {
            var curMsg = e.Message.Content.ToLowerInvariant();

            if (curMsg.StartsWith("https"))
            {
                curMsg = curMsg.Remove(4, 1);
            }

            if (curMsg == lastMsg && curMsg != lastPostedMessage)
            {
                var n = new byte[4];
                rng.GetBytes(n);

                if (BitConverter.ToUInt32(n, 0) % 10 == 0)
                {
                    rng.GetBytes(n);

                    if (BitConverter.ToUInt32(n, 0) % 10 > 4)
                    {
                        e.Room.PostMessageFast("C-C-C-COMBO BREAKER");
                    }
                    else
                    {
                        e.Room.PostMessageFast("https://s3.amazonaws.com/img.ultrasignup.com/events/raw/6a76f4a3-4ad2-4ae2-8a3b-c092e85586af.jpg");
                    }
                }
                else
                {
                    e.Room.PostMessageFast(e.Message.Content);
                }

                e.Handled = true;

                lastPostedMessage = lastMsg;
            }

            lastMsg = curMsg;
            return false;
        }

    }
}
