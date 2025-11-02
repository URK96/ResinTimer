using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace ResinTimer.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewBasePage : ContentPage
    {
        public StackLayout RootLayout => WebViewRootLayout;
        public WebView WebViewControl => BaseWebView;

        public WebViewBasePage()
        {
            InitializeComponent();
        }

        public WebViewBasePage(string url) : this()
        {
            NavigateURL(url);
        }

        public void NavigateURL(string url)
        {
            BaseWebView.Source = url;
            BaseWebView.Reload();
        }
    }
}