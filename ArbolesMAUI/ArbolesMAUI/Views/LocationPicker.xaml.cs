using ArbolesMAUI.ViewModels;
using GoogleApi.Entities.Common.Enums;
using MauiLocalization.Resources.Localization;
using Microsoft.Maui.Controls.Maps;
using System.Globalization;

namespace ArbolesMAUI.Views;

public partial class LocationPicker : ContentPage
{

    bool menuOpen = false;
    Location currentSelectedLocation;
    Location lastSearchedLocation;

    private CultureInfo culture = LocalizationResourceManager.Instance.Culture;
    private Language lang = Language.English;

    public LocationPicker()
    {
        InitializeComponent();

        mapFrame.Content = ViewMediator.Map;
        ViewMediator.chosePinAsLocation += ChosePinAsLocation; 

        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);


    }

    private void ChosePinAsLocation(object sender, EventArgs e)
    {
        string str = "Used Pin";
        ViewMediator.SetSpeciesColorBlockToPin();
        ViewMediator.LocationPickerButton.Text = str;
        CloseLocationPickerPage();
    }

    public void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        //Location center = ViewMediator.Map.VisibleRegion.Center;

        //System.Diagnostics.Debug.WriteLine($"MapClick: {center.Latitude}, {center.Longitude}");

        //LocLabel.Text = "Hello";

    }

    protected async override void OnAppearing()
    {
        // Ask for location permission when first opening the app
        await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        OpenMapMenuExpander(false);

        //if (ViewMediator.FirstMapLoad == false)
        //{
        await MapUtil.MoveViewToUser();
        //ViewMediator.FirstMapLoad = true;
        //}
       

    }

    

    public void OnFilterClicked(object? sender, EventArgs args)
    {
        OpenMapMenuExpander(false);

    }
    public async void OnCenterClicked(object? sender, EventArgs args)
    {
        OpenMapMenuExpander(false);

        //center map on user
        Location userLoc = await MapUtil.MoveViewToUser();
        currentSelectedLocation = userLoc;
        //await MapUtil.

    }
    public void OnSearchClicked(object? sender, EventArgs args)
    {
        OpenMapMenuExpander(false);
        ToggleSearchBarVisibility();

    }

    private void OnLocSearchFocused(object sender, EventArgs e)
    {
        string useCurrentLocationText;
        if (LocalizationResourceManager.Instance.Culture.Name.ToLower() == "es-cr") useCurrentLocationText = PlacesUtil.useCurrentLocationES;
        else useCurrentLocationText = PlacesUtil.useCurrentLocationEN;

        LocSearchResults.IsVisible = true;
        SearchResultsFrame.IsVisible = true;

        List<string> defaultList = new List<string>
        {
            useCurrentLocationText
        };
        LocSearchResults.ItemsSource = defaultList;
    }

    private void OnLocSearchUnfocused(object sender, EventArgs e)
    {
        LocSearchResults.IsVisible = false;
        SearchResultsFrame.IsVisible = false;
    }

    private async void OnLocTextChanged(object sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;

        List<string> readableResponseList = await PlacesUtil.HandleSearchBarPredictions(searchBar, LocSearchResults, lang);

        LocSearchResults.ItemsSource = readableResponseList;
    }

    private void ToggleSearchBarVisibility()
    {
        if (LocSearchBarFrame.IsVisible == false)
        {
            LocSearchBarFrame.IsVisible = true;

        }
        else if (LocSearchBarFrame.IsVisible == true)
        {
            LocSearchBarFrame.IsVisible = false;
            SearchResultsFrame.IsVisible = false;

        }

    }
    private async void OnLocItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        LocSearchBar.Text = args.SelectedItem.ToString();
        LocSearchResults.IsVisible = false;
        LocSearchBar.Unfocus();
        Location loc = await PlacesUtil.ListViewPlaceToLocation(LocSearchResults, lang);
        currentSelectedLocation = loc;
        lastSearchedLocation = loc;
        MapUtil.MoveViewTo(loc);
        LocSearchBarFrame.IsVisible = false;
        LocSearchBar.Text = null;


    }

    public void OnExpandClicked(object? sender, EventArgs args)
    {
        menuOpen = !menuOpen;
        ExpandButton.Source = menuOpen ? ImageSource.FromFile("angle_up_solid.svg") : ImageSource.FromFile("angle_down_solid.svg");
    }

    public void OnBackClicked(object? sender, EventArgs args)
    {
        //Application.Current.MainPage = ViewMediator.UploadPage;
        CloseLocationPickerPage();
    }

    public void OnCheckClicked(object? sender, EventArgs args)
    {
        Location center = ViewMediator.Map.VisibleRegion.Center;
        //Application.Current.MainPage = ViewMediator.UploadPage;
        currentSelectedLocation = center;
        //Set the location used by the upload page to the center of the map
        ViewMediator.UserSelectedLocation = center;
        ViewMediator.UsePinAsLocation = false;
        String lat = String.Format("{0:#0.####}",center.Latitude);
        String lon = String.Format("{0:#0.####}", center.Longitude);
        String str = $"Location: {lat}, {lon}";
        ViewMediator.LocationPickerButton.Text = str;
        CloseLocationPickerPage();
    }

    public void OpenMapMenuExpander(bool open)
    {
        MapExpander.IsExpanded = open;

    }



    public async void CloseLocationPickerPage()
    {
        await Navigation.PopAsync();
    }

}