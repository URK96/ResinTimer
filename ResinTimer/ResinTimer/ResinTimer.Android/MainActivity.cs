using Android.App;
using Android.Content.PM;
using Microsoft.Maui;

namespace ResinTimer.Droid;

// TODO: The original platform specific bootstrapping code has been archived as MainActivity.cs.original. ActivityAttributes should be migrated from the archived file to this one and any additional changes need to be manually migrated to MauiProgram.cs
// See Android App Lifecycle: https://learn.microsoft.com/dotnet/maui/fundamentals/app-lifecycle#android
// See MainActivity: https://learn.microsoft.com/dotnet/maui/android/manifest#activity-name

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
}
