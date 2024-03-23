using Android.App;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using QDockX.Debug;

namespace QDockX;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity
{
    private MyOrientationEventListener ocl;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        ocl = new(this);        
    }

    protected override void OnPause()
    {
        ocl.Disable();
        base.OnPause();
    }

    protected override void OnResume()
    {
        base.OnResume();
        ocl.Enable();
    }

}

public class MyOrientationEventListener : OrientationEventListener
{
    private readonly Android.Content.Context main = null;
    public MyOrientationEventListener(Android.Content.Context context) : base(context) { main = context; }
    public MyOrientationEventListener(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) {}
    public MyOrientationEventListener(Android.Content.Context context, [GeneratedEnum] SensorDelay rate) : base(context, rate) { main = context; }
    public override void OnOrientationChanged(int orientation)
    {
        if (main is Activity act)
        {
            switch (orientation)
            {
                case 0:
                    act.RequestedOrientation = ScreenOrientation.Portrait;
                    break;
                case 180:
                    act.RequestedOrientation = ScreenOrientation.ReversePortrait;
                    break;
            }
        }
    }
}