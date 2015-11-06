using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    public interface ITrigger
    {
        void AttachEvents(ChatEventRouter router);
        bool HandleEvent(object sender, ChatEventArgs e);
    }
}
