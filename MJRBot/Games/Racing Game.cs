using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MJRBot
{
    class Racing_Game
    {
        public static String[,] bets = new String[4,1000];
        private static String[] Top3Users;
        private static String[] WinnerUsers;
        public static int BetNumber = 0;
        private static Random random = new Random();
        public static bool StartedRace = false;

        public static int[] cars = new int[8];
        private static String[] WinnersUsers;

        public static Thread raceTimer = new Thread(raceCountdown);

        public static void PlaceBet(String User, String Bet, String type, String points) {
	        bets[0,BetNumber] = User.ToLower();
	        bets[1,BetNumber] = Bet.ToLower();
	        bets[2,BetNumber] = type.ToLower();
	        bets[3,BetNumber] = points.ToLower();
	        BetNumber++;
        }

        public static void Start() {
	        bool Good = false;
            bool runAgain = false;
	        for (int i = 0; i < 8; i++) {
                Console.WriteLine("Car: " + i);
                Good = false;
                runAgain = false;
                do
                {
                    runAgain = false;
                    int number = random.Next(1, 8);
                    Console.WriteLine("Number: " + number);
                    foreach (int car in cars)
                    {
                        if (runAgain != true)
                        {
                            if (car.Equals(number))
                            {
                                Console.WriteLine("Yes");
                                Good = false;
                                runAgain = true;
                            }
                            else
                            {
                                Console.WriteLine("No");
                                cars[i] = number;
                                Good = true;
                                runAgain = false;
                            }
                        }
                    }
                }
                while (Good == true && runAgain == false);
                
	        }
	        CheckForWinners();
        }

        public static void CheckForWinners()
        {
            if (BetNumber == 0)
            {
                BotClient.sendChatMessage("No one made any bets! So race got canceled!");
                return;
            }
            String WinnerBetWinners = "";
            String Top3BetWinners = "";

            bool Results = false;

            BotClient.sendChatMessage(
                "First Place was Car " + cars[0] + ", Second Place was Car " + cars[1] + ", Third Place was Car " + cars[2]);
            for (int k = 0; k < BetNumber; k++)
            {
                if (bets[2, k].ToLower().Equals("top3"))
                {
                    if (bets[1, k].Equals(cars[0].ToString()) || bets[1, k].Equals(cars[1].ToString()) || bets[1, k].Equals(cars[2].ToString()))
                    {
                        if(Top3BetWinners.Length < 1)
                            Top3BetWinners = Top3BetWinners + bets[0, k];
                        else
                            Top3BetWinners = Top3BetWinners + bets[0, k];
                    }
                }

                if (bets[2, k].ToLower().Equals("1st"))
                {
                    if (bets[1, k].Equals(cars[0].ToString()))
                    {
                        if (WinnerBetWinners.Length < 0)
                            WinnerBetWinners = WinnerBetWinners + bets[0, k] + ", ";
                        else
                            WinnerBetWinners = WinnerBetWinners + bets[0, k] + ", ";
                    }
                }
            }
            if (Top3BetWinners.Length < 1 && WinnerBetWinners.Length < 1)
                Results = false;
            else
                Results = true;

            if (Results)
            {
                Top3Users = Top3BetWinners.Replace(" ", "").Split(',');
                WinnerUsers = WinnerBetWinners.Replace(" ", "").Split(',');

                String pointsMessage = "User ";
                String Top3UsersWin = "";
                foreach (String user in Top3Users)
                {
                    Top3UsersWin += " " + user;
                }
                String WinnerUsersWin = "";
                foreach (String user in WinnerUsers)
                {
                    WinnerUsersWin += " " + user;
                }
                String Message = "Top 3 winners are [" + Top3UsersWin + "] and 1st place winners are [" + WinnerUsersWin+ "]";
                Message.Replace("[", "[ ");
                Message.Replace("]", " ]");
                BotClient.sendChatMessage(Message);
                if (Top3BetWinners.Length != 0)
                {
                    double randomOds = 1 + random.NextDouble();
                    for (int l = 0; l < Top3Users.Length; l++)
                    {
                        int points = 0;
                        for (int i = 0; i < BetNumber; i++)
                        {
                            if (Top3Users[l].ToLower().Equals(bets[0, i]))
                            {
                                points = (int)Math.Ceiling((randomOds * Convert.ToInt32(bets[3, i])));
                            }
                        }
                        if (Top3Users.Length <= 1)
                            pointsMessage = pointsMessage + Top3Users[l] + " has won " + points;
                        else
                            pointsMessage = pointsMessage + Top3Users[l] + " has won " + points + ", ";
                        PointsFile.AddPoints(Top3Users[l], points);
                    }
                }
                if (WinnerBetWinners.Length != 0)
                {
                    double randomOds = 1 + random.NextDouble();
                    for (int m = 0; m < WinnersUsers.Length; m++)
                    {
                        int points = 0;
                        for (int i = 0; i < BetNumber; i++)
                        {
                            if (WinnersUsers[m].ToLower().Equals(bets[0, i]))
                            {
                                points = (int)Math.Ceiling((randomOds * Convert.ToInt32(bets[3, i])));
                                points = points * 2;
                            }
                        }
                        if (WinnerUsers.Length <= 1)
                            pointsMessage = pointsMessage + WinnersUsers[m] + " has won " + points;
                        else
                            pointsMessage = pointsMessage + WinnersUsers[m] + " has won " + points + ", ";

                        PointsFile.AddPoints(WinnerUsers[m], points);
                    }
                }
                BotClient.sendChatMessage(pointsMessage);
            }
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 1000; j++)
                    bets[i, j] = "";

            Top3Users = new String[0];
            WinnerUsers = new String[0];
            StartedRace = false;
        }
        public static bool checkForValue(String val)
        {
            for (int i = 0; i < BetNumber; i++)
            {
                if (bets[0,i].Contains(val))
                    return true;
            }
            return false;
        }

        public static void raceCountdown()
        {
            while (true)
            {
                if (StartedRace)
                {
                    Console.WriteLine("running loop..");
                    Thread.Sleep(4000);
                    Start();
                }
                Thread.Sleep(4000);
            }
        }
    }
}
