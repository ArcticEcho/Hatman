using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    interface ICommand
    {
        Regex CommandPattern { get; }
        string Description { get; }
        string Usage { get; }

        void ProcessMessage(Message msg, ref Room rm);
    }
}
