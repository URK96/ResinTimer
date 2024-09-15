using GenshinInfo.Models;

using ResinTimer.Helper;
using ResinTimer.Resources;
using ResinTimer.Services;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ResinTimer.ViewModels
{
    public class DailyRewardPageViewModel : ViewModelBase
    {
        public enum GameTypeEnum { Genshin, Honkai3rd, HonkaiStarRail, ZenlessZoneZero };

        public string Name => GameType switch
        {
            GameTypeEnum.Genshin => AppResources.GameType_Genshin,
            GameTypeEnum.Honkai3rd => AppResources.GameType_Honkai3rd,
            GameTypeEnum.HonkaiStarRail => AppResources.GameType_HonkaiStarRail,
            GameTypeEnum.ZenlessZoneZero => AppResources.GameType_ZenlessZoneZero,

            _ => "NULL"
        };
        public bool IsRunningUpdate
        {
            get => _isRunningUpdate;
            set
            {
                _isRunningUpdate = value;

                OnPropertyChanged(nameof(IsRunningUpdate));
            }
        }
        public string TodayRewardItemName
        {
            get => _todayRewardItemName;
            set
            {
                _todayRewardItemName = value;

                OnPropertyChanged(nameof(TodayRewardItemName));
            }
        }
        public int? TodayRewardItemCount
        {
            get => _todayRewardItemCount;
            set
            {
                _todayRewardItemCount = value;

                OnPropertyChanged(nameof(TodayRewardItemCount));
            }
        }
        public string TodayRewardItemCountString => 
            TodayRewardItemCount is null ? 
            string.Empty : 
            $"(x{TodayRewardItemCount.Value})";
        public ImageSource TodayRewardItemIcon
        {
            get => _todayRewardItemIcon;
            set
            {
                _todayRewardItemIcon = value;

                OnPropertyChanged(nameof(TodayRewardItemIcon));
            }
        }
        public bool ErrorMessageVisibled
        {
            get => _errorMessageVisibled;
            set
            {
                _errorMessageVisibled = value;

                OnPropertyChanged(nameof(ErrorMessageVisibled));
            }
        }
        public bool AutoCheckInSwitchEnabled
        {
            get => _autoCheckInSwitchEnabled;
            set
            {
                _autoCheckInSwitchEnabled = value;

                OnPropertyChanged(nameof(AutoCheckInSwitchEnabled));
            }
        }
        public bool AutoCheckInEnabled
        {
            get => _autoCheckInEnabled;
            set
            {
                _autoCheckInEnabled = value;

                OnPropertyChanged(nameof(AutoCheckInEnabled));
            }
        }
        public bool CheckInButtonEnabled
        {
            get => _checkInButtonEnabled;
            set
            {
                _checkInButtonEnabled = value;

                OnPropertyChanged(nameof(CheckInButtonEnabled));
            }
        }
        public Color CheckInButtonBorderColor => _isCheckIn ? Color.Green : Color.FromHex("#0682F6");

        public GameTypeEnum GameType = GameTypeEnum.Genshin;

        private bool _isRunningUpdate = false;
        private string _todayRewardItemName = string.Empty;
        private int? _todayRewardItemCount = null;
        private ImageSource _todayRewardItemIcon = null;
        private bool _errorMessageVisibled = false;
        private bool _autoCheckInSwitchEnabled = false;
        private bool _autoCheckInEnabled = false;
        private bool _checkInButtonEnabled = false;
        private bool _isCheckIn = false;

        public DailyRewardPageViewModel(GameTypeEnum gameType)
        {
            GameType = gameType;
        }

        internal async Task<bool> UpdateTodayRewardInfo(CancellationToken cancelToken)
        {
            using WebClient wc = new();

            bool updateResult = true;

            if (cancelToken.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                IsRunningUpdate = true;

                TodayRewardItemName = string.Empty;
                TodayRewardItemCount = null;
                TodayRewardItemIcon = null;

                DailyRewardListItemData itemData = GameType switch
                {
                    GameTypeEnum.Honkai3rd => await DailyRewardHelper.GetHonkaiNowDailyRewardItem(),
                    GameTypeEnum.HonkaiStarRail => await DailyRewardHelper.GetHonkaiStarRailNowDailyRewardItem(),
                    GameTypeEnum.ZenlessZoneZero => await DailyRewardHelper.GetZenlessZoneZeroNowDailyRewardItem(),

                    _ => await DailyRewardHelper.GetNowDailyRewardItem()
                };

                if (cancelToken.IsCancellationRequested)
                {
                    IsRunningUpdate = false;

                    return false;
                }

                TodayRewardItemName = itemData.ItemName;
                TodayRewardItemCount = itemData.ItemCount;

                if (cancelToken.IsCancellationRequested)
                {
                    IsRunningUpdate = false;

                    return false;
                }

                ImageSource iconSource = ImageSource.FromUri(new Uri(itemData.IconUrl));

                TodayRewardItemIcon = iconSource;

                CheckInButtonEnabled = true;
            }
            catch
            {
                updateResult = false;
            }
            finally
            {
                IsRunningUpdate = false;
            }

            return updateResult;
        }

        internal void UpdateAutoCheckInStatus()
        {
            if (Device.RuntimePlatform is Device.Android)
            {
                AutoCheckInEnabled = GameType switch
                {
                    GameTypeEnum.Honkai3rd => DependencyService.Get<IDailyCheckInService>().IsRegisteredHonkai(),
                    GameTypeEnum.HonkaiStarRail => DependencyService.Get<IDailyCheckInService>().IsRegisteredHonkaiStarRail(),
                    GameTypeEnum.ZenlessZoneZero => DependencyService.Get<IDailyCheckInService>().IsRegisteredZenlessZoneZero(),

                    _ => DependencyService.Get<IDailyCheckInService>().IsRegistered()
                };
                AutoCheckInSwitchEnabled = true;
            }
        }

        internal async Task UpdateCheckInStatus()
        {
            DailyRewardHelper.SignInResult result = GameType switch
            {
                GameTypeEnum.Honkai3rd => await DailyRewardHelper.CheckInHonkaiTodayDailyReward(),
                GameTypeEnum.HonkaiStarRail => await DailyRewardHelper.CheckInHonkaiStarRailTodayDailyReward(),
                GameTypeEnum.ZenlessZoneZero => await DailyRewardHelper.CheckInZenlessZoneZeroTodayDailyReward(),

                _ => await DailyRewardHelper.CheckInTodayDailyReward()
            };

            _isCheckIn = result is DailyRewardHelper.SignInResult.AlreadySignIn;

            OnPropertyChanged(nameof(CheckInButtonBorderColor));
        }

        internal async Task CheckInTodayReward()
        {
            CheckInButtonEnabled = false;

            DailyRewardHelper.SignInResult result = GameType switch
            {
                GameTypeEnum.Honkai3rd => await DailyRewardHelper.CheckInHonkaiTodayDailyReward(),
                GameTypeEnum.HonkaiStarRail => await DailyRewardHelper.CheckInHonkaiStarRailTodayDailyReward(),
                GameTypeEnum.ZenlessZoneZero => await DailyRewardHelper.CheckInZenlessZoneZeroTodayDailyReward(),

                _ => await DailyRewardHelper.CheckInTodayDailyReward()
            };

            string message = result switch
            {
                DailyRewardHelper.SignInResult.Success => AppResources.DailyReward_CheckIn_Success,
                DailyRewardHelper.SignInResult.AlreadySignIn => AppResources.DailyReward_CheckIn_AlreadySignIn,

                _ => AppResources.DailyReward_CheckIn_Fail
            };

            DependencyService.Get<IToast>().Show(message);

            CheckInButtonEnabled = true;
        }

        internal void RegisterAutoCheckIn()
        {
            if (GameType is GameTypeEnum.Honkai3rd)
            {
                DependencyService.Get<IDailyCheckInService>().RegisterHonkai();
            }
            else if (GameType is GameTypeEnum.HonkaiStarRail)
            {
                DependencyService.Get<IDailyCheckInService>().RegisterHonkaiStarRail();
            }
            else if (GameType is GameTypeEnum.ZenlessZoneZero)
            {
                DependencyService.Get<IDailyCheckInService>().RegisterZenlessZoneZero();
            }
            else
            {
                DependencyService.Get<IDailyCheckInService>().Register();
            }
        }

        internal void UnregisterAutoCheckIn()
        {
            if (GameType is GameTypeEnum.Honkai3rd)
            {
                DependencyService.Get<IDailyCheckInService>().UnregisterHonkai();
            }
            else if (GameType is GameTypeEnum.HonkaiStarRail)
            {
                DependencyService.Get<IDailyCheckInService>().UnregisterHonkaiStarRail();
            }
            else if (GameType is GameTypeEnum.ZenlessZoneZero)
            {
                DependencyService.Get<IDailyCheckInService>().UnregisterZenlessZoneZero();
            }
            else
            {
                DependencyService.Get<IDailyCheckInService>().Unregister();
            }
        }
    }
}
