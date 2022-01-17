using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewBasePage : ContentPage
    {
        public StackLayout RootLayout => WebViewRootLayout;
        public WebView WebView => BaseWebView;

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