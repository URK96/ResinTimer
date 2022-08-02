using GenshinInfo.Models;

using ResinTimer.Helper;
using ResinTimer.Resources;
using ResinTimer.Services;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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

            DailyRewardCheckInButton.BorderColor = Color.Default;

            DailyRewardInfoLayout.IsVisible = false;
            ServerErrorLabel.IsVisible = false;
            DailyRewardCheckInButton.IsEnabled = false;
            DailyRewardLoadingIndicator.IsRunning = true;


            // Update Auto Check-In switch

            UpdateAutoCheckInStatus();


            // Run Reward Update Task

            _rewardUpdateTaskCancelTokenSource = new();

            if (await UpdateTodayRewardInfo(_rewardUpdateTaskCancelTokenSource.Token))
            {
                DailyRewardInfoLayout.IsVisible = true;
                DailyRewardCheckInButton.IsEnabled = true;


                DailyRewardCheckInButton.BorderColor =
                    (await DailyRewardHelper.CheckInTodayDailyReward() is DailyRewardHelper.SignInResult.AlreadySignIn) ?
                    Color.Green : Color.FromHex("#0682F6");
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

        private void UpdateAutoCheckInStatus()
        {
            AutoCheckInSwitch.Toggled -= AutoCheckInSwitch_Toggled;

            AutoCheckInSwitch.IsToggled = DependencyService.Get<IDailyCheckInService>().IsRegistered();

            AutoCheckInSwitch.Toggled += AutoCheckInSwitch_Toggled;
        }

        private async Task<bool> UpdateTodayRewardInfo(CancellationToken cancelToken)
        {
            using WebClient wc = new();

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

                if (cancelToken.IsCancellationRequested)
                {
                    return false;
                }

                ImageSource iconSource = ImageSource.FromUri(new Uri(itemData.IconUrl));

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

            try
            {
                if (isEnabled)
                {
                    DependencyService.Get<IDailyCheckInService>().Register();
                }
                else
                {
                    DependencyService.Get<IDailyCheckInService>().Unregister();
                }

                DependencyService.Get<IToast>().Show(isEnabled ? 
                    AppResources.DailyRewardPage_AutoCheckIn_Register_Success : 
                    AppResources.DailyRewardPage_AutoCheckIn_Unregister_Success);
            }
            catch
            {
                DependencyService.Get<IToast>().Show(isEnabled ?
                    AppResources.DailyRewardPage_AutoCheckIn_Register_Fail :
                    AppResources.DailyRewardPage_AutoCheckIn_Unregister_Fail);
            }
            finally
            {
                UpdateAutoCheckInStatus();
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