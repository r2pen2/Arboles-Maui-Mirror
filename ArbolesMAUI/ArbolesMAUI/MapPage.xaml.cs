using ArbolesMAUI.DB;
using ArbolesMAUI.ViewModels;
using ArbolesMAUI.Views;
using CommunityToolkit.Maui.Views;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Places.AutoComplete.Request;
using GoogleApi.Entities.Places.AutoComplete.Response;
using GoogleApi.Entities.Places.Common;
using GoogleApi.Entities.Places.Search.Text.Request;
using GoogleApi.Entities.Places.Search.Text.Response;
using Java.Lang;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using System.Globalization;
using Google.Cloud.Firestore;
using ArbolesMAUI.DB.ObjectManagers;
using static ArbolesMAUI.ViewModels.MapUtil;
using MauiLocalization.Resources.Localization;

namespace ArbolesMAUI;

public partial class MapPage : ContentPage
{
    bool menuOpen = false;
    int numPageLoads = 0;
    private CultureInfo culture = LocalizationResourceManager.Instance.Culture;
    private Language lang = Language.English;

    public MapPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);

        mapFrame.Content = ViewMediator.Map;

        MapUtil.AddAllPinsFromDB();

        CheckCameraEnable();
    }

    void CheckCameraEnable() {
        CameraButton.IsVisible = (DBManager.currentUser != null) && (!DBManager.currentUserManager.getIsBanned());
        CameraButton.IsEnabled = (DBManager.currentUser != null) && (!DBManager.currentUserManager.getIsBanned());
    }

    public void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"MapClick: {e.Location.Latitude}, {e.Location.Longitude}");

        //Location clickedLoc = new Location(e.Location);

        //MapUtil.MoveViewTo(clickedLoc);

    }

    public async void OnAddPinClicked(object sender, EventArgs args)
    {
        DisplayUploadPage(); //display popup for adding new tree + details            
    }


    protected async override void OnAppearing()
    {
        // Ask for location permission when first opening the app
        await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        OpenMapMenuExpander(false);

        if(numPageLoads == 0)
        {
            Location location = await Geolocation.Default.GetLastKnownLocationAsync();
            MapUtil.MoveViewToAbsoluteZoom(location, 12);
        }
        else
        {
            if (ViewMediator.UserSelectedLocation != null)
            {
                MapUtil.MoveViewTo(ViewMediator.UserSelectedLocation);
            }
            else
            {
                Location location = await Geolocation.Default.GetLastKnownLocationAsync();
                MapUtil.MoveViewToAbsoluteZoom(location, 12);
            }
        }
        

        //if (ViewMediator.FirstMapLoad == true)
        //{
        //    await MapUtil.MoveViewToUser();
        //    ViewMediator.FirstMapLoad = false;
        //}
        //else if(ViewMediator.UserSelectedLocation != null)
        //{
        //    MapUtil.MoveViewTo(ViewMediator.UserSelectedLocation);
        //}

        CheckCameraEnable();
        numPageLoads++;
    }

    //protected override bool OnBackButtonPressed() {
    //    return true;
    //}

    public async void DisplayUploadPage()
    {
        await Navigation.PushAsync(new UploadPage());
    }

    public async void OnFilterClicked(object? sender, EventArgs args)
    {
        OpenMapMenuExpander(false);

        await Navigation.PushAsync(new Filter());
    }
    public async void OnCenterClicked(object sender, EventArgs args)
    {
        OpenMapMenuExpander(false);

        //center map on user
        await MapUtil.MoveViewToUser();

    }
    public void OnSearchClicked(object sender, EventArgs args)
    {
        OpenMapMenuExpander(false);
        ToggleSearchBarVisibility();

    }

    private bool _rotated = false;

    public async void OnExpandClicked(object sender, EventArgs args)
    {
        // MapMenu.Opacity = 0;

        // menuOpen = !menuOpen;

        ExpandButton.RotateTo(_rotated ? 0 : 180);

        // _rotated = !_rotated;

        // MapExpander.Margin = new Thickness(0, 0, _rotated ? 0 : -100, 50);

        MapExpander.Animate<Thickness>("ExpandButton",
            value =>
            {
                int factor = Convert.ToInt32(value * 10);

                var leftMargin =
                    !_rotated
                    ? (factor * 10) - 100
                    : (factor * 10) * -1;
                return new Thickness(10, 0, leftMargin, 40);
            },
            newThickness => MapExpander.Margin = newThickness
            , length: 100
            , finished: (_, __) => _rotated = !_rotated);

        // ExpandButton.Source = menuOpen ? ImageSource.FromFile("angle_up_solid.svg") : ImageSource.FromFile("angle_down_solid.svg");

        // await MapMenu.FadeTo(.85, 500, Easing.Linear);
        // await MapMenu.TranslateTo(0, 0, 100, Easing.SinIn);

    }

    public void OnCameraClicked(object? sender, EventArgs args)
    {
        ViewMediator.UserSelectedLocation = ViewMediator.Map.VisibleRegion.Center;
        if (ViewMediator.UploadPageOpen == false)
        {
            ViewMediator.UploadPageOpen = true;
            DisplayUploadPage(); //display popup for adding new tree + details  
        }
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
        if(LocSearchBarFrame.IsVisible== false)
        {
            LocSearchBarFrame.IsVisible = true;

        }
        else if(LocSearchBarFrame.IsVisible == true)
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
        MapUtil.MoveViewTo(loc);
        LocSearchBarFrame.IsVisible = false;
        LocSearchBar.Text = null;


    }



    public void OpenMapMenuExpander(bool open)
    {
        MapExpander.IsExpanded = open;

    }
}