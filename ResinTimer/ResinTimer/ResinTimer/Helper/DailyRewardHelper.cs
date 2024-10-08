﻿using GenshinInfo.Managers;
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
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();

            if ((await manager.GetDailyRewardStatus()).IsSign)
            {
                return SignInResult.AlreadySignIn;
            }

            return (await manager.SignInDailyReward()) ? SignInResult.Success : SignInResult.Fail;
        }

        public static async Task<bool> CheckSignInStatus()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();

            return (await manager.GetDailyRewardStatus()).IsSign;
        }

        public static async Task<DailyRewardListItemData> GetNowDailyRewardItem()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();
            DailyRewardListData listData = await manager.GetDailyRewardList(Thread.CurrentThread.CurrentUICulture.Name);
            DailyRewardStatusData statusData = await manager.GetDailyRewardStatus();
            int index = statusData.TotalSignDayCount + (statusData.IsSign ? -1 : 0);

            return listData.Rewards[index];
        }


        // Honkai 3rd

        public static async Task<SignInResult> CheckInHonkaiTodayDailyReward()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();

            if ((await manager.GetHonkaiDailyRewardStatus()).IsSign)
            {
                return SignInResult.AlreadySignIn;
            }

            return (await manager.SignInHonkaiDailyReward()) ? SignInResult.Success : SignInResult.Fail;
        }

        public static async Task<bool> CheckHonkaiSignInStatus()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();

            return (await manager.GetHonkaiDailyRewardStatus()).IsSign;
        }

        public static async Task<DailyRewardListItemData> GetHonkaiNowDailyRewardItem()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();
            DailyRewardListData listData = await manager.GetHonkaiDailyRewardList(Thread.CurrentThread.CurrentUICulture.Name);
            DailyRewardStatusData statusData = await manager.GetHonkaiDailyRewardStatus();
            int index = statusData.TotalSignDayCount + (statusData.IsSign ? -1 : 0);

            return listData.Rewards[index];
        }


        // Honkai Star Rail

        public static async Task<SignInResult> CheckInHonkaiStarRailTodayDailyReward()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();

            if ((await manager.GetHonkaiStarRailDailyRewardStatus()).IsSign)
            {
                return SignInResult.AlreadySignIn;
            }

            return (await manager.SignInHonkaiStarRailDailyReward()) ? SignInResult.Success : SignInResult.Fail;
        }

        public static async Task<bool> CheckHonkaiStarRailSignInStatus()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();

            return (await manager.GetHonkaiStarRailDailyRewardStatus()).IsSign;
        }

        public static async Task<DailyRewardListItemData> GetHonkaiStarRailNowDailyRewardItem()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();
            DailyRewardListData listData = await manager.GetHonkaiStarRailDailyRewardList(Thread.CurrentThread.CurrentUICulture.Name);
            DailyRewardStatusData statusData = await manager.GetHonkaiStarRailDailyRewardStatus();
            int index = statusData.TotalSignDayCount + (statusData.IsSign ? -1 : 0);

            return listData.Rewards[index];
        }


        // Zenless Zone Zero

        public static async Task<SignInResult> CheckInZenlessZoneZeroTodayDailyReward()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();

            if ((await manager.GetZenlessZoneZeroDailyRewardStatus()).IsSign)
            {
                return SignInResult.AlreadySignIn;
            }

            return (await manager.SignInZenlessZoneZeroDailyReward()) ? SignInResult.Success : SignInResult.Fail;
        }

        public static async Task<bool> CheckZenlessZoneZeroSignInStatus()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();

            return (await manager.GetZenlessZoneZeroDailyRewardStatus()).IsSign;
        }

        public static async Task<DailyRewardListItemData> GetZenlessZoneZeroNowDailyRewardItem()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();
            DailyRewardListData listData = await manager.GetZenlessZoneZeroDailyRewardList(Thread.CurrentThread.CurrentUICulture.Name);
            DailyRewardStatusData statusData = await manager.GetZenlessZoneZeroDailyRewardStatus();
            int index = statusData.TotalSignDayCount + (statusData.IsSign ? -1 : 0);

            return listData.Rewards[index];
        }
    }
}