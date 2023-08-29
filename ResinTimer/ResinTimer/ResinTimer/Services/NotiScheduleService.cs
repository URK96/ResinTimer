using ResinTimer.Dialogs;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;
using Rg.Plugins.Popup.Services;

using System;
using System.Threading.Tasks;

using Xamarin.Essentials;

using Xamarin.Forms;

namespace ResinTimer.Services
{
    public class NotiScheduleService : INotiScheduleService
    {
        public virtual void Cancel<T>() where T : Noti { }

        public virtual void Schedule<T>() where T : Noti { }

        public virtual void ScheduleNotiItem<T>(Noti noti) where T : Noti { }

        public virtual void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime) { }

        public virtual void TestNoti(string message = "") { }

        public virtual bool CheckPlatformNotiEnabled() => true;

        public virtual async Task<bool> RequestNotiPermission() => await Task.FromResult(true);

        public void ScheduleAll()
        {
            Schedule<ResinNoti>();
            Schedule<RealmCurrencyNoti>();
            Schedule<RealmFriendshipNoti>();
            Schedule<ExpeditionNoti>();
            Schedule<GatheringItemNoti>();
            Schedule<GadgetNoti>();
            Schedule<FurnishingNoti>();
            Schedule<GardeningNoti>();
        }

        public virtual void CancelAll()
        {
            Cancel<ResinNoti>();
            Cancel<RealmCurrencyNoti>();
            Cancel<RealmFriendshipNoti>();
            Cancel<ExpeditionNoti>();
            Cancel<GatheringItemNoti>();
            Cancel<GadgetNoti>();
            Cancel<FurnishingNoti>();
            Cancel<GardeningNoti>();
        }

        public static async Task VerifyNotificationAvailable()
        {
            if (!Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                return;
            }

            var notiService = DependencyService.Get<INotiScheduleService>();

            if (!notiService.CheckPlatformNotiEnabled())
            {
                ActionView view = new(AppResources.VerifyNotiPermission_Dialog_Message);
                BaseDialog dialog = new(AppResources.VerifyNotiPermission_Dialog_Title, view);

                dialog.OnClose += async delegate
                {
                    if (view.Result)
                    {
                        if (!await notiService.RequestNotiPermission())
                        {
                            BaseDialog noticeDialog = new(
                                string.Empty,
                                new MessageView(AppResources.VerifyNotiPermission_AfterDenied_Message));

                            await PopupNavigation.Instance.PushAsync(noticeDialog);
                        }
                    }
                };

                await PopupNavigation.Instance.PushAsync(dialog);
            }
        }
    }
}
