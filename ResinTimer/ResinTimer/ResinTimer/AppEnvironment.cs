using System;
using System.Collections.Generic;
using System.Text;

using GenshinDB_Core;

namespace ResinTimer
{
    public static class AppEnvironment
    {
        public static bool isDebug = false;
        public static bool isRunningNotiThread = false;

        public static GenshinDB genshinDB;
    }
}
