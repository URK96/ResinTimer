using ResinTimer.Resources;

using Xamarin.Forms;

namespace ResinTimer.Models
{
    public class APICheckResult
    {
        public string APIName { get; set; }
        public bool IsPass { get; set; }
        public string ResultDetail { get; set; }
        public string ResultString => IsPass ? AppResources.APICheck_Pass : 
            AppResources.APICheck_Fail;
        public Color ResultTextColor => IsPass ? Color.Green : Color.Red;
    }
}
