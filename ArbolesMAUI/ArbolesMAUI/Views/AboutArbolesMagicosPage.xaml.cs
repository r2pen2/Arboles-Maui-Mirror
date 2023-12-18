using MauiLocalization.Resources.Localization;
using System.Globalization;

namespace ArbolesMAUI.Views;

public partial class AboutArbolesMagicosPage : ContentPage
{
	public AboutArbolesMagicosPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        BindingContext = this;
    }

    async void SocialIconClicked(object sender, EventArgs e)
    {
        ImageButton btn = (ImageButton)sender;
        
        // Get the x:name string from the
        string name = btn.StyleId;
        string url = "";
        switch (name)
        {
            case "facebookButton":
                url = "https://www.facebook.com/arbolesmagicoscostarica";
                break;
            case "instagramButton":
                url = "https://www.instagram.com/arbolesmagicos/";
                break;
            case "webButton":
                url = "https://arbolesmagicos.org/";
                break;
            case "emailButton":
                url = "mailto:hola@arbolesmagicos.org";
                break;
            default:
                break;
        }
        try
        {
            Uri uri = new Uri(url);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            // An unexpected error occurred. No browser may be installed on the device.
            System.Diagnostics.Debug.WriteLine(ex);

        }
    }
}