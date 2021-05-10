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
        public WebView WebView => BaseWebView;

        public WebViewPage(string url)
        {
            InitializeComponent();

            NavigateURL(url);
        }

        private void NavigateURL(string url)
        {
            BaseWebView.Source = url;
            BaseWebView.Reload();
        }
    }
}