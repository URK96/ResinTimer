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
        //private const string AuthKeyUrl = "https://webstatic-sea.mihoyo.com/ys/event/im-service/index.html?im_out=true&sign_type=2&auth_appid=im_ccs&authkey_ver=1&win_direction=portrait&lang=ko&device_type=pc&ext=%7b%22loc%22%3a%7b%22x%22%3a-2570.352783203125%2c%22y%22%3a274.71826171875%2c%22z%22%3a-4580.044921875%7d%2c%22platform%22%3a%22WinST%22%7d&game_version=OSRELWin2.2.0_R4705718_S4858667_D4715326&plat_type=pc&authkey=cYrxY0DyDVJZZ7n8CFLjpRfLtDIhgiDWUUlf7IoFXTCve1sa1VVkcuT3zZP43Btkaev06OhasqKdfok9HCDgp6BJUVs2wpZqYs95MKZHLHVrSbwg88wzdrQCvDazLbHZjl7%2f%2bY1%2bSp7x3tdHL%2bKoudfbme0btp6keMH1lXbinWssXte%2fx8D6Bu1D12shQo%2baSlzq5jSlBOv0s7keKOqbaxZBiNz%2fd7MovyipqPz0yfFdC0A938Cd5KZln5EDTQrVlK8Hgzt4DuaDqd6vum6m0DmMm13%2fiHKoMT5%2bIU9smgOWgApkq2DvaX0x7kCgHHL%2b1RujwbJ6%2fShNnnh%2bmOpVIk3BcJY3aIjb6MdaJdXLmljiMDWftISqPDODU8lF8UGxbK6InewEq2r95jeUJ9Ei%2f9pzz7K04ycSV2Ol6RhlAXc37vbcesMicCkZg1%2bTb%2fRqlMGjxrwhhzFPVgD6bRsCJd0ZXRcc1YK8N9RbNsrsq%2bVNcMB8Vdf3fvwQTf8fGUXPQ03GAR0Z%2fAs28KfX9Jc3ds9KHv%2fWhLnKzP8GJooD2QxWBM7%2bc4UzwXWd2xix5lkedKVQ15kPWVdsHc8ErUHjgxOyyR8hLJWJTViC7mz5adJ%2fp03d29va%2bf9FglIBL8sg2LLBiONZgzgESQv8HPric8ArMBgLBLHMK%2fdvauaf5KvnoxPROMBouDAXQIZu7upzTJax9u7XBJxUN1Db4tyqhHSJzf2EWEZTNDe7WOnnMzZKPbaUua%2bEFZzFvF7ybIglt%2f9c%2brMo7HQG1oZYpDhrAMdI3YAwckAkH%2bnR10qrnkPfmdUT6kCKr6i0d7wuTKSYAM5w5mN54eXZEvM5wKzrwy0V56b9QB37CcDFrfibiXjGX3k2KLiiRZx5l9%2b8FEhM%2fJvJM5YLfgttuYlL1WKAWV5uVVo0SI8eghZwupAVO%2bHANKRBoDZRCgQ%2bcEc6Qh7%2bUUh9uJg5GaPmn%2b2J2NTc5E8p5%2bS8DAm4vW7Ov84BGZmLTN0N3StxUoAB6NEHNvrLC0awCme3Z4TZKfMXYW%2brNYHc7FWSe0gOBbmpn%2bjX7NDsrMN7irWuoPewlZXYDNqGsL1qOiQYY4wH%2fepNukI1rQT619LAtbI78A6R%2bhjm3TXkFlN2taScoT8%2biniXBaLeuj75S8gvycnLcKfX%2b8kPoG6xaXGJKG4dytw4DKgABT%2fIvU2Wqes4Ei5PY2EKX%2fyGJfY%2fS3Uu9XW8vHHz%2fZhe04eL9nbO%2bmziFoh4mClPGKsR%2fvngoQ%2bVHf1%2b25bugihDQcAy1vGEY4p4xxIM4rSU%2bUcFNP1D5kPGBsv1BJfDoXEFCbUMxogJsxm3wmYNjnAjx9wfceHy7Efa8gXrSAedVQ%3d%3d&game_biz=hk4e_global#/";

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

                    await Task.Delay(10);
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
            catch (Exception)
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