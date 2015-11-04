using System.Diagnostics;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Update : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^(update|pull|fetch)", Extensions.RegOpts);
        private readonly AutoUpdater au;

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
                return "Updates the bot with steaming fresh code.";
            }
        }

        public string Usage
        {
            get
            {
                return "Update|pull|fetch";
            }
        }



        public Update() { }

        public Update(string apiToken)
        {
            if (string.IsNullOrWhiteSpace(apiToken))
            {
                return;
            }

            au = new AutoUpdater(apiToken);
        }



        public void ProcessMessage(Message msg, ref Room rm)
        {
            if (au == null) { return; }

            rm.PostReplyFast(msg, "Updating, one sec...");

            if (!au.Update())
            {
                rm.PostReplyFast(msg, "I'm already up to date.");
            }
            else
            {
                rm.PostReplyFast(msg, "Update successful, starting new version...");

                au.StartNewVersion();

                Process.GetCurrentProcess().CloseMainWindow();
            }
        }
    }
}
