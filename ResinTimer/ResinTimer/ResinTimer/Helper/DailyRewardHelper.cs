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

        public static async Task<DailyRewardListItemData> GetNowDailyRewardItem()
        {
            DailyRewardListData listData = await GetDailyRewardList();

            return listData.Rewards[DateTime.UtcNow.Day - 1];
        }

        private static async Task<DailyRewardListData> GetDailyRewardList()
        {
            GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);

            return await manager.GetDailyRewardList(Thread.CurrentThread.CurrentUICulture.Name);
        }
    }
}
