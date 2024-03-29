﻿using ResinTimer.Models.Materials;

using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseMaterialTimerPage : ContentPage
    {
        public List<IMaterialItem> Items { get; set; }
        public CollectionView ListView => ListCollectionView;

        public BaseMaterialTimerPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        internal virtual void ShowDetailInfo(object selectedItem) { }

        private void ToolbarItemClicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0: // Noti Setting
                    break;
                default:
                    break;
            }
        }

        private void ListCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                ShowDetailInfo(e.CurrentSelection.FirstOrDefault());
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Utils.RefreshCollectionView(ListCollectionView, Items);
        }
    }
}