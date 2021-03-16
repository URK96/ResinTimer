using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LicensePage : ContentPage
    {
        public List<License> Licenses { get; set; }

        public LicensePage()
        {
            InitializeComponent();
            InitList();

            BindingContext = this;
        }

        private void InitList()
        {
            string[] names = AppResources.LibraryList.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] types = AppResources.LicenseTypeList.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Licenses = new List<License>();

            for (int i = 0; i < names.Length; ++i)
            {
                Licenses.Add(new License()
                {
                    LibraryName = names[i].Trim(),
                    LicenseType = types[i].Trim()
                });
            }

            if (Device.RuntimePlatform != Device.iOS)
            {
                Licenses.Remove(Licenses.Find(x => x.LibraryName.Contains("iOS")));
            }
        }

        private async void ListCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count < 1)
            {
                return;
            }

            var item = e.CurrentSelection.FirstOrDefault() as License;

            (sender as CollectionView).SelectedItem = null;

            await Navigation.PushAsync(new LicenseViewer(item));
        }
    }
}