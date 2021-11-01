﻿using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System;

using Xamarin.Forms;

namespace ResinTimer.Managers.NotiManagers
{
    public class ResinNotiManager : NotiManager
    {
        public ResinNotiManager() : base()
        {
            try
            {
                notiType = NotiType.Resin;

                Notis.AddRange(GetNotiList<ResinNoti>());

                if (Notis.Count < 1)
                {
                    Notis.Add(new ResinNoti(ResinEnvironment.MAX_RESIN));
                    SaveNotis();
                }
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize noti manager");
            }
        }

        public override void RenewalIds()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                Notis[i].NotiId = (Notis[i] as ResinNoti).Resin;
            }
        }

        public override void UpdateNotisTime()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                (Notis[i] as ResinNoti).UpdateTime();
            }

            SaveNotis();
            UpdateScheduledNoti<ResinNoti>();
        }

        public override void EditList(Noti item, EditType type)
        {
            var noti = item as ResinNoti;

            switch (type)
            {
                case EditType.Add:
                    if (Notis.FindAll(x => (x as ResinNoti).Resin.Equals(noti.Resin)).Count == 0)
                    {
                        Notis.Add(item);
                        Notis.Sort(SortNotis);
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_AlreadyExistToast_Message);
                    }
                    break;
                case EditType.Remove:
                    Notis.Remove(Notis.Find(x => (x as ResinNoti).Resin.Equals(noti.Resin)));
                    break;
                case EditType.Edit:
                    (Notis[Notis.FindIndex(x => (x as ResinNoti).Resin.Equals(noti.Resin))] as ResinNoti).Resin = noti.Resin;
                    break;
                default:
                    break;
            }

            SaveNotis();
            UpdateScheduledNoti<ResinNoti>();
        }
    }

}