using System;
using System.Collections.Generic;
using System.Linq;
using ChatExchangeDotNet;
using Hatman.Triggers;
using Hatman.Commands;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;

namespace Hatman
{
    public class ChatEventRouter
    {
        private readonly MoarConfusion con = new MoarConfusion();
        private Room monitoredRoom;

        public ManualResetEvent ShutdownMre = new ManualResetEvent(false);



        public ChatEventRouter(Room chatRoom, string token)
        {
            monitoredRoom = chatRoom;

            PopulateCommands(token);
            PopulateTriggers();

            chatRoom.EventManager.ConnectListener(EventType.UserMentioned, new Action<Message>(m =>
            {
                EventCallback(EventType.UserMentioned, m, null, monitoredRoom, null);
            }));
            chatRoom.EventManager.ConnectListener(EventType.MessagePosted, new Action<Message>(m =>
            {
                EventCallback(EventType.MessagePosted, m, null, monitoredRoom, null);
            }));
            chatRoom.EventManager.ConnectListener(EventType.UserEntered, new Action<User>(u =>
            {
                EventCallback(EventType.UserEntered, null, u, monitoredRoom, null);
            }));
            chatRoom.EventManager.ConnectListener(EventType.UserLeft, new Action<User>(u =>
            {
                EventCallback(EventType.UserLeft, null, u, monitoredRoom, null);
            }));
        }


        #region Event Router

        private void EventCallback(EventType evt, Message m, User u, Room r, string raw)
        {
            if (evt == EventType.UserMentioned)
            {
                var n = new byte[4];
                Extensions.RNG.GetBytes(n);
                var i = BitConverter.ToUInt32(n, 0);

                if (i % 50 == 0)
                {
                    r.PostMessageFast("    FATAL EXCEPTION IN Hatman.ChatEventRouter.EventCallback() : COULD NOT -- Just kidding. Noooope.");
                    return;
                }

                if (Regex.IsMatch(m.Content, @"(?i)^(die|stop|shutdown)$"))
                {
                    if (i % 33 == 0)
                    {
                        r.PostMessageFast("Nope.");
                        return;
                    }
                    ShutdownMre.Set();
                    return;
                }
            }

            Console.WriteLine("{0} - {1} - {2}", 
                evt, 
                u != null ? u.GetChatFriendlyUsername() : (m != null ? m.Author.GetChatFriendlyUsername() : ""),
                m != null ? m.Content : "");

            var args = new ChatEventArgs(evt, m, u, r, raw);
            var handled = HandleTriggerEvent(args);

            if (!handled && m != null && evt == EventType.UserMentioned) HandleCommandEvent(args);
        }


        #endregion


        #region Triggers

        private Dictionary<EventType, ITrigger> ActiveTriggers = new Dictionary<EventType, ITrigger>();
        private Dictionary<EventType, List<ITrigger>> Triggers = new Dictionary<EventType, List<ITrigger>>();

        private void PopulateTriggers()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var trgs = types.Where(t => t.Namespace == "Hatman.Triggers");

            foreach (var type in trgs)
            {
                if (type.IsInterface) { continue; }

                var instance = (ITrigger)Activator.CreateInstance(type);

                instance.AttachEvents(this);
            }
        }

        public bool RegisterTriggerEvent(EventType type, ITrigger trigger)
        {
            if (!Triggers.ContainsKey(type))
            {
                Triggers.Add(type, new List<ITrigger>());
            }

            if (!ActiveTriggers.ContainsKey(type))
            {
                ActiveTriggers.Add(type, null);
            }

            Triggers[type].Add(trigger);
            return true;
        }

        private bool HandleTriggerEvent(ChatEventArgs e) 
        {
            bool stayActive = false;

            if (!Triggers.ContainsKey(e.Type)) return false;

            // Last active gets first crack
            if (ActiveTriggers[e.Type] != null)
            {
                stayActive = ActiveTriggers[e.Type].HandleEvent(this, e);
                if (e.Handled)
                {
                    return e.Handled;
                }
            }
            
            foreach (ITrigger trigger in Triggers[e.Type])
            {
                stayActive = trigger.HandleEvent(this, e);
                if (stayActive && e.Handled)
                {
                    ActiveTriggers[e.Type] = trigger;
                }
                if (e.Handled)
                {
                    break;
                }
            }

            if (!stayActive)
            {
                ActiveTriggers[e.Type] = null;
            }
            
            return e.Handled; 
        }
        
        #endregion

        #region Commands

        public List<ICommand> Commands
        {
            get { return commands; }
        }

        public Dictionary<ICommand, bool> CommandStates
        {
            get { return commandStates; }
        }

        Dictionary<ICommand, bool> commandStates = new Dictionary<ICommand, bool>();
        List<ICommand> commands = new List<ICommand>();

        private void PopulateCommands(string tkn)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var cmds = types.Where(t => t.Namespace == "Hatman.Commands");

            foreach (var type in cmds)
            {
                if (type.IsInterface || type.IsSealed || type == typeof(MoarConfusion)) { continue; }

                ICommand instance;

                if (type.Name == "Update")
                {
                    instance = (ICommand)Activator.CreateInstance(type, tkn);
                }
                else
                {
                    instance = (ICommand)Activator.CreateInstance(type);
                }
                commands.Add(instance);
                commandStates.Add(instance, true);
            }
        }

        private void HandleCommandEvent(ChatEventArgs e)
        {
            var r = e.Room;

            foreach (ICommand command in commands)
            {
                if (!commandStates[command]) continue;

                if (command.CommandPattern.IsMatch(e.Message.Content))
                {
                    command.ProcessMessage(e.Message, ref r);
                    return;
                }
            }

            con.ProcessMessage(e.Message, ref r);
        }

        #endregion

    }

    public class ChatEventArgs
    {
        public Message Message { get; private set; }
        public User User { get; private set; }
        public Room Room { get; private set; }
        public string RawData { get; private set; }
        public EventType Type { get; private set; }

        public bool Handled { get; set; }

        public ChatEventArgs(EventType t, Message m, User u, Room r, string rawData)
        {
            Type = t;
            Message = m;
            User = u;
            Room = r;
            Handled = false;
        }
    }
}
