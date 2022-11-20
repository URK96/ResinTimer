using Microsoft.Toolkit.Uwp.Notifications;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Xamarin.Essentials;

using ResinTimer.Resources;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Data.Xml.Dom;
using ResinTimer.Helper;

namespace ResinTimer.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static BackgroundTaskDeferral AppServiceDeferral;
        public static AppServiceConnection Connection;
        public static bool IsBackground = false;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            //this.Suspending += OnSuspending;
            //this.EnteredBackground += delegate 
            //{ 
            //    IsBackground = true;
            //};
            //this.LeavingBackground += delegate 
            //{ 
            //    IsBackground = false; 
            //};
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if ((e.Kind == ActivationKind.StartupTask) &&
                Preferences.ContainsKey(SettingConstants.NOTI_LIST) &&
                Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                NotiBootstrap();
            }
            else
            {
                InitializeApp(e);
                Platform.OnLaunched(e);
            }
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            if ((args.Kind == ActivationKind.StartupTask) &&
                Preferences.ContainsKey(SettingConstants.NOTI_LIST) &&
                Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                NotiBootstrap();
            }
            else
            {
                InitializeApp(args);
            }
        }

        private async void NotiBootstrap()
        {
            var builder = new ToastContentBuilder();

            try
            {
                var infos = await AppDiagnosticInfo.RequestInfoForAppAsync();
                var resourceInfos = infos[0].GetResourceGroups();
                await resourceInfos[0].StartSuspendAsync();

                if (UWPAppEnvironment.toastNotifier == null)
                {
                    UWPAppEnvironment.toastNotifier = ToastNotificationManager.CreateToastNotifier();
                }

                var notiUWP = new NotiScheduleUWP();

                notiUWP.CancelAll();
                notiUWP.ScheduleAll();

                builder.AddToastActivationInfo("BootNotiRegister", ToastActivationType.Foreground);
                builder.AddText(AppResources.BootAlarmRegisterSuccess);
            }
            catch (Exception)
            {
                builder.AddText(AppResources.BootAlarmRegisterFail);
            }
            finally
            {
                if (builder != null)
                {
                    UWPAppEnvironment.toastNotifier.AddToSchedule(
                        new ScheduledToastNotification(builder.GetToastContent().GetXml(), DateTime.Now.AddSeconds(3)));
                }

                await Task.Delay(3000);

                Exit();
            }
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            if (Preferences.Get(SettingConstants.APP_BACKGROUNDTRAYSERVICE_ENABLED, false) &&
                (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details))
            {
                AppServiceDeferral = args.TaskInstance.GetDeferral();
                Connection = details.AppServiceConnection;

                args.TaskInstance.Canceled += delegate { AppServiceDeferral?.Complete(); };
                Connection.RequestReceived += Connection_RequestReceived;
            }
            else
            {
                args.TaskInstance.GetDeferral()?.Complete();
            }
        }

        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            const string UpdateAllRequestKey = "UpdateAllRequest";
            const string ResinInfoKey = "ResinInfo";
            const string RealmCoinInfoKey = "RealmCoinInfo";
            const string RealmFriendshipInfoKey = "RealmFriendshipInfo";

            ValueSet message = args.Request.Message;

            if (message.ContainsKey("content"))
            {
                object val = null;
                args.Request.Message.TryGetValue("content", out val);

                ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode("UWP with Systray"));
                toastTextElements[1].AppendChild(toastXml.CreateTextNode(val.ToString()));

                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }

            if (message.ContainsKey(UpdateAllRequestKey))
            {
                // Resin Info Load & Update

                ResinEnvironment.LoadValues();
                ResinEnvironment.CalcResin();
                ResinEnvironment.CalcResinTime();

                if (ResinEnvironment.IsSyncEnabled)
                {
                    await SyncHelper.Update(SyncHelper.SyncTarget.Resin);
                }

                ResinEnvironment.UpdateSaveData();


                // Realm Currency Info Load & Update

                RealmCurrencyEnvironment.LoadValues();
                RealmCurrencyEnvironment.CalcRC();

                if (RealmCurrencyEnvironment.IsSyncEnabled)
                {
                    await SyncHelper.Update(SyncHelper.SyncTarget.RealmCurrency);
                }

                RealmCurrencyEnvironment.UpdateSaveData();


                // Realm Friendship Info Load & Update

                RealmFriendshipEnvironment.LoadValues();
                RealmFriendshipEnvironment.CalcRF();

                RealmFriendshipEnvironment.SaveValue();

                await Connection?.SendMessageAsync(new ValueSet()
                {
                    { ResinInfoKey, null },
                    { "NowResin", ResinEnvironment.Resin },
                    { "MaxResin", ResinEnvironment.MaxResin },
                    { "ResinRemainTime", ResinEnvironment.TotalCountTime },
                    { "IsResinSync", ResinEnvironment.IsSyncEnabled },
                    { RealmCoinInfoKey, null },
                    { "NowRC", RealmCurrencyEnvironment.Currency },
                    { "MaxRC", RealmCurrencyEnvironment.MaxRC },
                    { "RCRemainTime", RealmCurrencyEnvironment.TotalCountTime },
                    { "IsRealmCoinSync", RealmCurrencyEnvironment.IsSyncEnabled },
                    { RealmFriendshipInfoKey, null },
                    { "NowRF", RealmFriendshipEnvironment.Bounty },
                    { "MaxRF", RealmFriendshipEnvironment.MaxRF },
                    { "RFRemainTime", RealmFriendshipEnvironment.TotalCountTime },
                });
            }

            if (message.ContainsKey("exit"))
            {
                Current.Exit();
            }
        }

        private void InitializeApp(IActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                Rg.Plugins.Popup.Popup.Init();

                var assembliesToInclude = new List<Assembly>
                {
                    typeof(Syncfusion.SfGauge.XForms.UWP.SfGaugeRenderer).GetTypeInfo().Assembly,
                    typeof(Syncfusion.SfNumericUpDown.XForms.UWP.SfNumericUpDownRenderer).GetTypeInfo().Assembly,
                    typeof(Syncfusion.SfNumericUpDown.XForms.UWP.SfNumericUpDownRenderer).GetTypeInfo().Assembly
                };
                assembliesToInclude.AddRange(Rg.Plugins.Popup.Popup.GetExtraAssemblies());

                Xamarin.Forms.Forms.SetFlags("SwipeView_Experimental");

                Xamarin.Forms.Forms.Init(e, assembliesToInclude);

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                if (e is LaunchActivatedEventArgs args)
                {
                    rootFrame.Navigate(typeof(MainPage), (e as LaunchActivatedEventArgs).Arguments);
                }
                else
                {
                    rootFrame.Navigate(typeof(MainPage), e);
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            try
            {
                var deferral = e.SuspendingOperation?.GetDeferral();

                //TODO: Save application state and stop any background activity
                AppServiceDeferral?.Complete();
                deferral?.Complete();
            }
            catch { }
        }
    }
}
