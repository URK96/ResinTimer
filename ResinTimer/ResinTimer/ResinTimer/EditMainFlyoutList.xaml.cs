using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TimerPage = ResinTimer.MainPage.TimerPage;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditMainFlyoutList : ContentPage
    {
        public List<MainMenuItem> MainMenuItems { get; }

        public EditMainFlyoutList()
        {
            InitializeComponent();

            MainMenuItems = new();

            foreach (string pageName in Enum.GetNames(typeof(TimerPage)))
            {
                MainMenuItems.Add(new MainMenuItem(pageName));
            }

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        public class MainMenuItem
        {
            public bool IsShow { get; set; }
            public string IconName { get; set; }
            public string MenuText { get; set; }

            public MainMenuItem(string menuName)
            {
                MenuText = menuName;
                IsShow = true;

                IconName = !Enum.TryParse(MenuText, out TimerPage pageType) ? string.Empty :
                    pageType switch
                    {
                        TimerPage.Resin => "resin",
                        TimerPage.RealmCurrency => "realm_currency",
                        TimerPage.RealmFriendship => "friendship",
                        TimerPage.Expedition => "compass",
                        TimerPage.GatheringItem => "silk_flower",
                        TimerPage.Gadget => "parametric_transformer",
                        TimerPage.Furnishing => "furnishing_icon",
                        TimerPage.Gardening => "gardening_jade_field",
                        TimerPage.Talent => "talent_freedom",
                        TimerPage.WeaponAscension => "wa_aerosiderite_4",
                        _ => throw new NotImplementedException()
                    };

                IconName = $"{IconName}.png";
            }
        }

        private void ListCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is MainMenuItem item)
            {
                item.IsShow = !item.IsShow;

                CollectionView cv = sender as CollectionView;

                AppEnvironment.RefreshCollectionView(cv, MainMenuItems);
                AppEnvironment.ResetCollectionViewSelection(cv);
            }
        }

        private void Apply_Clicked(object sender, EventArgs e)
        {

        }
    }
}