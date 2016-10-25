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
        }

        public bool HandleEvent(object sender, ChatEventArgs e)
        {
            if (e.Type == EventType.UserEntered && !e.User.IsMod)
            {
                var b = new byte[4];
                Extensions.RNG.GetBytes(b);

                if (BitConverter.ToUInt32(b, 0) % 2 == 0 && e.User.ID == 4174897)
                {
                    e.Room.PostMessageLight("@Kyll Plop!");
                }
                else
                {
                    e.Room.PostMessageLight(string.Format("@{0} {1}", 
                        e.User.Name.Replace(" ", ""), 
                        Extensions.PickRandom<string>(hiPhrases)));
                }

                e.Handled = true;
                return true;
            }

            return false;
        }
    }
}
