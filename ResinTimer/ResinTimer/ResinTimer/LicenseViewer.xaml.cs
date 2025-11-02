
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LicenseViewer : ContentPage
    {
        public LicenseViewer(License license)
        {
            InitializeComponent();

            Title = license.LicenseType;

            LicenseContent.Text = license.GetLicenseContent();
        }
    }
}