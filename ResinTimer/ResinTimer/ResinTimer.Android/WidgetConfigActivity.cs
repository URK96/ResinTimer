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

using Xamarin.Essentials;

namespace ResinTimer.Droid
{
    [Activity(Label = "WidgetConfigActivity", Name = "com.urk.resintimer.WidgetConfigActivity", Theme = "@style/ResinTimer", Exported = true)]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_CONFIGURE" })]
    public class WidgetConfigActivity : AppCompatActivity
    {
        const string KEY_CLICKUPDATE = "Key_ClickUpdate";
        const string VALUE_CLICKUPDATE = "Value_ClickUpdate";

        private AppWidgetManager widgetManager;
        private int widgetId;
        private int widgetLayoutId;
        private RemoteViews remoteView;
        private View previewView;
        private GradientDrawable roundedDrawable;

        private SeekBar opacitySeekBar;
        private int opacity = 0;
        private string color = "FFFFFF";

        private string selectedColor = "White";

        private string bgSelectKey;

        private string BGColor => $"#{opacity:X2}{color}";

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
                bgSelectKey = $"{SettingConstants.WIDGET_BACKGROUND}_{widgetId}";
            }
            else
            {
                return;
            }

            if (!Preferences.ContainsKey(bgSelectKey))
            {
                Preferences.Set(bgSelectKey, selectedColor);
            }

            widgetManager = AppWidgetManager.GetInstance(this);
            widgetLayoutId = widgetManager.GetAppWidgetInfo(widgetId).InitialLayout;

            remoteView = new RemoteViews(PackageName, widgetLayoutId);
            previewView = LayoutInflater.Inflate(widgetLayoutId, null);
            roundedDrawable = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.rounded_background, null) as GradientDrawable;

            //opacitySeekBar = FindViewById<AndroidX.AppCompat.Widget.AppCompatSeekBar>(Resource.Id.WidgetConfigureOpacitySeekBar);
            //opacitySeekBar.ProgressChanged += (sender, e) =>
            //{
            //    opacity = e.Progress;

            //    UpdatePreviewStyle();
            //};

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
                    ApplyWidgetSetting();
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
            Preferences.Set(bgSelectKey, selectedColor);

            UpdateLayout();

            var resultValue = new Intent();

            resultValue.PutExtra(AppWidgetManager.ExtraAppwidgetId, widgetId);
            SetResult(Result.Ok, resultValue);
            Finish();
        }

        private void UpdateLayout()
        {
            var remoteViews = (Preferences.Get($"{SettingConstants.WIDGET_BACKGROUND}_{widgetId}", "White")) switch
            {
                "Black" => new RemoteViews(PackageName, Resource.Layout.ResinWidget_Black),
                _ => new RemoteViews(PackageName, Resource.Layout.ResinWidget)
            };

            remoteViews.SetTextViewText(Resource.Id.ResinWidgetCount, ResinEnvironment.Resin.ToString());
            remoteViews.SetTextViewText(Resource.Id.ResinWidgetEndTime, ResinEnvironment.EndTime.ToString());

            remoteViews.SetOnClickPendingIntent(Resource.Id.ResinWidgetRootLayout, CreateClickIntent());

            widgetManager.UpdateAppWidget(widgetId, remoteViews);
        }

        private PendingIntent CreateClickIntent()
        {
            var intent = new Intent(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, new int[] { widgetId });
            intent.PutExtra(KEY_CLICKUPDATE, VALUE_CLICKUPDATE);

            return PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
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

                selectedColor = (sender as RadioButton).Id switch
                {
                    Resource.Id.WidgetConfigureBackgroundColorBlack => "Black",
                    _ => "White"
                };

                UpdatePreviewStyle();
            }
        }

        private void SetPreviewValue()
        {
            switch (widgetLayoutId)
            {
                case Resource.Layout.ResinWidget:
                    previewView.FindViewById<TextView>(Resource.Id.ResinWidgetCount).Text = ResinEnvironment.MaxResin.ToString();
                    previewView.FindViewById<TextView>(Resource.Id.ResinWidgetEndTime).Text = DateTime.Now.ToString();
                    break;
                case Resource.Layout.ResinWidgetSimple:
                    previewView.FindViewById<TextView>(Resource.Id.ResinWidgetSimpleCount).Text = ResinEnvironment.MaxResin.ToString();
                    break;
            }
        }

        private void UpdatePreviewStyle()
        {
            const int PADDING = 15;

            int drawableId = selectedColor switch
            {
                "Black" => Resource.Drawable.rounded_background_black,
                _ => Resource.Drawable.rounded_background
            };

            //roundedDrawable.SetColor(Color.ParseColor($"#{opacity:X2}{color}"));

            var rootLayout = FindViewById<LinearLayout>(Resource.Id.ResinWidgetRootLayout);

            rootLayout.Background = ResourcesCompat.GetDrawable(Resources, drawableId, null);
            //rootLayout.SetPadding(PADDING, PADDING, PADDING, PADDING);

            FindViewById<FrameLayout>(Resource.Id.WidgetSettingPreviewLayout).SetBackgroundColor(color.Equals("000000") ? Color.White : Color.Black);
        }
    }
}