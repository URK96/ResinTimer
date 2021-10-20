using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using FEnv = ResinTimer.FurnishingEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditFurnishingItemPage : ContentPage
    {
        FurnishingNoti Noti { get; set; }

        NotiManager.EditType editType;
        NotiManager notiManager;

        public EditFurnishingItemPage(NotiManager notiManager, NotiManager.EditType type, FurnishingNoti noti = null)
        {
            editType = type;
            this.notiManager = notiManager;

            Noti = noti;

            InitializeComponent();

            Title = (editType == NotiManager.EditType.Add) ? AppResources.EditItemPage_Title_New : AppResources.EditItemPage_Title_Edit;

            InitPicker();
        }

        private void InitPicker()
        {
            FurnishingItemTypePicker.ItemsSource = new List<string>
            {
                AppResources.Furnishing_Rarity_2,
                AppResources.Furnishing_Rarity_3,
                AppResources.Furnishing_Rarity_4
            };

            if (editType == NotiManager.EditType.Edit)
            {
                FurnishingItemTypePicker.SelectedIndex = (int)Noti.ItemType;
                FurnishingItemNoteEntry.Text = Noti.ItemNote;
            }
            else
            {
                FurnishingItemTypePicker.SelectedIndex = 0;
                FurnishingItemNoteEntry.Text = string.Empty;
            }
        }

        private async void ApplySetting()
        {
            try
            {
                var type = (FEnv.FurnishType)FurnishingItemTypePicker.SelectedIndex;
                var note = FurnishingItemNoteEntry.Text;
                var manager = notiManager as FurnishingNotiManager;

                if (editType == NotiManager.EditType.Add)
                {
                    Noti = new FurnishingNoti(type);
                }
                else if (editType == NotiManager.EditType.Edit)
                {
                    Noti.EditItemType(type);
                }

                Noti.ItemNote = note;

                manager.EditList(Noti, editType);

                await Navigation.PopAsync();
            }
            catch { }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0:  // Apply
                    ApplySetting();
                    break;
            }
        }
    }
}