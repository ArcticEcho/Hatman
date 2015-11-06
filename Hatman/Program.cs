using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using ChatExchangeDotNet;
using Hatman.Commands;
using Hatman.Triggers;

namespace Hatman
{
    class Program
    {
        private static readonly ManualResetEvent shutdownMre = new ManualResetEvent(false);
        private static readonly List<ICommand> commands = new List<ICommand>();
        private static readonly List<ITrigger> triggers = new List<ITrigger>();
        private static Client chatClient;
        private static Room chatRoom;
        private static string roomURL;



        public static void Main(string[] args)
        {
            Console.Title = "Hatman";
            Console.Write("Reading config...");

            if (!File.Exists("Config.txt"))
            {
                Console.WriteLine("Config.txt not found." +
                    " \nPlease ensure the file can be found within the working directory.");
                Console.Read();
                return;
            }

            var email = "";
            var pass = "";
            var tkn = "";
            ReadConfig(out email, out pass, out tkn);

            chatClient = new Client(email, pass);

            Console.Write("done.\nJoining room...");
            chatRoom = chatClient.JoinRoom(roomURL);
            Extensions.SelfID = chatRoom.Me.ID;
            
            ChatEventRouter router = new ChatEventRouter(chatRoom, tkn);

            Console.WriteLine("done.\n");
            PostJoinPic();

            router.ShutdownMre.WaitOne();

            PostLeavePic();
            chatRoom.Leave();
        }

        private static void ReadConfig(out string email, out string password, out string appveyorTkn)
        {
            var settings = File.ReadAllLines("Config.txt");
            email = "";
            password = "";
            appveyorTkn = "";

            foreach (var l in settings)
            {
                if (string.IsNullOrWhiteSpace(l)) { continue; }

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
                    case "APPV":
                    {
                        appveyorTkn = l.Remove(0, 17);
                        break;
                    }
                }
            }
        }

        private static void PostJoinPic()
        {
            var pics = new[]
            {
                "https://i.stack.imgur.com/06ako.jpg",
                "https://i.stack.imgur.com/SY1YQ.jpg",
                "https://i.stack.imgur.com/lvGEs.jpg",
                "https://i.stack.imgur.com/e4Snz.jpg",
                "https://i.imgur.com/Cd1N7Dt.jpg"
            };

            chatRoom.PostMessageFast(pics.PickRandom());
        }

        private static void PostLeavePic()
        {
            var pics = new[]
            {
                "https://i.stack.imgur.com/8H3gH.jpg",
                "https://i.stack.imgur.com/HrDk9.jpg",
                "https://i.stack.imgur.com/LleDx.jpg",
                "https://i.stack.imgur.com/QIaHp.jpg",
                "https://i.stack.imgur.com/p7p8a.jpg"
            };

            chatRoom.PostMessageFast(pics.PickRandom());
        }
    }
}
