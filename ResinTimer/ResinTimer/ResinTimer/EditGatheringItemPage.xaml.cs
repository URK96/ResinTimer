using ResinTimer.Resources;

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using GIEnv = ResinTimer.GatheringItemEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditGatheringItemPage : ContentPage
    {
        GatheringItemNoti Noti { get; set; }

        NotiManager.EditType editType;
        NotiManager notiManager;

        public EditGatheringItemPage(NotiManager notiManager, NotiManager.EditType type, GatheringItemNoti noti = null)
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
            GatheringItemTypePicker.ItemsSource = new List<string>
            {
                AppResources.GatheringItem_Type_MagicalCrystalChunk,
                AppResources.GatheringItem_Type_Artifact,
                AppResources.GatheringItem_Type_Specialty,
                AppResources.GatheringItem_Type_Artifact12H,
                AppResources.GatheringItem_Type_CrystalChunk,
                AppResources.GatheringItem_Type_WhiteIronChunk,
                AppResources.GatheringItem_Type_IronChunk
            };

            if (editType == NotiManager.EditType.Edit)
            {
                GatheringItemTypePicker.SelectedIndex = (int)Noti.ItemType;
                GatheringItemNoteEntry.Text = Noti.ItemNote;
            }
            else
            {
                GatheringItemTypePicker.SelectedIndex = 0;
                GatheringItemNoteEntry.Text = string.Empty;
            }
        }

        private async void ApplyButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var type = (GIEnv.GItemType)GatheringItemTypePicker.SelectedIndex;
                var note = GatheringItemNoteEntry.Text;
                var manager = notiManager as GatheringItemNotiManager;

                if (editType == NotiManager.EditType.Add)
                {
                    Noti = new GatheringItemNoti(type);
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

        private async void ButtonPressed(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromHex("#500682F6");
                await button.ScaleTo(0.95, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void ButtonReleased(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.Transparent;
                await button.ScaleTo(1.0, 100, Easing.SinInOut);
            }
            catch { }
        }
    }
}