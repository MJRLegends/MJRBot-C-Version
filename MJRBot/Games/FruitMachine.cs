using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJRBot
{
    public class FruitMachine
    {
        public static int Slot1, Slot2, Slot3;

        public static String[] emotes = { "MrDestructoid", "MechaSupes", "SSSsss", "PJSalt", "NightBat", "KZskull", "KAPOW",
	    "GasJoker", "FuzzyOtterOO" };

        public static String Spin()
        {
            Slot1 = 0;
            Slot2 = 0;
            Slot3 = 0;
            Random random = new Random();
            Slot1 = random.Next(1,8);
            Slot2 = random.Next(1, 8);
            Slot3 = random.Next(1, 8);
            return emotes[Slot1] + " " + emotes[Slot2] + " " + emotes[Slot3];
        }

        public static bool hasWon()
        {
            if (Slot1 == Slot2 && Slot1 == Slot3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
