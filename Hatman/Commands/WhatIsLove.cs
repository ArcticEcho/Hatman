using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class WhatIsLove : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^what is love\??$", Extensions.RegOpts);

        public Regex CommandPattern
        {
            get
            {
                return ptn;
            }
        }

        public string Description
        {
            get
            {
                return "Sing along!";
            }
        }

        public string Usage
        {
            get
            {
                return "What is love[?]";
            }
        }

        public void ProcessMessage(Message msg, ref Room rm)
        {
            rm.PostReplyFast(msg, "Baby don't hurt me.");
        }
    }
}
