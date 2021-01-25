using System;

namespace ResinTimer
{
    public static class SettingConstants
    {
        enum StartScreen
        {
            Resin = 0,
            Expedition,
            GatheringItem
        }

        public const string RESIN_COUNT = "Resin_Count";
        public const string LAST_INPUT_TIME = "Last_Input_Time";
        public const string END_TIME = "End_Time";
        public const string RESIN_INPUT_TYPE = "Resin_Input_Type";

        public const string NOTI_LIST = "Noti_List";
        public const string EXPEDITION_NOTI_LIST = "Expedition_Noti_List";
        public const string GATHERINGITEM_NOTI_LIST = "GatheringItem_Noti_List";

        public const string QUICKCALC_VIBRATION = "QuickCalc_Vibration";
        public const string NOTI_ENABLED = "Noti_Enabled";

        public const string APP_START_DETAILSCREEN = "App_Start_DetailScreen";

        // Android Specific
        public const string WIDGET_BACKGROUND = "Widget_Background";
    }
}