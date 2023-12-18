using ArbolesMAUI.ViewModels;
using CommunityToolkit.Maui.Views;
using System.Runtime.Serialization;

namespace ArbolesMAUI.Views;

/**
 * Authored by Jared Chan
**/

public partial class Filter : ContentPage
{
	public Filter()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); //remove top nav bar from android

		//Initialize page state
        ViewMediator.IsFilterPageOpen = true;
		ViewMediator.FilterColorBlock = colorBlock;
		ViewMediator.FilterSpeciesButton = speciesButton;
	}

	private async void OnColorFilterClicked(object sender, EventArgs e)
	{
		ViewMediator.MethodOfIdentification = "By color";
		await Navigation.PushAsync(new CulturePage());
	}

	private async void OnNameFilterClicked(object sender, EventArgs e)
	{
        ViewMediator.MethodOfIdentification = "By species";
		await Navigation.PushAsync(new CulturePage());
    }


    private async void OnSaveClicked(object sender, EventArgs e)
	{
		if(colorBlock.BackgroundColor.ToHex() != Colors.LightGray.ToHex()) 
			MapFilterUtil.FilterByColor(ViewMediator.FilterColorBlock.BackgroundColor);
		if(resetCheckBox.IsChecked)
		{
			ViewMediator.Map.Pins.Clear();
			MapFilterUtil.ReloadAllMapPins();
		}
		if (speciesButton.Text != "Filter by species name")
			MapFilterUtil.FilterByName(speciesButton.Text);
        ViewMediator.IsFilterPageOpen = false;
        await Navigation.PopAsync();
    }
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        ViewMediator.IsFilterPageOpen = false;
        await Navigation.PopAsync();
    }
}