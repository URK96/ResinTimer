using ResinTimer.Resources;
using ResinTimer.Services;
using ResinTimer.ViewModels;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages.UtilPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DailyRewardPage : TabbedPage, INotifyPropertyChanged
    {
        private DailyRewardPageViewModel _viewModel;
        private CancellationTokenSource _rewardUpdateTaskCancelTokenSource;
        private bool _isRunningPageUpdate = false;
        private bool _isRunningCheckInUpdate = false;

        public DailyRewardPage()
        {
            InitializeComponent();

            ItemsSource = new DailyRewardPageViewModel[]
            {
                new(DailyRewardPageViewModel.GameTypeEnum.Genshin),
                new(DailyRewardPageViewModel.GameTypeEnum.Honkai3rd),
                new(DailyRewardPageViewModel.GameTypeEnum.HonkaiStarRail),
                new(DailyRewardPageViewModel.GameTypeEnum.ZenlessZoneZero)
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _rewardUpdateTaskCancelTokenSource?.Cancel();
            _rewardUpdateTaskCancelTokenSource?.Dispose();

            _rewardUpdateTaskCancelTokenSource = null;
        }

        private async void TabbedPage_CurrentPageChanged(object sender, EventArgs e)
        {
            _viewModel = SelectedItem as DailyRewardPageViewModel;

            await DailyPageUpdateProcess();
        }

        private async Task DailyPageUpdateProcess()
        {
            _isRunningPageUpdate = true;

            _viewModel.ErrorMessageVisibled = false;
            _viewModel.CheckInButtonEnabled = false;


            //// Update Auto Check-In switch

            _viewModel.UpdateAutoCheckInStatus();


            //// Run Reward Update Task

            _rewardUpdateTaskCancelTokenSource = new();

            if (await _viewModel.UpdateTodayRewardInfo(_rewardUpdateTaskCancelTokenSource.Token))
            {
                await _viewModel.UpdateCheckInStatus();
            }
            else
            {
                _viewModel.ErrorMessageVisibled = true;
            }

            _isRunningPageUpdate = false;
        }

        private void AutoCheckInSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (_isRunningPageUpdate ||
                _isRunningCheckInUpdate)
            {
                return;
            }

            bool isEnabled = e.Value;

            Debug.WriteLine("Toggled");

            try
            {
                if (isEnabled)
                {
                    _viewModel?.RegisterAutoCheckIn();
                }
                else
                {
                    _viewModel?.UnregisterAutoCheckIn();
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
                _isRunningCheckInUpdate = true;

                _viewModel.UpdateAutoCheckInStatus();

                _isRunningCheckInUpdate = false;
            }
        }

        private async void CheckInButton_Clicked(object sender, EventArgs e)
        {
            await _viewModel?.CheckInTodayReward();
        }

        private async void GoToWebsiteButton_Clicked(object sender, EventArgs e)
        {
            string url = _viewModel.GameType switch
            {
                DailyRewardPageViewModel.GameTypeEnum.Genshin => DailyCheckInEventPage.DailyCheckInUrl,
                DailyRewardPageViewModel.GameTypeEnum.Honkai3rd => DailyCheckInEventPage.DailyCheckInUrlHonkai3rd,
                DailyRewardPageViewModel.GameTypeEnum.HonkaiStarRail => DailyCheckInEventPage.DailyCheckInUrlHonkaiStarRail,
                DailyRewardPageViewModel.GameTypeEnum.ZenlessZoneZero => DailyCheckInEventPage.DailyCheckInUrlZenlessZoneZero,

                _ => string.Empty
            };

            await Navigation.PushAsync(new DailyCheckInEventPage(url));
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