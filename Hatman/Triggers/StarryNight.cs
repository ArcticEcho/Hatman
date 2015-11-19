using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    class StarryNight : ITrigger
    {
        private bool outOfStars;
        private bool starryNight;



        public void AttachEvents(ChatEventRouter router) => router.RegisterTriggerEvent(EventType.MessagePosted, this);

        public bool HandleEvent(object sender, ChatEventArgs e)
        {
            if (e.Message.Content.ToLowerInvariant().Contains("starry night"))
            {
                starryNight = true;
            }

            if (starryNight && !outOfStars)
            {
                outOfStars = !e.Room.ToggleStar(e.Message);
            }

            return true;
        }
    }
}
