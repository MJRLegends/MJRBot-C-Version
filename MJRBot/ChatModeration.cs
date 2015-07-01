using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MJRBot
{
    class ChatModeration
    {
        public static bool Ban = false;
        public static String banType = "";

        public static bool Allowed = false;
        public static String PermitedUsers = "";
        public static bool Link = false;


        private static List<String> emotes = new List<String>();
        public static String[] BadWords = { "Fuck", "Shit", "Cunt", "Wanker", "Tosser", "Slag", "Slut", "Penis", "Cock", "Vagina", "Pussy",
	    "Boobs", "Tits", "Ass", "Bastard", "Twat", "Nigger", "Bitch", "***"};


        public static void Check(String message, String user){
            getEmotes();
            if (Ban != true)
                if (SettingsFile.getSetting("BadwordsChecker").Equals("true"))
                CheckBadWords(message,user);
            if (Ban != true)
                if (SettingsFile.getSetting("LinkChecker").Equals("true"))
                CheckLink(message,user);
            if (Ban != true)
                if (SettingsFile.getSetting("EmoteChecker").Equals("true"))
                    checkEmoteSpam(message, user);
            if (Ban != true && Link != true)
                if (SettingsFile.getSetting("SymbolChecker").Equals("true"))
                checkSymbolSpam(message,user);
            if (Ban)
            {
                if (!Viewers.moderators.Contains(user.ToLower()) && !user.ToLower().Equals(BotClient.getChannel(false)) && !RanksFile.getRank(user).ToLower().Equals("gold") && user.ToLower().Equals("mjrlegends"))
                {
                    if (banType.Equals("Words"))
                    {
                        BotClient.sendChatMessage("/timeout " + user);
                        BotClient.sendChatMessage("/unban " + user);
                        BotClient.sendChatMessage(user + " " + SettingsFile.getSetting("LanguageWarning"));
                    }
                    else if (banType.Equals("Emotes"))
                    {
                        if (!RanksFile.getRank(user).ToLower().Equals("sliver"))
                        {
                            BotClient.sendChatMessage("/timeout " + user);
                            BotClient.sendChatMessage("/unban " + user);
                            BotClient.sendChatMessage(user + " " + SettingsFile.getSetting("EmoteWarning"));
                        }
                    }
                    else if (banType.Equals("Links"))
                    {
                        if (!RanksFile.getRank(user).ToLower().Equals("sliver") && !RanksFile.getRank(user).ToLower().Equals("bronze"))
                        {
                            BotClient.sendChatMessage("/timeout " + user);
                            BotClient.sendChatMessage("/unban " + user);
                            BotClient.sendChatMessage(user + " " + SettingsFile.getSetting("LinkWarning"));
                        }
                    }
                    else if (banType.Equals("Symbols"))
                    {
                        if (!RanksFile.getRank(user).ToLower().Equals("sliver"))
                        {
                            BotClient.sendChatMessage("/timeout " + user);
                            BotClient.sendChatMessage("/unban " + user);
                            BotClient.sendChatMessage(user + " " + SettingsFile.getSetting("SymbolWarning"));
                        }
                    }
                    Ban = false;
                    banType = "";
                }
            }
        }
        public static void CheckBadWords(String message, String sender)
        {
            for (int i = 0; i < BadWords.Length; i++)
            {
                String Check = BadWords[i];
                if (message.ToLower().Contains(Check.ToLower()))
                {
                    if (Viewers.moderators.Contains(sender))
                    {
                        Ban = false;
                    }
                    else
                    {
                        Ban = true;
                        banType = "Words";
                    }
                }
            }
        }


        public static void checkEmoteSpam(String message, String user)
        {
            int number = 0;
            String[] temp;
            temp = message.Split(' ');
            for (int i = 0; i < temp.Length; i++)
            {
                if (emotes.Contains(temp[i].ToLower()))
                {
                    number++;
                }
            }
            if (number > Convert.ToInt32(SettingsFile.getSetting("MaxEmotes")))
            {
                Ban = true;
                banType = "Emotes";
            }
        }


        public static void CheckLink(String message, String sender)
        {
            String TempMessage = "";
            if (message.StartsWith("www.") || message.StartsWith("http://www."))
            {
                if (message.StartsWith("http://www."))
                {
                    TempMessage = message;
                }
                else if (message.StartsWith("www."))
                {
                    TempMessage = "http://" + message;
                }
            }
            else if (!message.StartsWith("www.") && !message.StartsWith("http://www.") && message.Contains("www.")
              || message.Contains("http://www."))
            {
                if (message.Contains("www."))
                {
                    message = message.Substring(message.IndexOf("www."));
                    if (message.Contains(" "))
                    {
                        message = message.Substring(0, message.IndexOf(' '));
                    }
                    TempMessage = "http://" + message;
                }
                else if (message.Contains("http://www."))
                {
                    message = message.Substring(message.IndexOf("http://www."));
                    if (message.Contains(" "))
                    {
                        message = message.Substring(0, message.IndexOf(' '));
                    }
                    TempMessage = message;
                }
            }
            else if (!message.StartsWith("www.") || !message.StartsWith("http://www.") || !message.Contains("www.")
              || !message.Contains("http://www."))
            {
                TempMessage = "http://www." + message;
            }

            try
            {
                String result;
                WebClient web = new WebClient();
                System.IO.Stream stream = web.OpenRead(TempMessage);
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
                Link = true;
                if (Viewers.moderators.Contains(sender.ToLower()))
                {
                    Allowed = true;
                }
                else if (PermitedUsers.Contains(sender))
                {
                    Allowed = true;
                    PermitedUsers = "";
                }
                else
                {
                    Allowed = false;
                }

            }
            catch (Exception e)
            {
                Link = false;
            }
            if (Allowed == false && Link == true)
            {
                Ban = true;
                banType = "Links";
            }
        }

        public static void checkSymbolSpam(String message, String user)
        {
            int number = 0;
            for (int i = 0; i < message.Length; i++)
            {
                Char[] test = message.ToCharArray();
                foreach(Char testChar in test){
                    if (!Char.IsLetterOrDigit(testChar))
                        if (!testChar.Equals('!') && !testChar.Equals(' '))
                        number++;
                }
            }
            if (number > Convert.ToInt32(SettingsFile.getSetting("MaxSymbols")))
            {
                Ban = true;
                banType = "Symbols";
            }
        }

        public static void getEmotes()
        {
            try
            {
                String result;
                WebClient web = new WebClient();
                System.IO.Stream stream = web.OpenRead("https://api.twitch.tv/kraken/chat/" + BotClient.getChannel(false) + "/emoticons");
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }

                int index = result.IndexOf("regex");
                while (index > -1)
                {
                    result = result.Substring(index + 8);
                    emotes.Add(result.Substring(0, result.IndexOf("\"")));
                    index = result.IndexOf("regex");
                }

                emotes.Add(":)");
                emotes.Add(":(");
                emotes.Add(":/");
                emotes.Add(":O");
                emotes.Add(":D");
                emotes.Add(":P");
                emotes.Add(">(");
                emotes.Add(":Z");
                emotes.Add("O_o");
                emotes.Add("B)");
                emotes.Add("<3");
                emotes.Add(";)");
                emotes.Add(";P");
                emotes.Add("R)");
            }
            catch (Exception e)
            {
            }
            ;
        }
    }
}
