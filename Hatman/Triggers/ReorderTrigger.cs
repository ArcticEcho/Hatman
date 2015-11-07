using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;
using Hatman.Commands;

namespace Hatman.Triggers
{
    public class ReorderTrigger : ITrigger
    {
        ChatEventRouter router;
        private User activeUser;

        private static readonly Regex startReorderCmdPattern = new Regex("(?i:reorder)", Extensions.RegOpts);
        private static readonly Regex doneReorderCmd = new Regex("(?i:done)", Extensions.RegOpts);

        private static readonly Regex switchCmdPattern = new Regex("(?i:switch)", Extensions.RegOpts);
        private static readonly Regex toggleCmdPattern = new Regex("(?i:toggle)", Extensions.RegOpts);
        private static readonly Regex cmdArgs = new Regex(@"\b(\d+?)\b", Extensions.RegOpts);

        public void AttachEvents(ChatEventRouter router)
        {
            this.router = router;
            router.RegisterTriggerEvent(EventType.UserMentioned, this);
        }

        public bool HandleEvent(object sender, ChatEventArgs e)
        {
            if (activeUser == null)
            {
                if (startReorderCmdPattern.IsMatch(e.Message.Content))
                {
                    activeUser = e.Message.Author;
                    PrintCommandListReply(e.Message, e.Room);
                    e.Handled = true;
                    return true; // stay as the active trigger
                }
            }
            else if (e.Message.Author.Name.Equals(activeUser.Name))
            {
                List<int> args = ParseCommandArgs(e.Message.Content);
                if (switchCmdPattern.IsMatch(e.Message.Content))
                {
                    // need 2 args
                    if (args.Count < 2)
                    {
                        e.Room.PostReplyFast(e.Message, "Not enough parameters. Try again, dummy.");
                        e.Handled = true;
                        return true;
                    }
                    // sanity check
                    if (args[0] > router.Commands.Count || args[1] > router.Commands.Count)
                    {
                        e.Room.PostReplyFast(e.Message, "Message failed sanity check. You're crazy.");
                        e.Handled = true;
                        return true;
                    }

                    //swap
                    ICommand first = router.Commands[args[0] - 1];
                    router.Commands[args[0] - 1] = router.Commands[args[1] - 1];
                    router.Commands[args[1] - 1] = first;
                    e.Handled = true;
                    return true;

                }
                else if (toggleCmdPattern.IsMatch(e.Message.Content))
                {
                    if (args.Count <1)
                    {
                        e.Room.PostReplyFast(e.Message, "Not enough parameters. Try again, dummy.");
                        e.Handled = true;
                        return true;
                    }
                    if (args[0] > router.Commands.Count)
                    {
                        e.Room.PostReplyFast(e.Message, "Message failed sanity check. You're crazy.");
                        e.Handled = true;
                        return true;
                    }

                    // toggle the state.
                    router.CommandStates[router.Commands[args[0] - 1]] = !router.CommandStates[router.Commands[args[0] - 1]];
                }
                else if (doneReorderCmd.IsMatch(e.Message.Content))
                {
                    this.activeUser = null;
                    e.Room.PostReplyFast(e.Message, "Done.");
                    e.Handled = true;
                }
                else
                {
                    e.Room.PostReplyFast(e.Message, "Try not mumbling so much.");
                }
            }
            
            return false;
        }

        private List<int> ParseCommandArgs(string content)
        {
            List<int> result = new List<int>();

            foreach (Match m in cmdArgs.Matches(content))
            {
                int item = -1;
                if (int.TryParse(content, out item))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private void PrintCommandListReply(Message m, Room r)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < router.Commands.Count; i++)
            {
                sb.AppendFormat("{0}. {1}", i + 1, router.Commands[i].Usage);
            }
            r.PostReplyFast(m, sb.ToString());
        }


    }
}
