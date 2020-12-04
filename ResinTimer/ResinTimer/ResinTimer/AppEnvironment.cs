using System;
using System.Collections.Generic;
using System.Text;

namespace ResinTimer
{
    public static class AppEnvironment
    {
        public static bool isDebug = false;
        public static bool isRunningNotiThread = false;

        public static bool SetAlarm(DateTime dt)
        {
            try
            {

            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
