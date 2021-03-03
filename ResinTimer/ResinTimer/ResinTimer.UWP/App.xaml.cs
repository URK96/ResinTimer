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

namespace ResinTimer.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
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

                var notiUWP = new ScheduledNotiUWP();

                notiUWP.CancelAll();
                notiUWP.ScheduleAllNoti();

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
                    UWPAppEnvironment.toastNotifier.AddToSchedule(new ScheduledToastNotification(builder.GetToastContent().GetXml(), DateTime.Now.AddSeconds(3)));
                }

                await Task.Delay(3000);

                Exit();
            }
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

        }

        private void InitializeApp(IActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                Rg.Plugins.Popup.Popup.Init();

                var assembliesToInclude = new List<Assembly>
                {
                    typeof(Syncfusion.SfGauge.XForms.UWP.SfGaugeRenderer).GetTypeInfo().Assembly,
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
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
