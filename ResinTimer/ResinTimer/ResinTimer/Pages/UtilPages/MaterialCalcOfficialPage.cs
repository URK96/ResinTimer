using ResinTimer.Resources;

namespace ResinTimer.Pages.UtilPages
{
    public class MaterialCalcOfficialPage : WebViewBasePage
    {
        private const string MaterialCalcOfficialURL = "https://webstatic-sea.mihoyo.com/ys/event/calculator-sea/index.html";

        public MaterialCalcOfficialPage() : base(MaterialCalcOfficialURL)
        {
            Title = AppResources.MasterDetail_MasterList_Web_MaterialCalcOfficail;
        }
    }
}