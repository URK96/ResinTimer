﻿namespace ResinTimer
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

        public const string ITEM_TALENT_LOCATION = "Item_Talent_Location";
        public const string ITEM_TALENT_SERVER = "Item_Talent_Server";

        public const string ITEM_WEAPONASCENSION_LOCATION = "Item_WeaponAscension_Location";

        public const string NOTI_LIST = "Noti_List";
        public const string EXPEDITION_NOTI_LIST = "Expedition_Noti_List";
        public const string GATHERINGITEM_NOTI_LIST = "GatheringItem_Noti_List";
        public const string GADGET_NOTI_LIST = "Gadget_Noti_List";
        public const string FURNISHING_NOTI_LIST = "Furnishing_Noti_List";
        public const string CHECKLIST_LIST = "Checklist_List";
        
        // Setting - App
        public const string NOTI_ENABLED = "Noti_Enabled";
        public const string APP_START_DETAILSCREEN = "App_Start_DetailScreen";
        public const string APP_USE_24H_TIMEFORMAT = "App_Use_24H_TimeFormat";
        public const string APP_LANG = "App_Lang";
        public const string APP_INGAMESERVER = "App_InGameServer";

        // Setting - Main
        public const string QUICKCALC_VIBRATION = "QuickCalc_Vibration";

        // Android Specific
        public const string WIDGET_BACKGROUND = "Widget_Background";
    }
}