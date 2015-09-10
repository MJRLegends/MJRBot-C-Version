using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace MJRBot
{
    class Commands
    {
        /// <summary>
        /// To check for known commands
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        public static void onCommand(String user, String message)
        {
            string[] args = message.Split(' ');
            if (SettingsFile.getSetting(BotClient.getChannel(false), "Rank").Equals("true"))
            {
                if (message.ToLower().Equals("!rank"))
                {
                    BotClient.sendChatMessage(user + " your current rank is " + RanksFile.getRank(user));
                }
                if (message.ToLower().Contains("!buyrank"))
                {
                    if (args.Length == 2)
                    {
                        if (!RanksFile.isValidRank(args[1]))
                        {
                            BotClient.sendChatMessage(user + " the " + args[1] + " is not a valid rank!");
                            return;
                        }
                        RanksFile.setRank(user, args[1], true);
                    }
                    else
                    {
                        BotClient.sendChatMessage("Invalid arguments! You need to enter !buyrank <RANK>");
                    }
                }
            }
            if (SettingsFile.getSetting(BotClient.getChannel(false), "Points").Equals("true"))
            {
                if (message.ToLower().Equals("!points"))
                {
                    BotClient.sendChatMessage(user + " you currently have " + PointsFile.getPoints(user) + " points");
                }
                if (SettingsFile.getSetting(BotClient.getChannel(false), "Games").Equals("true"))
                {
                    if (message.ToLower().Equals("!spin"))
                    {
                        if (PointsFile.getPoints(user.ToLower()) >= 1)
                        {
                            String Answer = FruitMachine.Spin();
                            BotClient.sendChatMessage(user + " the Fruit Machine is spinning...");
                            int waittime = 0;
                            while (waittime < 100)
                            {
                                waittime++;
                            }
                            if (FruitMachine.hasWon())
                            {
                                BotClient.sendChatMessage(user + " " + Answer + " you have Won! " + PointsFile.AddRandomPoints(user.ToLower()) + " Points");
                            }
                            else
                            {
                                BotClient.sendChatMessage(user.ToLower() + " " + Answer + " you have lost! 1 Point taken!");
                                PointsFile.RemovePoints(user.ToLower(), 1);
                            }
                        }
                        else
                        {
                            BotClient.sendChatMessage(user.ToLower() + " you do not have enough points to play!");
                        }
                    }
                    else if (message.ToLower().Equals("!race"))
                    {
                        if (Racing_Game.StartedRace == false)
                        {
                            BotClient.sendChatMessage("The race will start in 1 minute! Use !placebet CAR TYPE POINTS(Cars 1-8)(Types Top3, 1st) E.g !placebet 5 Top3 10");
                            Racing_Game.StartedRace = true;
                            if(!Racing_Game.raceTimer.IsAlive)
                            Racing_Game.raceTimer.Start();
                        }
                        else
                        {
                            BotClient.sendChatMessage("Race has already been started!");
                        }
                    }
                    else if (message.ToLower().Contains("!placebet"))
                    {
                        if (Racing_Game.StartedRace)
                        {
                            if (args.Length == 4)
                            {
                                if (Racing_Game.checkForValue(user) == false)
                                {
                                    String bet = args[1];
                                    String type = args[2];
                                    String points = args[3];
                                    if (type.ToLower().Equals("1st".ToLower()) || type.ToLower().Equals("Top3".ToLower()))
                                    {
                                        Racing_Game.PlaceBet(user.ToLower(), bet, type, points);
                                        PointsFile.RemovePoints(user.ToLower(), Convert.ToInt32(points));
                                    }
                                    else
                                        BotClient.sendChatMessage("Invalid arguments! You need to enter !placebet CAR TYPE POINTS(Example !placebet 5 Top3 10) Cars range from 1 to 8, Types = Top3, 1st");
                                }
                                else
                                    BotClient.sendChatMessage(user.ToLower() + " you have already made a bet!");

                            }
                            else
                            {
                                BotClient.sendChatMessage("Invalid arguments! You need to enter !placebet CAR TYPE POINTS(Example !placebet 5 Top3 10) Cars range from 1 to 8, Types = Top3, 1st");
                            }
                        }
                        else
                            BotClient.sendChatMessage("Racing game hasnt been started yet!");
                    }
                    else if (message.ToLower().Contains("!answer"))
                    {
                        if (MathsGame.isMathsGameActive == true)
                        {
                            if (args.Length == 2)
                            {
                                int index = Convert.ToInt32(args[1]);
                                if (MathsGame.Answer == index)
                                {
                                    BotClient.sendChatMessage(user + " Well done, You have got the right answer! You have gained 10 points!");
                                    PointsFile.AddPoints(user.ToLower(), 10);
                                    MathsGame.isMathsGameActive = false;
                                }
                                else
                                {
                                    BotClient.sendChatMessage(user + " you have got the wrong answer try again!");
                                }
                            }
                            else
                            {
                                BotClient.sendChatMessage("Invalid arguments! You need to enter !answer YOURANSWER (Example !answer 10)");
                            }
                        }
                        else
                            BotClient.sendChatMessage("The maths game is currently not active!");
                    }
                }
            }
            if (message.ToLower().Equals("!commands"))
            {
                BotClient.sendChatMessage("You can check out the commands that " + "Mjrbot" + " offers over at http://goo.gl/iZhu2W");
            }
            if (message.ToLower().Equals("!uptime"))
            {
                String result;
                WebClient web = new WebClient();
                System.IO.Stream stream = web.OpenRead("https://api.twitch.tv/kraken/streams/" + BotClient.getChannel(false).ToLower());
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }

                if (result.Contains("updated_at"))
                {
                    String uptime = result.Substring(result.IndexOf("updated_at") + 13);
                    uptime = uptime.Substring(0, 19);
                    DateTime date = Convert.ToDateTime(uptime);
                    date.ToUniversalTime();
                    DateTime nowDate = DateTime.Now;
                    nowDate.ToUniversalTime();
                    TimeSpan upTime = nowDate.Subtract(date);

                    BotClient.sendChatMessage(BotClient.getChannel(false) + " has been live for " + upTime.Days + " day(s) " + upTime.Hours + " hour(s) " + upTime.Minutes + " minute(s)");
                }
                else
                {
                    BotClient.sendChatMessage(BotClient.getChannel(false) + " is currently not streaming!");
                }
            }
            if (user.ToLower().Equals(BotClient.getChannel(false)) || user.ToLower().Equals("mjrlegends"))
            {
                if (message.ToLower().Equals("!disconnect"))
                {
                    if (SettingsFile.getSetting(BotClient.getChannel(false), "SilentJoin").Equals("true"))
                    {
                        BotClient.sendChatMessage(SettingsFile.getSetting(null, "Username") + " Disconnected!");
                        Thread.Sleep(6000);
                    }
                    System.Environment.Exit(0);
                }
            }
            if (user.ToLower().Equals(BotClient.getChannel(false)) || Viewers.moderators.Contains(user.ToLower()) || user.ToLower().Equals("mjrlegends"))
            {
                if (message.ToLower().Contains("!permit"))
                {
                    if (args.Length == 2)
                    {
                        ChatModeration.PermitedUsers = ChatModeration.PermitedUsers + args[1].ToLower() + ", ";
                        BotClient.sendChatMessage(user + " is now permited to post a link for their next message!");
                    }
                    else
                        BotClient.sendChatMessage("Invalid arguments! You need to enter !permit <USER>");
                }
                if (message.ToLower().Contains("!addcommand"))
                {
                    if (args.Length >= 4)
                    {
                        if (args[2].ToLower().Equals("Mod".ToLower()) || args[2].ToLower().Equals("User".ToLower()))
                        {
                            String response = message.Substring(message.IndexOf(args[2]));
                            response = response.Substring(response.IndexOf(" "));
                            CommandsFile.addCommand(args[1].ToLower(), response, "true", args[2]);
                            if (CommandsFile.getCommandState(args[1].ToLower()) == "true")
                                BotClient.sendChatMessage("Command " + args[1] + " has been added!");
                        }
                        else
                            BotClient.sendChatMessage("Invalid Permission Level! Use Mod or User");
                    }
                    else
                        BotClient.sendChatMessage("Invalid arguments! You need to enter !addcommand <NAME> <PERMISSIONLEVEL> <RESPONSE>");
                }
                else if (message.ToLower().Contains("!removecommand"))
                {
                    if (args.Length == 2)
                    {
                        CommandsFile.removeCommand(args[1].ToLower());
                        BotClient.sendChatMessage("Command " + args[1] + " has been deleted!");
                    }
                    else
                        BotClient.sendChatMessage("Invalid arguments! You need to enter !removecommand <NAME>");
                }
                else if (message.ToLower().Contains("!commandstate"))
                {
                    if (args.Length == 3)
                    {
                        CommandsFile.setCommand(args[1].ToLower(), CommandsFile.getCommandResponse(args[1].ToLower()), args[2], CommandsFile.getCommandPermissions(args[1].ToLower()));
                        BotClient.sendChatMessage("Command " + args[1] + " has been updated!");
                    }
                    else
                        BotClient.sendChatMessage("Invalid arguments! You need to enter !commandstate <NAME> <TRUE/FALSE>");
                }
                else if (message.ToLower().Contains("!commandresponse"))
                {
                    if (args.Length >= 3)
                    {
                        String response = message.Substring(message.IndexOf(args[1]));
                        response = response.Substring(response.IndexOf(" "));
                        CommandsFile.setCommand(args[1].ToLower(), response, CommandsFile.getCommandState(args[1].ToLower()), CommandsFile.getCommandPermissions(args[1].ToLower()));
                        BotClient.sendChatMessage("Command " + args[1] + " has been updated!");
                    }
                    else
                        BotClient.sendChatMessage("Invalid arguments! You need to enter !commandresponse <NAME> <RESPONSE>");
                }

                if (SettingsFile.getSetting(BotClient.getChannel(false), "Rank").Equals("true"))
                {
                    if (message.ToLower().Contains("!setrank"))
                    {
                        if (args.Length == 3)
                        {
                            if (!RanksFile.isValidRank(args[1]))
                            {
                                BotClient.sendChatMessage(user + " the " + args[1] + " is not a valid rank!");
                                return;
                            }
                            RanksFile.setRank(args[2].ToLower(), args[1], false);
                        }
                        else
                            BotClient.sendChatMessage("Invalid arguments! You need to enter !setrank <RANK> <USER>");
                    }
                    else if (message.ToLower().Contains("!removerank"))
                    {
                        if (args.Length == 2)
                        {
                            RanksFile.RemoveRank(args[1].ToLower());
                        }
                        else
                            BotClient.sendChatMessage("Invalid arguments! You need to enter !removerank <USER>");
                    }
                    else if (message.ToLower().Contains("!getrank"))
                    {
                        if (args.Length == 2)
                        {
                            BotClient.sendChatMessage(args[1] + " has the rank " + RanksFile.getRank(args[1].ToLower()));
                        }
                        else
                            BotClient.sendChatMessage("Invalid arguments! You need to enter !getrank <USER>");
                    }
                }

                if (SettingsFile.getSetting(BotClient.getChannel(false), "Points").Equals("true"))
                {
                    if (SettingsFile.getSetting(BotClient.getChannel(false), "Games").Equals("true"))
                    {
                        if (message.ToLower().Equals("!maths"))
                        {
                            if (MathsGame.isMathsGameActive == false)
                            {
                                BotClient.sendChatMessage(MathsGame.CreateQuestion());
                                BotClient.sendChatMessage("Type !answer YOURANSWER (e.g !answer 10) to start guessing!");
                                MathsGame.isMathsGameActive = true;
                            }
                            else
                            {
                                BotClient.sendChatMessage("Game Already started!");
                            }
                        }
                    }
                    if (message.ToLower().Contains("!addpoints"))
                    {
                        if (args.Length == 3)
                        {
                            PointsFile.AddPoints(args[2].ToLower(), Convert.ToInt32(args[1]));
                            BotClient.sendChatMessage("Added " + Convert.ToInt32(args[1]) + " points " + "to " + args[2].ToLower());
                        }
                        else
                            BotClient.sendChatMessage("Invalid arguments! You need to enter !addpoints <POINTS> <USER>");
                    }
                    else if (message.ToLower().Contains("!removepoints"))
                    {
                        if (args.Length == 3)
                        {
                            PointsFile.RemovePoints(args[2].ToLower(), Convert.ToInt32(args[1]));
                            BotClient.sendChatMessage("Removed " + Convert.ToInt32(args[1]) + " points " + "from " + args[2].ToLower());
                        }
                        else
                            BotClient.sendChatMessage("Invalid arguments! You need to enter !removepoints <POINTS> <USER>");
                    }
                    else if (message.ToLower().Contains("!setpoints"))
                    {
                        if (args.Length == 3)
                        {
                            PointsFile.setPoints(args[2].ToLower(), Convert.ToInt32(args[1]));
                            BotClient.sendChatMessage("Set " + args[2] + " points to " + Convert.ToInt32(args[1]) + " points");
                        }
                        else
                            BotClient.sendChatMessage("Invalid arguments! You need to enter !setpoints <POINTS> <USER>");
                    }
                    else if (message.ToLower().Contains("!pointscheck"))
                    {
                        if (args.Length == 2)
                        {
                            BotClient.sendChatMessage(args[1].ToLower() + " has " + PointsFile.getPoints(args[1].ToLower()) + " points");
                        }
                        else
                            BotClient.sendChatMessage("Invalid arguments! You need to enter !pointscheck <USER>");
                    }
                }
            }
            if (CommandsFile.getCommandState(args[0].Substring(1)) != "false" && CommandsFile.getCommandState(args[0].Substring(1)) != "")
            {
                if(CommandsFile.getCommandPermissions(args[0].Substring(1)).ToLower().Equals("Mod".ToLower())){
                    if (!Viewers.moderators.Contains(user.ToLower()) && !BotClient.getChannel(false).ToLower().Equals(user.ToLower()))
                    {
                        return;
                    }
                }
                else if(CommandsFile.getCommandPermissions(args[0].Substring(1)).ToLower().Equals("User".ToLower()))
                BotClient.sendChatMessage(CommandsFile.getCommandResponse(args[0].Substring(1)));
            }

        }


    } // End of class
}
