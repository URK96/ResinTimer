using GenshinInfo.Managers;
using GenshinInfo.Models;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ResinTimer.Helper
{
    public static class DailyRewardHelper
    {
        public enum SignInResult
        {
            AlreadySignIn = 0,
            Fail = 1,
            Success = 2
        }

        public static async Task<SignInResult> CheckInTodayDailyReward()
        {
            GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);

            if ((await manager.GetDailyRewardStatus()).IsSign)
            {
                return SignInResult.AlreadySignIn;
            }

            return (await manager.SignInDailyReward()) ? SignInResult.Success : SignInResult.Fail;
        }

        public static async Task<bool> CheckSignInStatus()
        {
            GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);

            return (await manager.GetDailyRewardStatus()).IsSign;
        }

        public static async Task<DailyRewardListItemData> GetNowDailyRewardItem()
        {
            GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);
            DailyRewardListData listData = await manager.GetDailyRewardList(Thread.CurrentThread.CurrentUICulture.Name);
            DailyRewardStatusData statusData = await manager.GetDailyRewardStatus();
            int index = statusData.TotalSignDayCount + (statusData.IsSign ? -1 : 0);

            return listData.Rewards[index];
        }


        // Honkai 3rd

        public static async Task<SignInResult> CheckInHonkaiTodayDailyReward()
        {
            GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);

            if ((await manager.GetHonkaiDailyRewardStatus()).IsSign)
            {
                return SignInResult.AlreadySignIn;
            }

            return (await manager.SignInHonkaiDailyReward()) ? SignInResult.Success : SignInResult.Fail;
        }

        public static async Task<bool> CheckHonkaiSignInStatus()
        {
            GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);

            return (await manager.GetHonkaiDailyRewardStatus()).IsSign;
        }

        public static async Task<DailyRewardListItemData> GetHonkaiNowDailyRewardItem()
        {
            GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);
            DailyRewardListData listData = await manager.GetHonkaiDailyRewardList(Thread.CurrentThread.CurrentUICulture.Name);
            DailyRewardStatusData statusData = await manager.GetHonkaiDailyRewardStatus();
            int index = statusData.TotalSignDayCount + (statusData.IsSign ? -1 : 0);

            return listData.Rewards[index];
        }
    }
}
