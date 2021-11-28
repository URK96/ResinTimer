using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : ContentPage
    {
        public StackLayout RootLayout => WebViewRootLayout;
        public WebView WebView => BaseWebView;

        public WebViewPage()
        {
            InitializeComponent();
        }

        public WebViewPage(string url) : this()
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