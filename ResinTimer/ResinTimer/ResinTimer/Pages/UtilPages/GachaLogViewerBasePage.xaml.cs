using GenshinInfo.Managers;
using GenshinInfo.Models;

using ResinTimer.Managers;
using ResinTimer.Models;
using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using static GenshinInfo.Managers.GachaInfoManager;

using AppEnv = ResinTimer.AppEnvironment;

namespace ResinTimer.Pages.UtilPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GachaLogViewerBasePage : ContentPage
    {
        public List<GachaLogGroup> LogList { get; private set; }

        internal GachaTypeNum gachaListType = GachaTypeNum.Permanent;
        internal string logSaveFilePath = string.Empty;

        private CancellationTokenSource cancelTokenSource;

        public GachaLogViewerBasePage()
        {
            InitializeComponent();

            LogList = new();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            try
            {
                cancelTokenSource?.Cancel();
                cancelTokenSource?.Dispose();
            }
            catch { }
        }

        private async void ToolbarItemClicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0: // Input authkey
                    await Navigation.PushAsync(new AuthKeyInputPage());
                    break;
                case 1: // Renew list
                    cancelTokenSource = new();

                    await RenewLogs(cancelTokenSource.Token);
                    break;
            }
        }

        internal async Task LoadInitLogs()
        {
            try
            {
                LogList.Clear();

                List<GachaLogGroup> saveLogList = await FileManager.LoadObject<List<GachaLogGroup>>(logSaveFilePath);

                if (saveLogList is not null)
                {
                    LogList.AddRange(saveLogList);

                    AppEnv.RefreshCollectionView(ListCollectionView, LogList);
                }

                string dtStr = Preferences.Get(gachaListType switch
                {
                    GachaTypeNum.WeaponEvent => SettingConstants.GACHALOGVIEWER_WEAPON_LASTUPDATE_INFO,
                    GachaTypeNum.CharacterEvent => SettingConstants.GACHALOGVIEWER_CHARACTER_LASTUPDATE_INFO,
                    _ => SettingConstants.GACHALOGVIEWER_PERMANENT_LASTUPDATE_INFO
                }, string.Empty);

                DisplayLastUpdate(string.IsNullOrWhiteSpace(dtStr) ? null : Utils.GetDateTimeFromString(dtStr));
            }
            catch (Exception) { }
            finally
            {
                CheckLogList();
            }
        }

        private void DisplayLastUpdate(DateTime? dt)
        {
            StringBuilder sb = new();

            sb.Append(AppResources.LastUpdate_Label);

            if (dt is not null)
            {
                sb.Append(Utils.GetTimeString(dt.Value));
            }

            LastUpdateLabel.Text = sb.ToString();
        }

        private void CheckLogList()
        {
            if (LogList.Count is 0)
            {
                ListCollectionView.IsVisible = false;
                BusyLayout.IsVisible = true;
                BusyIndicator.IsRunning = false;
                BusyStatusLabel.Text = AppResources.GachaLogViewer_LogList_NoItem;
            }
            else
            {
                ListCollectionView.IsVisible = true;
                BusyLayout.IsVisible = false;
                BusyIndicator.IsRunning = false;
            }
        }

        private async Task RenewLogs(CancellationToken cancelToken)
        {
            string loadBusyMsg = AppResources.GachaLogViewer_RenewLogList_Load;
            string processingBusyMsg = AppResources.GachaLogViewer_RenewLogList_Processing;

            try
            {
                ListCollectionView.IsVisible = false;
                BusyLayout.IsVisible = true;
                BusyIndicator.IsRunning = true;

                List<GachaInfo> infoList = new();
                GachaInfoManager manager = new();
                string lastId = "0";

                LogList.Clear();

                manager.SetAuthKey(Preferences.Get(SettingConstants.AUTHKEY_COMMON, string.Empty));

                BusyStatusLabel.Text = loadBusyMsg;

                while (true)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    string shortCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                    List<GachaInfo> result = await manager.GetGachaInfos(gachaListType, lastId, AppEnv.GetLangShortCode);

                    if (result is null)
                    {
                        throw new NullReferenceException();
                    }

                    if (result.Count is 0)
                    {
                        break;
                    }

                    infoList.AddRange(result);

                    BusyStatusLabel.Text = $"{loadBusyMsg} ({infoList.Count})";

                    lastId = result.Last().Id;

                    await Task.Delay(1000);
                }

                await Task.Delay(500);

                foreach (GachaInfo info in infoList)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    GachaLogGroup group = LogList.FirstOrDefault(x => x.GachaShortDateTime.Equals(info.GachaShortDateTime));

                    if (group is not null)
                    {
                        group.GachaInfos.Add(info);
                    }
                    else
                    {
                        GachaLogGroup newGroup = new(info.GachaShortDateTime);

                        newGroup.GachaInfos.Add(info);

                        LogList.Add(newGroup);
                    }

                    BusyStatusLabel.Text = processingBusyMsg;
                }

                await Task.Delay(1000);

                AppEnv.RefreshCollectionView(ListCollectionView, LogList);

                await FileManager.SaveObject(logSaveFilePath, LogList);

                CheckLogList();
            }
            catch (Exception ex)
            {
                BusyIndicator.IsRunning = false;
                BusyStatusLabel.Text = AppResources.GachaLogViewer_RenewLogList_Fail;
            }
            finally
            {
                DateTime now = DateTime.Now;

                Preferences.Set(gachaListType switch
                {
                    GachaTypeNum.WeaponEvent => SettingConstants.GACHALOGVIEWER_WEAPON_LASTUPDATE_INFO,
                    GachaTypeNum.CharacterEvent => SettingConstants.GACHALOGVIEWER_CHARACTER_LASTUPDATE_INFO,
                    _ => SettingConstants.GACHALOGVIEWER_PERMANENT_LASTUPDATE_INFO
                }, now.ToString("yyyy,MM,dd,HH,mm,ss"));

                DisplayLastUpdate(now);
            }
        }

        private void ListCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}