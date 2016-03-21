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
            try
            {
                return s_random.Next();
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }

        public static int Next(int max)
        {
            try
            {
                return s_random.Next(max);
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }

        public static int Next(int min, int max)
        {
            try
            {
                return s_random.Next(min, max);
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }
    }
}
