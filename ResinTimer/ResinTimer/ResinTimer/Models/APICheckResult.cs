using ResinTimer.Resources;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace ResinTimer.Models
{
    public class APICheckResult
    {
        public string APIName { get; set; }
        public bool IsPass { get; set; }
        public string ResultDetail { get; set; }
        public string ResultString => IsPass ? AppResources.APICheck_Pass : 
            AppResources.APICheck_Fail;
        public Color ResultTextColor => IsPass ? Colors.LightGreen : Colors.Red;
        public bool ShowDetail => !IsPass;
    }
}
