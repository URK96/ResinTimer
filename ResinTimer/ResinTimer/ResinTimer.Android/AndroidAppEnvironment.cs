﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResinTimer.Droid
{
    public static class AndroidAppEnvironment
    {
        public const string CHANNEL_ID = "ResinTimerNoti";

        // Widget Global Constant
        public const string KEY_CLICKUPDATE = "Key_ClickUpdate";
        public const string VALUE_CLICKUPDATE = "Value_ClickUpdate";
        public const string KEY_RUNAPP = "Key_RunApp";
        public const string VALUE_RUNAPP = "Value_RunApp";

        // Talent Widget Constant
        public const string KEY_TALENTITEM_CLICK = "Key_TalentItem_Click";
        public const string VALUE_TALENTITEM_CLICK = "Value_TalentItem_Click";
        public const string KEY_TALENTITEM_LIST = "Key_TalentItem_List";

        // WA Widget Constant
        public const string KEY_WAITEM_CLICK = "Key_WAItem_Click";
        public const string VALUE_WAITEM_CLICK = "Value_WAItem_Click";
        public const string KEY_WAITEM_LIST = "Key_WAItem_List";
    }
}