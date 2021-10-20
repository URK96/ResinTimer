using ResinTimer.Resources;
using ResinTimer.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ResinTimer
{
    public class GenshinWorldMapPage : WebViewPage
    {
        private const string GenshinWorldMapURL = "https://webstatic-sea.mihoyo.com/app/ys-map-sea/index.html";

        public GenshinWorldMapPage() : base(GenshinWorldMapURL)
        {
            Title = AppResources.MasterDetail_MasterList_Web_GenshinWorldMap;
        }
    }
}