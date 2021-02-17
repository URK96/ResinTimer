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
                AppResources.Gadget_Type_ParametricTransformer
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

        private async void ApplyButtonClicked(object sender, EventArgs e)
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