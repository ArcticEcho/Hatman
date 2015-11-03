using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatExchangeDotNet;

namespace Hatman
{
    class Program
    {
        private static Client chatClient;
        private static Room chatRoom;
        private static string roomURL;



        public static void Main(string[] args)
        {
            Console.Title = "We Hat Fun";

            if (!File.Exists("Config.txt"))
            {
                Console.WriteLine("Config.txt not found." +
                    " \nPlease ensure the file can be found within the working directory.");
                Console.Read();
                return;
            }

            var email = "";
            var pass = "";
            ReadConfig(out email, out pass);

            chatClient = new Client(email, pass);
            chatRoom = chatClient.JoinRoom(roomURL);
            AttachChatEventListeners();
        }

        private static void ReadConfig(out string email, out string password)
        {
            var settings = File.ReadAllLines("Config.txt");
            email = "";
            password = "";

            foreach (var l in settings)
            {
                var prop = l.Trim().ToUpperInvariant().Substring(0, 4);

                switch (l)
                {
                    case "EMAI":
                    {
                        email = l.Remove(6);
                        break;
                    }
                    case "PASS":
                    {
                        password = l.Remove(9);
                        break;
                    }
                    case "ROOM":
                    {
                        roomURL = l.Remove(8);
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
                // Handle command. 
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
    }
}
