using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    interface ITrigger
    {
        void ProcessMessage(Message msg, ref Room rm);
    }
}
