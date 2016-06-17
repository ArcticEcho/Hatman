using System.Text.RegularExpressions;

namespace Hatman.Commands
{
    public class Cowbell : ICommand
    {
        private static readonly Regex pattern = new Regex("(?i:needs something)|(?i:needs more)", Extensions.RegOpts);

        public Regex CommandPattern => pattern;

        public string Description => "Needs more cowbell";

        public string Usage => "needs something|needs more";

        public void ProcessMessage(ChatExchangeDotNet.Message msg, ref ChatExchangeDotNet.Room rm) =>
            rm.PostReplyLight(msg, "http://i.stack.imgur.com/3U9DQ.gif");
    }

    public class Fever : ICommand
    {
        private static readonly Regex pattern = new Regex("(?i:fever)", Extensions.RegOpts);

        public Regex CommandPattern => pattern;

        public string Description => "Gets advice from record producer Bruce Dickinson.";

        public string Usage => "[...]fever[...]";

        public void ProcessMessage(ChatExchangeDotNet.Message msg, ref ChatExchangeDotNet.Room rm) =>
            rm.PostReplyLight(msg, "http://i.stack.imgur.com/edt7R.jpg");
    }
}
