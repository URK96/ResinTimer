using GenshinInfo.Models;

using ResinTimer.Helper;
using ResinTimer.Resources;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages.UtilPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DailyRewardPage : ContentPage
    {
        private CancellationTokenSource _rewardUpdateTaskCancelTokenSource;

        public DailyRewardPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            DailyRewardInfoLayout.IsVisible = false;
            ServerErrorLabel.IsVisible = false;
            DailyRewardCheckInButton.IsEnabled = false;
            DailyRewardLoadingIndicator.IsRunning = true;


            // Update Auto Check-In switch

            AutoCheckInSwitch.Toggled -= AutoCheckInSwitch_Toggled;

            AutoCheckInSwitch.IsToggled = Preferences.Get(SettingConstants.DAILYREWARD_ENABLE_AUTOCHECKIN, false);

            AutoCheckInSwitch.Toggled += AutoCheckInSwitch_Toggled;


            // Run Reward Update Task

            _rewardUpdateTaskCancelTokenSource = new();

            bool updateResult = await UpdateTodayRewardInfo(_rewardUpdateTaskCancelTokenSource.Token);

            if (updateResult)
            {
                DailyRewardInfoLayout.IsVisible = true;
                DailyRewardCheckInButton.IsEnabled = true;
            }
            else
            {
                ServerErrorLabel.IsVisible = true;
            }

            DailyRewardLoadingIndicator.IsRunning = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _rewardUpdateTaskCancelTokenSource?.Cancel();
            _rewardUpdateTaskCancelTokenSource?.Dispose();
        }

        private async Task<bool> UpdateTodayRewardInfo(CancellationToken cancelToken)
        {
            bool updateResult = true;

            if (cancelToken.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                DailyRewardListItemData itemData = await DailyRewardHelper.GetNowDailyRewardItem();

                if (cancelToken.IsCancellationRequested)
                {
                    return false;
                }

                TodayRewardName.Text = itemData.ItemName;

                string tempFilePath = System.IO.Path.Combine(FileSystem.CacheDirectory, "daily_reward.png");

                if (cancelToken.IsCancellationRequested)
                {
                    return false;
                }

                await new WebClient().DownloadFileTaskAsync(itemData.IconUrl, tempFilePath);

                if (cancelToken.IsCancellationRequested)
                {
                    return false;
                }

                ImageSource iconSource = ImageSource.FromFile(tempFilePath);

                TodayRewardIcon.Source = iconSource;
                TodayRewardCount.Text = $"(x{itemData.ItemCount})";
            }
            catch
            {
                updateResult = false;
            }

            return updateResult;
        }

        private void AutoCheckInSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            bool isEnabled = e.Value;

            Preferences.Set(SettingConstants.DAILYREWARD_ENABLE_AUTOCHECKIN, isEnabled);

            if (isEnabled)
            {

            }
        }

        private async void CheckInButton_Clicked(object sender, EventArgs e)
        {
            DailyRewardCheckInButton.IsEnabled = false;

            string message = await DailyRewardHelper.CheckInTodayDailyReward() switch
            {
                DailyRewardHelper.SignInResult.Success => AppResources.DailyReward_CheckIn_Success,
                DailyRewardHelper.SignInResult.AlreadySignIn => AppResources.DailyReward_CheckIn_AlreadySignIn,
                _ => AppResources.DailyReward_CheckIn_Fail
            };

            DependencyService.Get<IToast>().Show(message);

            DailyRewardCheckInButton.IsEnabled = true;
        }

        private async void GoToWebsiteButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DailyCheckInEventPage());
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