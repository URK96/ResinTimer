using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPage : ContentPage
    {
        public EditPage()
        {
            InitializeComponent();
        }

        private void OkButtonClicked(object sender, EventArgs e)
        {
            SetValue();
            CalcRemainTimeResin();
            SaveValue();

            OnBackButtonPressed();
        }

        private void SetValue()
        {
            int hour, min, sec;

            int.TryParse(NowTotalTimeHour.Text, out hour);
            int.TryParse(NowTotalTimeMinute.Text, out min);
            int.TryParse(NowTotalTimeSecond.Text, out sec);

            ResinEnvironment.totalCountTime.SetTime(hour, min, sec);
        }

        private void CalcRemainTimeResin()
        {
            var now = DateTime.Now;
            int totalSec = ResinEnvironment.totalCountTime.TotalSec;
            int remainResinCount = totalSec / ResinTime.ONE_RESTORE_INTERVAL;

            ResinEnvironment.oneCountTime.SetTime(totalSec % ResinTime.ONE_RESTORE_INTERVAL);
            ResinEnvironment.endTime = now.AddHours(ResinEnvironment.totalCountTime.Hour).AddMinutes(ResinEnvironment.totalCountTime.Min).AddSeconds(ResinEnvironment.totalCountTime.Sec);
            //ResinEnvironment.resin = ResinEnvironment.MAX_RESIN - remainResinCount - 1;

            //if (ResinEnvironment.resin < 0)
            //{
            //    ResinEnvironment.resin = 0;
            //}
        }

        private void SaveValue()
        {
            Preferences.Set(SettingConstants.RESIN_COUNT, ResinEnvironment.resin);
            Preferences.Set(SettingConstants.END_TIME, ResinEnvironment.endTime.ToString());
        }
    }
}