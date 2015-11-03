using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    public class Train : ITrigger
    {
        private string lastMsg = "";
        private string lastPostedMessage = "";



        public void ProcessMessage(Message msg, ref Room rm)
        {
            var curMsg = msg.Content.ToLowerInvariant();

            if (curMsg == lastMsg && curMsg != lastPostedMessage)
            {
                rm.PostMessageFast(lastMsg);
                lastPostedMessage = lastMsg;
            }

            lastMsg = msg.Content.ToLowerInvariant();
        }
    }
}
