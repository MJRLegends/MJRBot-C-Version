using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MJRBot
{
    class Followers
    {
        public static int followersNum = 0;
        public static List<String> followers = new List<String>();
        public static void getFollowersNum()
        {
            String result;
            WebClient web = new WebClient();
            System.IO.Stream stream = web.OpenRead("https://api.twitch.tv/kraken/channels/" + BotClient.getChannel(false).ToLower());
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            if (result.Contains("followers"))
            {
                result = result.Substring(result.IndexOf("followers") + 11);
                result = result.Substring(0, result.IndexOf(','));
                
                followersNum = Convert.ToInt32(result);
            }
        }
        public static void getFollowers()
        {
            String result;
            WebClient web = new WebClient();
            System.IO.Stream stream = web.OpenRead("https://api.twitch.tv/kraken/channels/" + BotClient.getChannel(false).ToLower() + "/follows?limit=100");
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            int times;

            if (followersNum > 1700)
                times = 1700;
            else
                times = followersNum;
            if (times < 100)
            {
                String oldname = "";
                for (int j = 0; j < times; j++)
                {
                    if (j == 0)
                    {
                        oldname = result.Substring(result.IndexOf("display_name") + 15);
                        oldname = oldname.Substring(0, oldname.IndexOf("\""));
                        followers.Add(oldname.ToLower());
                    }
                    else
                    {
                        String temp = "";
                        temp = result.Substring(result.IndexOf(oldname));
                        temp = temp.Substring(temp.IndexOf("logo"));
                        temp = temp.Substring(temp.IndexOf("display_name") + 15);
                        temp = temp.Substring(0, temp.IndexOf("\""));
                        followers.Add(temp.ToLower());
                        oldname = temp;
                    }
                }
            }
            else
            {
                int left = times;
                int currentSet = 0;
                for (int j = 0; j < left; j++)
                {
                    String result2;
                    WebClient web2 = new WebClient();
                    String url;
                    Console.WriteLine("Set " + currentSet);
                    if(currentSet == 100)
                        url = "https://api.twitch.tv/kraken/channels/" + BotClient.getChannel(false).ToLower() + "/follows?limit=" + currentSet;
                    else
                        url = "https://api.twitch.tv/kraken/channels/" + BotClient.getChannel(false).ToLower() + "/follows?direction=DESC&limit=100&offset=" + currentSet;
                    System.IO.Stream stream2 = web.OpenRead(url);
                    using (System.IO.StreamReader reader2 = new System.IO.StreamReader(stream2))
                    {
                        result2 = reader2.ReadToEnd();
                    }
                    String oldname = "";
                    for (int k = 0; k < 100; k++)
                    {
                        Console.WriteLine("Follower " + k);
                        if (k == 0)
                        {
                            oldname = result.Substring(result.IndexOf("display_name") + 15);
                            oldname = oldname.Substring(0, oldname.IndexOf("\""));
                            followers.Add(oldname.ToLower());
                            left--;
                        }
                        else
                        {
                            String temp = "";
                            temp = result.Substring(result.IndexOf(oldname));
                            temp = temp.Substring(temp.IndexOf("logo"));
                            temp = temp.Substring(temp.IndexOf("display_name") + 15);
                            temp = temp.Substring(0, temp.IndexOf("\""));
                            followers.Add(temp.ToLower());
                            oldname = temp;
                            left--;
                        }
                    }
                    currentSet = currentSet + 100;
                    
                }
            }
            BotClient.chatMessages.Add("[MJRBot Info]" + "MJRBot has got " + +followers.Count + " followers!");
        }
        public static void checkForNewFollowers()
        {
            if (BotClient.connected == false || followers.Count < 1)
                return;
            String followersList = "";
            String result;
            WebClient web = new WebClient();
            System.IO.Stream stream = web.OpenRead("https://api.twitch.tv/kraken/channels/" + BotClient.getChannel(false).ToLower() + "/follows?limit=100");
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            int times = 10;
            String oldname = "";
            for (int j = 0; j < times; j++)
            {
                if (j == 0)
                {
                    oldname = result.Substring(result.IndexOf("display_name") + 15);
                    oldname = oldname.Substring(0, oldname.IndexOf("\""));
                    followersList = oldname;
                }
                else
                {
                    String temp = "";
                    temp = result.Substring(result.IndexOf(oldname));
                    temp = temp.Substring(temp.IndexOf("logo"));
                    temp = temp.Substring(temp.IndexOf("display_name") + 15);
                    temp = temp.Substring(0, temp.IndexOf("\""));
                    followersList = followersList + "," + oldname;
                    oldname = temp;
                }
            }
            String[] tempFollowers = followersList.Split(',');
            for (int i = 0; i < tempFollowers.Length; i++)
            {
                if (!followers.Contains(tempFollowers[i].ToLower()))
                {
                    followers.Add(tempFollowers[i].ToLower());
                    BotClient.chatMessages.Add("[MJRBot Info]" + tempFollowers[i].ToLower() + " has just followed!");
                    followersNum++;
                }
            }
        }
    }
}
