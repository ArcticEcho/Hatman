using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Commands : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^(commands|cmds)", Extensions.RegOpts);
        private readonly List<ICommand> commands = new List<ICommand>();

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
                return "I get to show off what I can do.";
            }
        }

        public string Usage
        {
            get
            {
                return "[...]commands[...]";
            }
        }

        public void ProcessMessage(Message msg, ref Room rm)
        {
            if (commands.Count == 0)
            {
                var types = Assembly.GetExecutingAssembly().GetTypes();
                var cmds = types.Where(t => t.Namespace == "Hatman.Commands");

                foreach (var type in cmds)
                {
                    if (type.IsInterface || type.IsSealed) { continue; }

                    var instance = (ICommand)Activator.CreateInstance(type);

                    commands.Add(instance);
                }
            }

            var cmdsMsg = new MessageBuilder(MultiLineMessageType.Code, false);
            cmdsMsg.AppendPing(msg.Author);

            foreach (var cmd in commands)
            {
                cmdsMsg.AppendText("\n" + cmd.Usage + " - " + cmd.Description);
            }

            rm.PostMessageFast(cmdsMsg);
        }
    }
}
