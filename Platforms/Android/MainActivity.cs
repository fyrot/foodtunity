using Android.App;
using Android.Content.PM;
using Android.OS;

namespace BlazorMudBlazorCACApp1
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            Window.SetNavigationBarColor(Android.Graphics.Color.Rgb(21, 21, 21));

            base.OnCreate(savedInstanceState);
        }
    }
}
