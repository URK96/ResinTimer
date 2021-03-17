using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeEditView : ContentView
    {
        private readonly Noti noti;
        private readonly NotiManager notiManager;
        private readonly int maxHour;

        public TimeEditView(Noti noti, NotiManager notiManager)
        {
            this.noti = noti;
            this.notiManager = notiManager;
            maxHour = GetMaxHour();

            InitializeComponent();

            InitValue();
        }

        private void InitValue()
        {
            GetMaxHour();

            var ts = noti.NotiTime - DateTime.Now;

            TimeHour.Text = ts.Hours.ToString();
            TimeMinute.Text = ts.Minutes.ToString();
        }

        private int GetMaxHour()
        {
            int hour = 0;

            if (noti is ExpeditionNoti expeditionNoti)
            {
                hour = expeditionNoti.ExpeditionTime.Hours;
            }
            else if (noti is GatheringItemNoti giNoti)
            {
                hour = giNoti.ResetTime.Hours;
            }
            else if (noti is GadgetNoti gadgetNoti)
            {
                hour = gadgetNoti.ResetTime.Hours;
            }

            return hour;
        }

        private int GetInputTimeToMinute() => int.Parse(TimeHour.Text) * 60 + int.Parse(TimeMinute.Text);

        private void TimeEntry_Completed(object sender, EventArgs e)
        {
            if (GetInputTimeToMinute() > (maxHour * 60))
            {
                TimeHour.Text = maxHour.ToString();
                TimeMinute.Text = "0";
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var btText = (sender as Button).Text;

            if (btText.Equals(AppResources.Dialog_Ok))
            {
                noti.NotiTime = DateTime.Now.AddMinutes(GetInputTimeToMinute());

                notiManager.EditList(noti, NotiManager.EditType.EditOnlyTime);
            }

            await PopupNavigation.Instance.PopAsync();
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