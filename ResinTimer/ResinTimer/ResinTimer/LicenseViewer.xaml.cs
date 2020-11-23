
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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