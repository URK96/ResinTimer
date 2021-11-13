using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using static GenshinInfo.Managers.GachaInfoManager;

namespace ResinTimer.Pages.UtilPages
{
    public class GachaLogViewerWeaponEventPage : GachaLogViewerBasePage
    {
        public GachaLogViewerWeaponEventPage()
        {
            Title = "Weapon";

            gachaListType = GachaTypeNum.WeaponEvent;
        }
    }
}