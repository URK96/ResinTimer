using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using static GenshinInfo.Managers.GachaInfoManager;

namespace ResinTimer.Pages.UtilPages
{
    public class GachaLogViewerCharacterEventPage : GachaLogViewerBasePage
    {
        public GachaLogViewerCharacterEventPage()
        {
            Title = "Character";

            gachaListType = GachaTypeNum.CharacterEvent;
        }
    }
}