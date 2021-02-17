using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ResinTimer.Resources;

using ManualCategory = ResinTimer.AppEnvironment.ManualCategory;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HowToUseListPage : ContentPage
    {
        public List<HowToUseCategory> Categories => new List<HowToUseCategory>
        {
            new HowToUseCategory(AppResources.HowToUse_Category_Timer_Resin),
            new HowToUseCategory(AppResources.HowToUse_Category_Timer_Expedition),
            new HowToUseCategory(AppResources.HowToUse_Category_Timer_GatheringItem),
            new HowToUseCategory(AppResources.HowToUse_Category_Timer_Gadget),
            new HowToUseCategory(AppResources.HowToUse_Category_Timer_TalentBook),
            new HowToUseCategory(AppResources.HowToUse_Category_Widget_Resin),
            new HowToUseCategory(AppResources.HowToUse_Category_Widget_TalentBook)
        };

        public HowToUseListPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        private async void ListCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count < 1)
            {
                return;
            }

            var cv = sender as CollectionView;

            var category = e.CurrentSelection.FirstOrDefault() as string;

            cv.SelectedItems = null;
            cv.SelectedItem = null;

            await Navigation.PushAsync(new HowToUseViewerPage(category), true);
        }
    }

    public class HowToUseCategory
    {
        public string Category { get; }
        public ManualCategory MCategory { get; }

        public HowToUseCategory(ManualCategory manualCategory)
        {
            MCategory = manualCategory;

            Category = manualCategory switch
            {
                ManualCategory.TimerResin => AppResources.HowToUse_Category_Timer_Resin,
                ManualCategory.TimerExpedition => AppResources.HowToUse_Category_Timer_Expedition,
                ManualCategory.TimerGatheringItem => AppResources.HowToUse_Category_Timer_GatheringItem,
            };
        }
    }
}