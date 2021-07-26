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
                AppResources.GatheringItem_Type_IronChunk,
                AppResources.GatheringItem_Type_ElectroCrystal,
                AppResources.GatheringItem_Type_CrystalCore,
                AppResources.GatheringItem_Type_AmethystLump
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

        private async void ApplySetting()
        {
            GIEnv.GItemType type = (GIEnv.GItemType)GatheringItemTypePicker.SelectedIndex;
            string note = GatheringItemNoteEntry.Text;
            GatheringItemNotiManager manager = notiManager as GatheringItemNotiManager;

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

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0:  // Apply
                    ApplySetting();
                    break;
                default:
                    break;
            }
        }
    }
}