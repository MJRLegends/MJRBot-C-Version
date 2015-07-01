using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MJRBot
{
    class Viewers
    {
        public static String viewers = "";
        public static String mods = "";

        public static List<string> moderators = new List<string>();

        public static void getViewers()
        {
            try
            {
                String result;
                WebClient web = new WebClient();
                System.IO.Stream stream = web.OpenRead("https://tmi.twitch.tv/group/user/" + BotClient.getChannel(false).ToLower() + "/chatters");
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
                if (result.Length > 1)
                {
                    if (result.Contains("viewers" + "\"" + ": [") || result.Contains("moderators" + "\"" + ": [")
                || result.Contains("staff" + "\"" + ": [") || result.Contains("admins" + "\"" + ": [")
                || result.Contains("global_mods" + "\"" + ": ["))
                    {
                        if (result.Contains("moderators" + "\"" + ": ["))
                        {
                            mods = result.Substring(result.IndexOf("moderators") + 21);
                            mods = mods.Substring(0, mods.IndexOf("staff"));
                        }
                        if (result.Contains("viewers" + "\"" + ": ["))
                        {
                            viewers = "," + result.Substring(result.IndexOf("viewers") + 18);
                            viewers = viewers.Substring(0, viewers.IndexOf("]"));
                        }
                        String newviewers = mods + viewers;
                        if (newviewers.Length > 1)
                        {
                            newviewers = newviewers.Replace(" ", "");
                            newviewers = newviewers.Replace("\"", "");
                            newviewers = newviewers.Replace("\n", "");
                            newviewers = newviewers.Replace("\\", "");
                            newviewers = newviewers.Replace("]", "");

                            mods = mods.Replace(" ", "");
                            mods = mods.Replace("\"", "");
                            mods = mods.Replace("\n", "");
                            mods = mods.Replace("\\", "");
                            mods = mods.Replace("]", "");

                            String[] ModeratorsList;
                            ModeratorsList = mods.Split(',');
                            foreach (String viewer in ModeratorsList)
                            {
                                moderators.Add(viewer.ToLower());
                            }
                            String[] viewersList;
                            viewersList = newviewers.Split(',');
                            foreach (String viewer in viewersList)
                            {
                                BotClient.addUser(viewer.ToLower());
                            }
                        }
                    }
                }
                else
                {
                    BotClient.chatMessages.Add("[MJRBot Info]" + "Error when getting viewers from twitch");
                    return;
                }
            }
            catch
            {
                BotClient.chatMessages.Add("[MJRBot Info]" + "Unable to get any viewers!");
            }
        }
    }
}
