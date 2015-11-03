using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatExchangeDotNet;

namespace Hatman.Triggers
{
    interface ITrigger
    {
        void ProcessMessage(Message msg, ref Room rm);
    }
}
