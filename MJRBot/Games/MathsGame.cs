using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJRBot
{
    class MathsGame
    {
        public static Random rand = new Random();
        public static int rannum1;
        public static int rannum2;
        public static int ransign;
        public static String sign = "+";
        public static bool isMathsGameActive = false;

        public static int Answer;

        public static String CreateQuestion()
        {
            rannum1 = rand.Next(1,100);
            rannum2 = rand.Next(1, 100);
            ransign = rand.Next(1, 3);

            switch (ransign)
            {
                case 1:
                    sign = "+";
                    Answer = rannum1 + rannum2;
                    break;
                case 2:
                    sign = "-";
                    Answer = rannum1 - rannum2;
                    break;
                case 3:
                    sign = "*";
                    Answer = rannum1 * rannum2;
                    break;
            }
            return "The question is " + rannum1.ToString() + sign + rannum2.ToString();
        }
    }
}
