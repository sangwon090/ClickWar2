using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Utility
{
    public class Random
    {
        protected static System.Random s_random = new System.Random();

        public static int Next()
        {
            return s_random.Next();
        }

        public static int Next(int max)
        {
            return s_random.Next(max);
        }

        public static int Next(int min, int max)
        {
            return s_random.Next(min, max);
        }
    }
}
