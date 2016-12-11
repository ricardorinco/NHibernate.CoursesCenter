using System;
using System.Collections.Generic;

namespace Persistence.Helper
{
    public class Aleatory
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        public static int GetInteger(int minimum, int maximum)
        {
            lock (syncLock)
            {
                return random.Next(minimum, maximum);
            }
        }

        public static DateTime GetDateTime(int minimum, int maximum)
        {
            DateTime start = new DateTime(minimum, 1, 1, Aleatory.GetInteger(7, 20), Aleatory.GetInteger(0, 59), Aleatory.GetInteger(0, 59));
            int range = (new DateTime(maximum, 1, 1) - start).Days;

            return start.AddDays(random.Next(range));
        }
    }
}