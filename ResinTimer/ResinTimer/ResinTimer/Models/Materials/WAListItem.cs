using GenshinDB_Core.Types;

using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Essentials;

using AppEnv = ResinTimer.AppEnvironment;

namespace ResinTimer.Models.Materials
{
    public class WAListItem : IMaterialItem
    {
        public WeaponAscensionItem Item { get; }
        public string ItemName =>
            Item.ItemName.Equals("All") ? AppResources.WAItemTimerPage_PreLabel_All :
            AppEnv.GDB.FindLangDic(Item.ItemName);
        public string LocationName => AppEnv.GDB.FindLangDic(
            AppEnv.GDB.GetLocationName(Item.Location));
        public string ItemImageString
        {
            get
            {
                return Item.ItemName switch
                {
                    "All" => $"wa_all_{Item.Location:F}.png",
                    _ => $"wa_{Item.ItemName.Replace(" ", "_").ToLower()}_4.png"
                };
            }
        }

        public WAListItem(WeaponAscensionItem item)
        {
            Item = item;
        }
    }
}
