using ResinTimer.Resources;

namespace ResinTimer.Pages.UtilPages
{
    public class GenshinWorldMapPage : WebViewBasePage
    {
        private const string GenshinWorldMapURL = "https://webstatic-sea.mihoyo.com/app/ys-map-sea/index.html";

        public GenshinWorldMapPage() : base(GenshinWorldMapURL)
        {
            Title = AppResources.MasterDetail_MasterList_Web_GenshinWorldMap;
        }
    }
}