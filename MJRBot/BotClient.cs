using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net.Sockets;

namespace MJRBot
{
    class BotClient
    {
        private static StreamWriter socketStreamWriter;
        private static StreamReader socketStreamReceiver;
        private static int socketIndex = 0;
        private static Thread socketReader;
        private static Thread socketWriter;
        private static TcpClient clientSocket;

        private static bool shouldStop = false;

        public static String channel = "";

        private static List<string> socketCommands = new List<string>();
        public static List<string> chatMessages = new List<string>();
        public static List<string> onlineUsers = new List<string>();

        public static void connectToServer(String server, int port)
        {
            try
            {
                clientSocket = new TcpClient();
                clientSocket.Connect(server,port);

                socketReader = new Thread(getMessageFromServer);
                socketReader.Start();

                socketWriter = new Thread(sendMessageToServer);
                socketWriter.Start();
                chatMessages.Add("[MJRBot Info]" + "Connecting to Twitch!");
                socketCommands.Add("PASS " + SettingsFile.getSetting("Password"));
                socketCommands.Add("NICK " + SettingsFile.getSetting("Username"));
                chatMessages.Add("[MJRBot Info]" + "Connected to Twitch!");
            }
            catch(Exception ex)
            {
                chatMessages.Add("Error! Message: " + ex.Message);
            }
        }

        public static void joinChannel(String newchannel)
        {
            channel = "#" + newchannel;
            try
            {
                socketCommands.Add("JOIN " + channel);
                chatMessages.Add("[MJRBot Info]" + "Joined " + getChannel(false) + "'s channel");
            }
            catch
            {
                chatMessages.Add("Error joining that channel!");
            }
        }

        /// <summary>
        /// Disconnects the Bot from the IRC Server
        /// </summary>
        public static void disconnectFromServer()
        {
            try
            {
                socketReader.Abort();
                socketWriter.Abort();
            }
            catch { }
            chatMessages.Clear();
            onlineUsers.Clear();
        }
        /// <summary>
        /// To receive messages from the IRC Server 
        /// </summary>
        private static void getMessageFromServer()
        {
            while (!shouldStop)
            {
                socketStreamReceiver = new StreamReader(clientSocket.GetStream());
                string _response = socketStreamReceiver.ReadLine();
                if (_response != null)
                {
                    Console.WriteLine("> " + _response);
                    parseChatLine(_response);
                }
                else
                {
                    //Happens after 10 Min withot sending data
                }
            }
        }
        /// <summary>
        /// To send messages from the IRC Server
        /// </summary>
        private static void sendMessageToServer()
        {
            socketStreamWriter = new StreamWriter(clientSocket.GetStream());
            while (!shouldStop)
            {
                if (socketCommands.Count > socketIndex)
                {
                    socketStreamWriter.WriteLine(socketCommands[socketIndex]);
                    Console.WriteLine("< " + socketCommands[socketIndex]);
                    socketStreamWriter.Flush();
                    socketIndex++;
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// To send a message to the IRC Server
        /// </summary>
        /// <param name="message"></param>
        public static void sendChatMessage(string message)
        {
            socketCommands.Add("PRIVMSG " + getChannel(true) + " :" + message);
            chatMessages.Add("[Bot]" + "Mjrbot" + ": " + message);
        }

        /// <summary>
        /// Checking the message from the IRC Server
        /// </summary>
        /// <param name="chatLine"></param>
        private static void parseChatLine(string chatLine)
        {
            if (chatLine.StartsWith(":"))
            {
                if (chatLine.Contains("PRIVMSG"))
                {
                    int pos = chatLine.IndexOf(getChannel(true)) + getChannel(true).Length + 2;
                    string message = chatLine.Substring(pos, chatLine.Length - pos);
                    string user = chatLine.Substring(1, chatLine.IndexOf('!') - 1);
                    addUser(user);
                    String prefix;
                    if (user.ToLower().Equals("mjrlegends"))
                        prefix = "Bot Owner";
                    else if (user.ToLower().Equals(getChannel(false)))
                        prefix = "Streamer";
                    else if (Viewers.moderators.Contains(user.ToLower()))
                        prefix = "Moderator";
                    else
                        prefix = "User";

                    chatMessages.Add("[" + prefix + "]" + user + ": " + message);
                    PointsFile.isOnList(user);
                    RanksFile.isOnList(user);
                    if (SettingsFile.getSetting("Commands").Equals("true"))
                        Commands.onCommand(user, message.ToLower());
                    ChatModeration.Check(message, user);
                }
                else if (chatLine.Contains("JOIN"))
                {
                    string username = chatLine.Substring(1, chatLine.IndexOf('!') - 1);
                    addUser(username);
                }
                else if (chatLine.Contains("PART"))
                {
                    string username = chatLine.Substring(1, chatLine.IndexOf('!') - 1);
                    delUser(username);
                }
            }
        }
        /// <summary>
        /// Adds a User to the onlineUsers List Array
        /// </summary>
        /// <param name="username"></param>
        public static void addUser(string username)
        {
            if (onlineUsers.Count == 0)
            {
                onlineUsers.Add(username);
            }
            else
            {
                for (int i = 0; i < onlineUsers.Count; i++)
                {
                    if (onlineUsers[i] == username)
                    {
                        return;
                    }
                }
                onlineUsers.Add(username);
            }
            chatMessages.Add("[MJRBot Info]" + username + " has joined!");
        }

        /// <summary>
        /// Deletes a User from onlineUsers List Array
        /// </summary>
        /// <param name="username"></param>
        public static void delUser(string username)
        {
            if (onlineUsers.Count > 0)
            {
                onlineUsers.Remove(username);
            }
            chatMessages.Add("[MJRBot Info]" + username + " has left!");
        }

        /// <summary>
        /// Returns a list of Users from the onlineUsers List Array
        /// </summary>
        /// <returns>Returns a list of Users</returns>
        public static string getUserList()
        {
            string user = "";
            for (int i = 0; i < onlineUsers.Count; i++)
            {
                if (!"".Equals(onlineUsers[i]))
                {
                    user += onlineUsers[i] + "\r\n";
                }
            }

            if (user.Length == 0)
            {
                return "";
            }
            return user.Substring(0, user.Length - 2);
        }

        /// <summary>
        /// Returns a list of Messages from the chatMessages List Array
        /// </summary>
        /// <returns>Returns a List of Chat Messages</returns>
        public static string getChatMessages()
        {
            string temp = "";
            for (int i = 0; i < chatMessages.Count; i++)
            {
                temp = chatMessages[i] + "\r\n" + temp;
            }
            return temp;
        }

        public static string getChannel(bool withHash)
        {
            if (withHash)
                return channel;
            else
                return channel.Substring(1);
        }
    }
}
