using System;
using System.Linq;
using System.Threading.Tasks;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;

using ResinTimer.TimerPages;

using Rg.Plugins.Popup.Services;

using Xamarin.Forms;

using ResinTimer.Droid.Permissions;

using static ResinTimer.Droid.AndroidAppEnvironment;

namespace ResinTimer.Droid
{
    [Activity(Theme = "@style/ResinTimer", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
              Exported = true)]
    [IntentFilter(new[] { Xamarin.Essentials.Platform.Intent.ActionAppAction }, Categories = new[] { Android.Content.Intent.CategoryDefault })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private App _app;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                base.OnCreate(savedInstanceState);

                Rg.Plugins.Popup.Popup.Init(this);

                Xamarin.Forms.Forms.SetFlags(new string[] { "Shapes_Experimental", "SwipeView_Experimental" });

                //Xamarin.Forms.DependencyService.Register<ResinTimer.NotiScheduleService, ResinTimer.Droid.NotiScheduleAndroid>();

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

                App app = new();
                    
                app.SetMainPage(Intent.GetStringExtra(KEY_TALENTITEM_CLICK) switch
                {
                    VALUE_TALENTITEM_CLICK => new NavigationPage(
                        new TalentCharacterPage(Intent.GetStringArrayListExtra(KEY_TALENTITEM_LIST)?.ToArray())),
                    _ => null
                });

                _app = app;

                await CheckPlatformPermissions();
                LoadApplication(app);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                Log.Error("ResinTimer", ex.ToString());
            }
        }

        private async Task CheckPlatformPermissions()
        {
            Xamarin.Essentials.PermissionStatus status = 
                await Xamarin.Essentials.Permissions.CheckStatusAsync<NotificationPermission>();

            if (status is not Xamarin.Essentials.PermissionStatus.Granted)
            {
                await Xamarin.Essentials.Permissions.RequestAsync<NotificationPermission>();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (Xamarin.Essentials.Preferences.Get(SettingConstants.APP_RETURNSTARTPAGE_ENABLED, true))
            {
                if (PopupNavigation.Instance.PopupStack.Count > 0)
                {
                    Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
                }
                else if ((_app is not null) &&
                         (_app.MainPage is MainPage page) &&
                         ((page.Detail as NavigationPage).CurrentPage.Navigation.NavigationStack.Count <= 1) &&
                         ((page.Detail as NavigationPage).CurrentPage is not TimerHomePage))
                {
                    page.ApplyDetailPage(new(new TimerHomePage()));
                }
                else
                {
                    base.OnBackPressed();
                }
            }
            else
            {
                Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            Xamarin.Essentials.Platform.OnResume(this);
        }

        protected override void OnNewIntent(Android.Content.Intent intent)
        {
            base.OnNewIntent(intent);

            Xamarin.Essentials.Platform.OnNewIntent(intent);
        }
    }
}