using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using Newtonsoft.Json;
using AndroidX.Core.App;

using static ResinTimer.Droid.AndroidAppEnvironment;
using System.Collections.Generic;
using System.Linq;
using Android.Util;
using ResinTimer.TimerPages;
using Rg.Plugins.Popup.Services;
using Java.Util.Prefs;

namespace ResinTimer.Droid
{
    [Activity(Theme = "@style/ResinTimer", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
              Exported = true)]
    [IntentFilter(new[] { Xamarin.Essentials.Platform.Intent.ActionAppAction }, Categories = new[] { Android.Content.Intent.CategoryDefault })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private App _app;

        protected override void OnCreate(Bundle savedInstanceState)
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

                LoadApplication(app);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                Log.Error("ResinTimer", ex.ToString());
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