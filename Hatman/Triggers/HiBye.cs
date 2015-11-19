using System;
using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    public class HiBye : ITrigger
    {
        public readonly string[] hiPhrases = new string[] { "Hi", "Heya", "Yo", "Sup" };



        public void AttachEvents(ChatEventRouter router)
        {
            router.RegisterTriggerEvent(EventType.UserEntered, this);
            //router.RegisterTriggerEvent(EventType.UserLeft, this);
        }

        public bool HandleEvent(object sender, ChatEventArgs e)
        {
            if (e.Type == EventType.UserEntered)
            {
                if (e.User.GetChatFriendlyUsername().ToLowerInvariant() == "kyll")
                {
                    e.Room.PostMessageFast("@Kyll Plop!");
                }
                else
                {
                    e.Room.PostMessageFast(string.Format("@{0} {1}", 
                        e.User.GetChatFriendlyUsername(), 
                        Extensions.PickRandom<string>(hiPhrases)));
                }

                e.Handled = true;
                return true;
            }

            return false;
        }
    }
}
