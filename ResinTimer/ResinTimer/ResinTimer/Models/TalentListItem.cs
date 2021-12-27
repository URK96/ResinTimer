﻿using GenshinDB_Core.Types;

using ResinTimer.Resources;

using AppEnv = ResinTimer.AppEnvironment;

namespace ResinTimer.Models
{
    public class TalentListItem
    {
        public TalentItem Item { get; set; }
        public string ItemName => 
            Item.ItemName.Equals("All") ? AppResources.TalentItemTimerPage_NowBook_All :
            AppEnv.genshinDB.FindLangDic(Item.ItemName);
        public string LocationName => AppEnv.genshinDB.FindLangDic(
            AppEnv.genshinDB.GetLocationName(Item.Location));
        public string ItemImageString
        {
            get
            {
                return Item.ItemName switch
                {
                    "All" => $"talent_all_{Item.Location:F}.png",
                    _ => $"talent_{Item.ItemName.Replace(" ", "_").ToLower()}.png"
                };
            }
        }

        public TalentListItem(TalentItem item)
        {
            Item = item;
        }
    }
}
