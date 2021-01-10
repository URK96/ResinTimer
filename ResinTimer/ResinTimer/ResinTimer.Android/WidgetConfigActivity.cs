using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;
using AndroidX.Core.Content.Resources;

using ResinTimer.Resources;

using System;

namespace ResinTimer.Droid
{
    [Activity(Label = "WidgetConfigActivity", Name = "com.urk.resintimer.WidgetConfigActivity", Theme = "@style/ResinTimer")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_CONFIGURE" })]
    public class WidgetConfigActivity : AppCompatActivity
    {
        private AppWidgetManager widgetManager;
        private int widgetId;
        private int widgetLayoutId;
        private RemoteViews remoteView;
        private View previewView;
        private Drawable deviceWallpaper;
        private GradientDrawable roundedDrawable;

        private SeekBar opacitySeekBar;
        private int opacity = 0;
        private string color = "FFFFFF";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.WidgetSetting);
            SetSupportActionBar(FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar));
            Title = AppResources.WidgetConfigure_Title;

            var mExtras = Intent.Extras;

            if (mExtras != null)
            {
                widgetId = mExtras.GetInt(AppWidgetManager.ExtraAppwidgetId, AppWidgetManager.InvalidAppwidgetId);
            }

            deviceWallpaper = WallpaperManager.GetInstance(this).Drawable;

            widgetManager = AppWidgetManager.GetInstance(this);
            widgetLayoutId = widgetManager.GetAppWidgetInfo(widgetId).InitialLayout;

            remoteView = new RemoteViews(PackageName, widgetLayoutId);
            previewView = LayoutInflater.Inflate(widgetLayoutId, null);
            roundedDrawable = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.rounded_background, null) as GradientDrawable;

            opacitySeekBar = FindViewById<AndroidX.AppCompat.Widget.AppCompatSeekBar>(Resource.Id.WidgetConfigureOpacitySeekBar);
            opacitySeekBar.ProgressChanged += (sender, e) =>
            {
                opacity = e.Progress;

                UpdatePreviewStyle();
            };

            FindViewById<RadioButton>(Resource.Id.WidgetConfigureBackgroundColorWhite).CheckedChange += WidgetConfigBackgroundSelector_CheckedChange;
            FindViewById<RadioButton>(Resource.Id.WidgetConfigureBackgroundColorBlack).CheckedChange += WidgetConfigBackgroundSelector_CheckedChange;

            FindViewById<FrameLayout>(Resource.Id.WidgetSettingPreviewLayout).AddView(previewView);

            SetPreviewValue();
            UpdatePreviewStyle();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.WidgetConfigureToolbarMenu, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item?.ItemId)
            {
                case Resource.Id.WidgetConfigureApply:
                    var resultValue = new Intent();

                    resultValue.PutExtra(AppWidgetManager.ExtraAppwidgetId, widgetId);
                    SetResult(Result.Ok, resultValue);
                    Finish();
                    break;
                case Resource.Id.WidgetConfigureClose:
                    Finish();
                    break;
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void ApplyWidgetSetting()
        {
            widgetManager.UpdateAppWidget()
        }

        private void WidgetConfigBackgroundSelector_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                color = (sender as RadioButton).Id switch
                {
                    Resource.Id.WidgetConfigureBackgroundColorBlack => "000000",
                    _ => "FFFFFF",
                };

                UpdatePreviewStyle();
            }
        }

        private void SetPreviewValue()
        {
            switch (widgetLayoutId)
            {
                case Resource.Layout.ResinWidget:
                    previewView.FindViewById<TextView>(Resource.Id.ResinWidgetCount).Text = ResinEnvironment.MAX_RESIN.ToString();
                    previewView.FindViewById<TextView>(Resource.Id.ResinWidgetEndTime).Text = DateTime.Now.ToString();
                    break;
                case Resource.Layout.ResinWidgetSimple:
                    previewView.FindViewById<TextView>(Resource.Id.ResinWidgetSimpleCount).Text = ResinEnvironment.MAX_RESIN.ToString();
                    break;
            }
        }

        private void UpdatePreviewStyle()
        {
            const int PADDING = 15;

            roundedDrawable.SetColor(Color.ParseColor($"#{opacity:X2}{color}"));

            var rootLayout = FindViewById<LinearLayout>(Resource.Id.ResinWidgetRootLayout);

            rootLayout.Background = roundedDrawable;
            rootLayout.SetPadding(PADDING, PADDING, PADDING, PADDING);

            FindViewById<FrameLayout>(Resource.Id.WidgetSettingPreviewLayout).SetBackgroundColor(color.Equals("000000") ? Color.White : Color.Black);
        }
    }
}