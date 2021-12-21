using GenshinInfo.Services;

using ResinTimer.Models;
using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppEnv = ResinTimer.AppEnvironment;

namespace ResinTimer.Pages.UtilPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckmiHoYoAPIStatusPage : ContentPage
    {
        enum APIType
        {
            RealTimeNote,
            RealTimeNoteSetting,
            WishLog
        }

        public List<APICheckResult> ResultList { get; private set; }

        private string Uid => Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_UID, string.Empty);
        private string Ltuid => Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_LTUID, string.Empty);
        private string Ltoken => Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_LTOKEN, string.Empty);
        private string AuthKey => Preferences.Get(SettingConstants.AUTHKEY_COMMON, string.Empty);

        public CheckmiHoYoAPIStatusPage()
        {
            InitializeComponent();

            ResultList = new List<APICheckResult>();
        }

        private async Task CheckAPIProcess()
        {
            ResultList.Clear();
            
            ListCollectionView.IsVisible = false;
            BusyLayout.IsVisible = true;
            BusyIndicator.IsRunning = true;

            UpdateCheckingStatusText(APIType.RealTimeNote);
            await CheckAPI(APIType.RealTimeNote);

            UpdateCheckingStatusText(APIType.RealTimeNoteSetting);
            await CheckAPI(APIType.RealTimeNoteSetting);

            UpdateCheckingStatusText(APIType.WishLog);
            await CheckAPI(APIType.WishLog);

            AppEnv.RefreshCollectionView(ListCollectionView, ResultList);

            ListCollectionView.IsVisible = true;
            BusyLayout.IsVisible = false;
            BusyIndicator.IsRunning = false;
        }

        private async Task CheckAPI(APIType type)
        {
            APICheckResult result = new();

            try
            {
                result.APIName = GetAPIName(type);

                (bool, string) testResult = type switch
                {
                    APIType.RealTimeNote => await TestAPIService.TestRealTimeNoteAPI(Uid, Ltuid, Ltoken),
                    APIType.RealTimeNoteSetting => await TestAPIService.TestRealTimeNoteSettingAPI(Ltuid, Ltoken),
                    APIType.WishLog => await TestAPIService.TestWishLogAPI(AuthKey),
                    _ => (false, string.Empty)
                };

                result.IsPass = testResult.Item1;
                result.ResultDetail = testResult.Item2;
            }
            catch (Exception ex)
            {
                StringBuilder sb = new();

                sb.AppendLine("API test function terminate :(");
                sb.AppendLine("");
                sb.Append(ex.ToString());

                result.ResultDetail = sb.ToString();
                result.IsPass = false;
            }
            finally
            {
                await Task.Delay(1000);
            }

            ResultList.Add(result);
        }

        private void UpdateCheckingStatusText(APIType type)
        {
            BusyStatusLabel.Text = $"{AppResources.APICheck_Status_Checking}({GetAPIName(type)})";
        }

        private string GetAPIName(APIType type)
        {
            return type switch
            {
                APIType.RealTimeNote => AppResources.APICheck_APIName_RealTimeNote,
                APIType.RealTimeNoteSetting => AppResources.APICheck_APIName_RealTimeNoteSetting,
                APIType.WishLog => AppResources.APICheck_APIName_WishLog,
                _ => string.Empty
            };
        }

        private async void RenewToolbarItemClicked(object sender, EventArgs e)
        {
            ToolbarItem item = sender as ToolbarItem;

            item.IsEnabled = false;

            await CheckAPIProcess();

            item.IsEnabled = true;
        }
    }
}