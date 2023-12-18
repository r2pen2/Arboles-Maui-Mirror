namespace ArbolesMAUI;
using ArbolesMAUI.Views;
using ArbolesMAUI.ViewModels;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using ArbolesMAUI.DB;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        MainPage = new LoginPage(); //new AppShell();
        LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;
    }

    private void OnNotificationActionTapped(NotificationActionEventArgs e)
    {
        if (e.IsDismissed)
        {

        }

        if (e.IsTapped)
        {
            string[] stringLocation = e.Request.ReturningData.Split(',');
            Location location = new Location(Convert.ToDouble(stringLocation[0]), Convert.ToDouble(stringLocation[1]));
            if (ViewMediator.FirstMapLoad) ViewMediator.UserSelectedLocation = location;

            else MapUtil.MoveViewToAbsoluteZoom(location, 20);
        }
    }


    protected override void OnStart()
    {
        ViewMediator.MapZoomLevel = 12;
        System.Diagnostics.Debug.WriteLine($"ZoomLevel: {ViewMediator.MapZoomLevel}");
        ViewMediator.FirstMapLoad = true;
        //Sets the theme of the app to light mode
        UserAppTheme = AppTheme.Light;
        base.OnStart();
    }
}
