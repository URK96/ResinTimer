﻿using System;
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
        public List<HowToUseCategory> Categories => CreateCategoryList();

        public HowToUseListPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        private List<HowToUseCategory> CreateCategoryList()
        {
            var list = new List<HowToUseCategory>
            {
                new HowToUseCategory(ManualCategory.TimerResin),
                new HowToUseCategory(ManualCategory.TimerRealmCurrency),
                new HowToUseCategory(ManualCategory.TimerExpedition),
                new HowToUseCategory(ManualCategory.TimerGatheringItem),
                new HowToUseCategory(ManualCategory.TimerGadget),
                new HowToUseCategory(ManualCategory.TimerFurnishing),
                new HowToUseCategory(ManualCategory.TimerTalentBook),
                new HowToUseCategory(ManualCategory.TimerWeaponAscension)
            };

            if (Device.RuntimePlatform == Device.Android)
            {
                list.Add(new HowToUseCategory(ManualCategory.WidgetResin));
                list.Add(new HowToUseCategory(ManualCategory.WidgetTalentBook));
                list.Add(new HowToUseCategory(ManualCategory.WidgetWeaponAscension));
            }

            return list;
        }

        private async void ListCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count < 1)
            {
                return;
            }

            var cv = sender as CollectionView;

            var category = e.CurrentSelection.FirstOrDefault() as HowToUseCategory;

            cv.SelectedItems = null;
            cv.SelectedItem = null;

            await Navigation.PushAsync(new HowToUseViewerPage(category.MCategory), true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ListCollectionView.SelectedItems = null;
            ListCollectionView.SelectedItem = null;
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
                ManualCategory.TimerRealmCurrency => AppResources.HowToUse_Category_Timer_RealmCurrency,
                ManualCategory.TimerExpedition => AppResources.HowToUse_Category_Timer_Expedition,
                ManualCategory.TimerGatheringItem => AppResources.HowToUse_Category_Timer_GatheringItem,
                ManualCategory.TimerGadget => AppResources.HowToUse_Category_Timer_Gadget,
                ManualCategory.TimerFurnishing => AppResources.HowToUse_Category_Timer_Furnishing,
                ManualCategory.TimerTalentBook => AppResources.HowToUse_Category_Timer_TalentBook,
                ManualCategory.TimerWeaponAscension => AppResources.HowToUse_Category_Timer_WeaponAscension,
                ManualCategory.WidgetResin => AppResources.HowToUse_Category_Widget_Resin,
                ManualCategory.WidgetTalentBook => AppResources.HowToUse_Category_Widget_TalentBook,
                ManualCategory.WidgetWeaponAscension => AppResources.HowToUse_Category_Widget_WeaponAscension,
                _ => string.Empty
            };
        }
    }
}