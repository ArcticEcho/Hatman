using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    public interface ITrigger
    {
        /// <summary>
        /// Called to allow the trigger to register for its desired events.
        /// </summary>
        /// <param name="router"></param>
        void AttachEvents(ChatEventRouter router);
        /// <summary>
        /// Called when a subscribed event is handled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <remarks>
        /// Return true to indicate that the trigger should get first crack at handling the next event.
        /// Return false when done processing to return to the normal execution order.
        /// Set e.Handled = true to stop processing the event
        /// </remarks>
        bool HandleEvent(object sender, ChatEventArgs e);
    }
}
