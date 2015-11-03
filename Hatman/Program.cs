using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatExchangeDotNet;
using Hatman.Commands;

namespace Hatman
{
    class Program
    {
        private static readonly ManualResetEvent shutdownMre = new ManualResetEvent(false);
        private static readonly List<ICommand> commands = new List<ICommand>();
        private static readonly List<ICommand> triggers = new List<ICommand>();
        private static Client chatClient;
        private static Room chatRoom;
        private static string roomURL;



        public static void Main(string[] args)
        {
            Console.Title = "Hatman";

            if (!File.Exists("Config.txt"))
            {
                Console.WriteLine("Config.txt not found." +
                    " \nPlease ensure the file can be found within the working directory.");
                Console.Read();
                return;
            }

            //PopulateTriggers();
            PopulateCommands();

            var email = "";
            var pass = "";
            ReadConfig(out email, out pass);

            chatClient = new Client(email, pass);
            chatRoom = chatClient.JoinRoom(roomURL);
            AttachChatEventListeners();

            shutdownMre.WaitOne();
        }

        private static void ReadConfig(out string email, out string password)
        {
            var settings = File.ReadAllLines("Config.txt");
            email = "";
            password = "";

            foreach (var l in settings)
            {
                var prop = l.Trim().ToUpperInvariant().Substring(0, 4);

                switch (prop)
                {
                    case "EMAI":
                    {
                        email = l.Remove(0, 6);
                        break;
                    }
                    case "PASS":
                    {
                        password = l.Remove(0, 9);
                        break;
                    }
                    case "ROOM":
                    {
                        roomURL = l.Remove(0, 8);
                        break;
                    }
                }
            }
        }

        private static void AttachChatEventListeners()
        {
            chatRoom.EventManager.ConnectListener(EventType.InternalException, new Action<Exception>(e =>
            {
                Console.WriteLine(e);
            }));
            chatRoom.EventManager.ConnectListener(EventType.UserMentioned, new Action<Message>(m =>
            {
                foreach (var cmd in commands)
                {
                    if (cmd.CommandPattern.IsMatch(m.Content))
                    {
                        cmd.ProcessMessage(m, ref chatRoom);
                    }
                }
            }));
            chatRoom.EventManager.ConnectListener(EventType.MessagePosted, new Action<Message>(m =>
            {
                // Handle possible trigger.
            }));
            chatRoom.EventManager.ConnectListener(EventType.UserEntered, new Action<User>(u =>
            {
                // Handle user entering room.
            }));
            chatRoom.EventManager.ConnectListener(EventType.UserLeft, new Action<User>(u =>
            {
                // Handle user leaving room.
            }));
        }

        private static void PopulateCommands()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var cmds = types.Where(t => t.Namespace == "Hatman.Commands");

            foreach (var type in cmds)
            {
                if (type.IsInterface) { continue; }

                var instance = (ICommand)Activator.CreateInstance(type);

                commands.Add(instance);
            }
        }

        private static void PopulateTriggers()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var eventTypes = types.Where(t => t.Namespace == "Hatman.Triggers");

            foreach (EventType chatEvent in Enum.GetValues(typeof(EventType)))
            {
                var eventName = Enum.GetName(typeof(EventType), chatEvent);
                var type = eventTypes.First(t => t.Name == eventName);
                var instance = (ICommand)Activator.CreateInstance(type);

                commands.Add(instance);
            }
        }
    }
}
