using GenshinInfo.Managers;
using GenshinInfo.Models;

using ResinTimer.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private const string AuthKeyUrl = "https://webstatic-sea.mihoyo.com/ys/event/im-service/index.html?im_out=true&sign_type=2&auth_appid=im_ccs&authkey_ver=1&win_direction=portrait&lang=ko&device_type=pc&ext=%7b%22loc%22%3a%7b%22x%22%3a1638.73974609375%2c%22y%22%3a197.1015625%2c%22z%22%3a-2665.09423828125%7d%2c%22platform%22%3a%22WinST%22%7d&game_version=OSRELWin2.2.0_R4705718_S4858667_D4715326&plat_type=pc&authkey=cYrxY0DyDVJZZ7n8CFLjpRfLtDIhgiDWUUlf7IoFXTCve1sa1VVkcuT3zZP43Btkaev06OhasqKdfok9HCDgp6BJUVs2wpZqYs95MKZHLHVrSbwg88wzdrQCvDazLbHZjl7%2f%2bY1%2bSp7x3tdHL%2bKoudfbme0btp6keMH1lXbinWssXte%2fx8D6Bu1D12shQo%2baSlzq5jSlBOv0s7keKOqbaxZBiNz%2fd7MovyipqPz0yfFdC0A938Cd5KZln5EDTQrVlK8Hgzt4DuaDqd6vum6m0DmMm13%2fiHKoMT5%2bIU9smgOWgApkq2DvaX0x7kCgHHL%2b1RujwbJ6%2fShNnnh%2bmOpVIk3BcJY3aIjb6MdaJdXLmljiMDWftISqPDODU8lF8UGxbK6InewEq2r95jeUJ9Ei%2f9pzz7K04ycSV2Ol6RhlAXc37vbcesMicCkZg1%2bTb%2fRqlMGjxrwhhzFPVgD6bRsCJd0ZXRcc1YK8N9RbNsrsq%2bVNcMB8Vdf3fvwQTf8fGUXPQ03GAR0Z%2fAs28KfX9Jc3ds9KHv%2fWhLnKzP8GJooD2QxWBM7%2bc4UzwXWd2xix5lkedKVQ15kPWVdsHc8ErUHjgxOyyR8hLJWJTViC7mz5adJ%2fp03d29va%2bf9FglIBL8sg2LLBiONZgzgESQv8HPric8ArMBgLBLHMK%2fdvauaf5KuTp6L3qFJBvnleSpNMkPDjIx2UgP2e5wBmSfYEbl1QEU0TnsRAeLRxJwXP6BbOCa3a2ZOJ4O%2fM1uY4H4HHJx42pP%2f2fWENSiwJk5wM2oEmfQaaIgExFntNV1Y4w8aNwkzV%2f3NaMVtm06G2B4PEcDDtog48Ry%2f7XDyCW0xjehqzzk5ZX%2bzKJ8qiwgTgXtmlY0hkVkXzrC%2bdUKfas6pCz23eT6xzo6gx8Hfh7VDeWhY7yqUByidBrEz3L12B5BrMnFPaGUwn%2bwPC9RT1dba%2fNJDRMIK7O8N4OXbqcyBWaTHtPM0%2feldnoQiXEXL%2bSP9uYUFHwqOyhJNbfLXYLMnCyd3RC0awCme3Z4TZKfMXYW%2brNYHc7FWSe0gOBbmpn%2bjX7NDsrMN7irWuoPewlZXYDNqGsL1qOiQYY4wH%2fepNukI1rQT619LAtbI78A6R%2bhjm3TXkFlN2taScoT8%2biniXBaLeuj75S8gvycnLcKfX%2b8kPoG6xaXGJKG4dytw4DKgABT%2fIvU2Wqes4Ei5PY2EKX%2fyGJfY%2fS3Uu9XW8vHHz%2fZhe04eL9nbO%2bmziFoh4mClPGKsR%2fvngoQ%2bVHf1%2b25bugihDQcAy1vGEY4p4xxIM4rSU%2bUcFNP1D5kPGBsv1BJfDoXEFCbUMxogJsxm3wmYNjnAjx9wfceHy7Efa8gXrSAedVQ%3d%3d&game_biz=hk4e_global#/";

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
            ListCollectionView.IsVisible = false;
            BusyLayout.IsVisible = true;
            BusyIndicator.IsRunning = true;

            List<GachaInfo> infoList = new();
            GachaInfoManager manager = new();
            string lastId = "0";

            LogList.Clear();

            manager.SetAuthKeyByUrl(AuthKeyUrl);

            BusyStatusLabel.Text = "Load from server... (Total 0)";

            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                List<GachaInfo> result = await manager.GetGachaInfos(gachaListType, lastId);

                if (result.Count is 0)
                {
                    break;
                }

                infoList.AddRange(result);

                BusyStatusLabel.Text = $"Load from server... (Total {infoList.Count})";

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

            ListCollectionView.IsVisible = true;
            BusyLayout.IsVisible = false;
            BusyIndicator.IsRunning = false;

            AppEnv.RefreshCollectionView(ListCollectionView, LogList);
        }

        private void ListCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}