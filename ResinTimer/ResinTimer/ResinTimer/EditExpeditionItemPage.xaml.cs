using ResinTimer.Resources;

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ExpEnv = ResinTimer.ExpeditionEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditExpeditionItemPage : ContentPage
    {
        ExpeditionNoti Noti { get; set; }

        NotiManager.EditType editType;
        NotiManager notiManager;

        public EditExpeditionItemPage(NotiManager notiManager, NotiManager.EditType type, ExpeditionNoti noti = null)
        {
            editType = type;
            this.notiManager = notiManager;

            Noti = noti;

            InitializeComponent();

            Title = (editType == NotiManager.EditType.Add) ? AppResources.EditExpedition_Title_New : AppResources.EditExpedition_Title_Edit;

            InitValues();
        }

        private void InitValues()
        {
            ExpeditionTypePicker.ItemsSource = new List<string>
            {
                AppResources.Expedition_Type_Chunk,
                AppResources.Expedition_Type_Ingredient,
                AppResources.Expedition_Type_Mora
            };
            ExpeditionTimePicker.ItemsSource = new List<string>
            {
                "4H",
                "8H",
                "12H",
                "20H"
            };

            if (editType == NotiManager.EditType.Edit)
            {
                ExpeditionTypePicker.SelectedIndex = (int)Noti.ExpeditionType;
                ExpeditionTimePicker.SelectedIndex = Noti.StandardTime.Hours switch
                {
                    8 => 1,
                    12 => 2,
                    20 => 3,
                    _ => 0
                };
                ExpeditionEffectCheckBox.IsChecked = Noti.StandardTime != Noti.ExpeditionTime;
            }
            else
            {
                ExpeditionTypePicker.SelectedIndex = 0;
                ExpeditionTimePicker.SelectedIndex = 0;
            }
        }

        private async void ApplySetting()
        {
            var type = (ExpEnv.ExpeditionType)ExpeditionTypePicker.SelectedIndex;
            var time = TimeSpan.FromHours(ExpeditionTimePicker.SelectedIndex switch
            {
                1 => 8,
                2 => 12,
                3 => 20,
                _ => 4
            });
            bool applyTimeEffect = ExpeditionEffectCheckBox.IsChecked;
            var manager = notiManager as ExpeditionNotiManager;

            if (editType == NotiManager.EditType.Add)
            {
                Noti = new ExpeditionNoti(time, type, ExpeditionEffectCheckBox.IsChecked);
            }
            else if (editType == NotiManager.EditType.Edit)
            {
                Noti.EditTime(time, applyTimeEffect);
                Noti.ExpeditionType = type;
            }

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
            }
        }
    }
}