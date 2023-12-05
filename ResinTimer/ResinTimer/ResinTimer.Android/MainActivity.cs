using System;
using System.Linq;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;

using AndroidX.Activity;
using AndroidX.AppCompat.App;

using ResinTimer.TimerPages;

using Rg.Plugins.Popup.Services;

using Xamarin.Forms;

using static ResinTimer.Droid.AndroidAppEnvironment;

namespace ResinTimer.Droid
{
    [Activity(Theme = "@style/ResinTimer", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
              Exported = true)]
    [IntentFilter(new[] { Xamarin.Essentials.Platform.Intent.ActionAppAction }, Categories = new[] { Android.Content.Intent.CategoryDefault })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private static App s_app;

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

                s_app = app;

                OnBackPressedDispatcher.AddCallback(this, new MainBackPressedCallback(this));
                LoadApplication(app);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                Log.Error("ResinTimer", ex.ToString());
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

        class MainBackPressedCallback : OnBackPressedCallback
        {
            private readonly AppCompatActivity _activity;

            public MainBackPressedCallback(AppCompatActivity activity = null) : base(true) 
            {
                _activity = activity;
            }

            public override void HandleOnBackPressed()
            {
                bool enableReturnHomePage =
                    Xamarin.Essentials.Preferences.Get(SettingConstants.APP_RETURNSTARTPAGE_ENABLED, true);

                if (PopupNavigation.Instance.PopupStack.Any())
                {
                    Rg.Plugins.Popup.Popup.SendBackPressed(_activity.OnBackPressedDispatcher.OnBackPressed);
                }
                else if ((s_app is not null) &&
                         (s_app.MainPage is MainPage page))
                {
                    Page currentPage = (page.Detail as NavigationPage).CurrentPage;

                    if (currentPage.Navigation.NavigationStack.Count > 1)
                    {
                        currentPage.Navigation.PopAsync();
                    }
                    else if (enableReturnHomePage &&
                             (currentPage is not TimerHomePage))
                    {
                        page.ApplyDetailPage(new NavigationPage(new TimerHomePage()));
                    }
                    else
                    {
                        _activity.FinishAfterTransition();
                        Process.KillProcess(Process.MyPid());
                    }
                }
                else
                {
                    _activity.FinishAfterTransition();
                    Process.KillProcess(Process.MyPid());
                }
            }
        }
    }
}