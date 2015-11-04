using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class DontHurtMe : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^don'?t hurt me\.?$", Extensions.RegOpts);

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
                return "Don't hurt me[.]";
            }
        }

        public void ProcessMessage(Message msg, ref Room rm)
        {
            rm.PostReplyFast(msg, "No more.");
        }
    }
}
