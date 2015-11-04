using System.Text.RegularExpressions;

namespace Hatman.Commands
{
    public class Cowbell : ICommand
    {
        private static readonly Regex pattern = new Regex("(?i:needs something)|(?i:needs more)", Extensions.RegOpts);

        public System.Text.RegularExpressions.Regex CommandPattern
        {
            get { return pattern; }
        }

        public string Description
        {
            get { return "Needs more cowbell"; }
        }

        public string Usage
        {
            get { return "[...]needs something[...]|[...]needs more[...]"; }
        }

        public void ProcessMessage(ChatExchangeDotNet.Message msg, ref ChatExchangeDotNet.Room rm)
        {
            rm.PostReplyFast(msg, "http://i.stack.imgur.com/3U9DQ.gif");
        }
    }

    public class Fever : ICommand
    {
        private static readonly Regex pattern = new Regex("(?i:fever)", Extensions.RegOpts);

        public System.Text.RegularExpressions.Regex CommandPattern
        {
            get { return pattern; }
        }

        public string Description
        {
            get { return "Gets advice from record producer Bruce Dickinson."; }
        }

        public string Usage
        {
            get { return "[...]fever[...]"; }
        }

        public void ProcessMessage(ChatExchangeDotNet.Message msg, ref ChatExchangeDotNet.Room rm)
        {
            rm.PostReplyFast(msg, "http://i.stack.imgur.com/edt7R.jpg");
        }
    }

}
