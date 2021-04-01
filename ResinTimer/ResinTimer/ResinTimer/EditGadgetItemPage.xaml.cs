using ResinTimer.Resources;

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using GEnv = ResinTimer.GadgetEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditGadgetItemPage : ContentPage
    {
        GadgetNoti Noti { get; set; }

        NotiManager.EditType editType;
        NotiManager notiManager;

        public EditGadgetItemPage(NotiManager notiManager, NotiManager.EditType type, GadgetNoti noti = null)
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
            GadgetItemTypePicker.ItemsSource = new List<string>
            {
                AppResources.Gadget_Type_ParametricTransformer,
                AppResources.Gadget_Type_PortableWaypoint
            };

            if (editType == NotiManager.EditType.Edit)
            {
                GadgetItemTypePicker.SelectedIndex = (int)Noti.ItemType;
                GadgetItemNoteEntry.Text = Noti.ItemNote;
            }
            else
            {
                GadgetItemTypePicker.SelectedIndex = 0;
                GadgetItemNoteEntry.Text = string.Empty;
            }
        }

        private async void ApplySetting()
        {
            try
            {
                var type = (GEnv.GadgetType)GadgetItemTypePicker.SelectedIndex;
                var note = GadgetItemNoteEntry.Text;
                var manager = notiManager as GadgetNotiManager;

                if (editType == NotiManager.EditType.Add)
                {
                    Noti = new GadgetNoti(type);
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