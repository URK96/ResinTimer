using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using GdEnv = ResinTimer.GardeningEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditGardeningItemPage : ContentPage
    {
        GardeningNoti Noti { get; set; }

        NotiManager.EditType editType;
        NotiManager notiManager;

        public EditGardeningItemPage(NotiManager notiManager, NotiManager.EditType type, GardeningNoti noti = null)
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
            GardeningItemTypePicker.ItemsSource = new List<string>
            {
                AppResources.Gardening_JadeField,
                AppResources.Gardening_LuxuriantGlebe,
                AppResources.Gardening_OrderlyMeadow
            };

            if (editType == NotiManager.EditType.Edit)
            {
                GardeningItemTypePicker.SelectedIndex = (int)Noti.FieldType;
                GardeningItemNoteEntry.Text = Noti.ItemNote;
            }
            else
            {
                GardeningItemTypePicker.SelectedIndex = 0;
                GardeningItemNoteEntry.Text = string.Empty;
            }
        }

        private async void ApplySetting()
        {
            try
            {
                var type = (GdEnv.FieldType)GardeningItemTypePicker.SelectedIndex;
                var note = GardeningItemNoteEntry.Text;
                var manager = notiManager as GardeningNotiManager;

                if (editType == NotiManager.EditType.Add)
                {
                    Noti = new GardeningNoti(type);
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