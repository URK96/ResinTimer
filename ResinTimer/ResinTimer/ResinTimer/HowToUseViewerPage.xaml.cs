using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ResinTimer.Resources;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HowToUseViewerPage : ContentPage
    {
        public HowToUseViewerPage(string category)
        {
            InitializeComponent();
        }
    }

    public class HowToUseContent
    {
        public string Image { get; }
        public string ExplainText { get; }

        public HowToUseContent(string category)
        {
            
        }
    }
}