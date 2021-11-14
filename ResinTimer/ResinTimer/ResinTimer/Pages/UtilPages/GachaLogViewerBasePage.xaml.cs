using GenshinInfo.Managers;
using GenshinInfo.Models;

using ResinTimer.Models;
using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using static GenshinInfo.Managers.GachaInfoManager;

using AppEnv = ResinTimer.AppEnvironment;

namespace ResinTimer.Pages.UtilPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GachaLogViewerBasePage : ContentPage
    {
        private const string AuthKeyUrl = "https://webstatic-sea.mihoyo.com/ys/event/im-service/index.html?im_out=true&sign_type=2&auth_appid=im_ccs&authkey_ver=1&win_direction=portrait&lang=ko&device_type=pc&ext=%7b%22loc%22%3a%7b%22x%22%3a-3093.122802734375%2c%22y%22%3a252.1823272705078%2c%22z%22%3a-4432.9033203125%7d%2c%22platform%22%3a%22WinST%22%7d&game_version=OSRELWin2.2.0_R4705718_S4858667_D4715326&plat_type=pc&authkey=cYrxY0DyDVJZZ7n8CFLjpRfLtDIhgiDWUUlf7IoFXTCve1sa1VVkcuT3zZP43Btkaev06OhasqKdfok9HCDgp6BJUVs2wpZqYs95MKZHLHVrSbwg88wzdrQCvDazLbHZjl7%2f%2bY1%2bSp7x3tdHL%2bKoudfbme0btp6keMH1lXbinWssXte%2fx8D6Bu1D12shQo%2baSlzq5jSlBOv0s7keKOqbaxZBiNz%2fd7MovyipqPz0yfFdC0A938Cd5KZln5EDTQrVlK8Hgzt4DuaDqd6vum6m0DmMm13%2fiHKoMT5%2bIU9smgOWgApkq2DvaX0x7kCgHHL%2b1RujwbJ6%2fShNnnh%2bmOpVIk3BcJY3aIjb6MdaJdXLmljiMDWftISqPDODU8lF8UGxbK6InewEq2r95jeUJ9Ei%2f9pzz7K04ycSV2Ol6RhlAXc37vbcesMicCkZg1%2bTb%2fRqlMGjxrwhhzFPVgD6bRsCJd0ZXRcc1YK8N9RbNsrsq%2bVNcMB8Vdf3fvwQTf8fGUXPQ03GAR0Z%2fAs28KfX9Jc3ds9KHv%2fWhLnKzP8GJooD2QxWBM7%2bc4UzwXWd2xix5lkedKVQ15kPWVdsHc8ErUHjgxOyyR8hLJWJTViC7mz5adJ%2fp03d29va%2bf9FglIBL8sg2LLBiONZgzgESQv8HPric8ArMBgLBLHMK%2fdvauaf5KswIHiVuq8q0%2fzjQ7ft6xxi5p2lCsZe9or2ycN%2biN6Wv9S%2ffADwRfTvO6pPoxIrifggVeIE5LCcqTcxriKPcmiU6bw0%2fR4jeD2gn1R0nUaMYU0JO9wkadmMhJS2aLMZOAs7mNH93IfRU1xxbi6eBO3uc47Apm%2bY%2fK4iC5PqCqpEPMSlkckAYWyP2gCxX8UwQv3blVmH6ByR2kDO2xM8Hkjin%2fI5dz9OylcdCLrCly2EqmKoR3XRwxf0ZXXtYm9FdXqczL7f%2farBQmGHHHHjDA%2fBh4Ib2VmNPS0S3EKAPrhEzner%2b7C1JCBGKesbuQE%2bwHr1bDRKL3CtsAbgShFkC1KnC0awCme3Z4TZKfMXYW%2brNYHc7FWSe0gOBbmpn%2bjX7NDsrMN7irWuoPewlZXYDNqGsL1qOiQYY4wH%2fepNukI1rQT619LAtbI78A6R%2bhjm3TXkFlN2taScoT8%2biniXBaLeuj75S8gvycnLcKfX%2b8kPoG6xaXGJKG4dytw4DKgABT%2fIvU2Wqes4Ei5PY2EKX%2fyGJfY%2fS3Uu9XW8vHHz%2fZhe04eL9nbO%2bmziFoh4mClPGKsR%2fvngoQ%2bVHf1%2b25bugihDQcAy1vGEY4p4xxIM4rSU%2bUcFNP1D5kPGBsv1BJfDoXEFCbUMxogJsxm3wmYNjnAjx9wfceHy7Efa8gXrSAedVQ%3d%3d&game_biz=hk4e_global#/";

        public List<GachaLogGroup> LogList { get; private set; }

        internal GachaTypeNum gachaListType = GachaTypeNum.Permanent;

        private CancellationTokenSource cancelTokenSource;

        public GachaLogViewerBasePage()
        {
            InitializeComponent();

            LogList = new();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            cancelTokenSource?.Cancel();
        }

        private async void ToolbarItemClicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0: // Edit authkey

                    break;
                case 1: // Renew list
                    cancelTokenSource = new();

                    await RenewLogs(cancelTokenSource.Token);
                    break;
            }
        }

        private async Task RenewLogs(CancellationToken cancelToken)
        {
            string loadBusyMsg = AppResources.GachaLogViewer_RenewLogList_Load;

            try
            {
                ListCollectionView.IsVisible = false;
                BusyLayout.IsVisible = true;
                BusyIndicator.IsRunning = true;

                List<GachaInfo> infoList = new();
                GachaInfoManager manager = new();
                string lastId = "0";

                LogList.Clear();

                manager.SetAuthKeyByUrl(AuthKeyUrl);

                BusyStatusLabel.Text = $"{loadBusyMsg}";

                while (true)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    List<GachaInfo> result = await manager.GetGachaInfos(gachaListType, lastId);

                    if (result is null)
                    {
                        BusyIndicator.IsRunning = false;
                        BusyStatusLabel.Text = AppResources.GachaLogViewer_RenewLogList_Fail;
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

                    BusyStatusLabel.Text = $"Processing logs...";
                }

                await Task.Delay(1000);

                AppEnv.RefreshCollectionView(ListCollectionView, LogList);
            }
            catch (Exception)
            {

            }
            finally
            {
                ListCollectionView.IsVisible = true;
                BusyLayout.IsVisible = false;
                BusyIndicator.IsRunning = false;
            }
        }

        private void ListCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}