using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Commands : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)^(commands|cmds)", Extensions.RegOpts);
        private readonly List<ICommand> commands = new List<ICommand>();

        public Regex CommandPattern => ptn;

        public string Description =>  "I get to show off what I can do.";

        public string Usage =>  "commands|cmds";



        public Commands()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var cmds = types.Where(t => t.Namespace == "Hatman.Commands");

            foreach (var type in cmds)
            {
                if (type.IsInterface || type.IsSealed || type.Name == "Commands") continue;

                var instance = (ICommand)Activator.CreateInstance(type);

                commands.Add(instance);
            }
        }



        public void ProcessMessage(Message msg, ref Room rm)
        {
            var cmdsMsg = new MessageBuilder(MultiLineMessageType.Code, false);
            cmdsMsg.AppendPing(msg.Author);

            foreach (var cmd in commands)
            {
                cmdsMsg.AppendText("\n" + cmd.Usage + " - " + cmd.Description);
            }

            cmdsMsg.AppendText("\n" + Usage + " - " + Description);

            rm.PostMessageLight(cmdsMsg);
        }
    }
}
