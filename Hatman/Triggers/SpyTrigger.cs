using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    public class SpyTrigger : ITrigger
    {
        // right now not really anything useful.

        public void AttachEvents(ChatEventRouter router)
        {
            router.RegisterTriggerEvent(EventType.MessagePosted, this);
        }


        Regex plusCommand = new Regex("(?i:plus)", Extensions.RegOpts);
        Regex minusCommand = new Regex("(?i:minus)", Extensions.RegOpts);

        int currentState = 0;

        public bool HandleEvent(object sender, ChatEventArgs e)
        {
            if (plusCommand.IsMatch(e.Message.Content))
            {
                e.Room.PostReplyFast(e.Message, String.Format("Count: {0}", ++currentState));
            }
            else if (minusCommand.IsMatch(e.Message.Content))
            {
                e.Room.PostReplyFast(e.Message, String.Format("Count: {0}", --currentState));
            }
            
            return false;
        }
    }
}
