
using Android.App.AppSearch;
using Android.Gms.Auth.Api.SignIn.Internal;
using Android.Gms.Maps.Model;
using Android.OS;
using ArbolesMAUI.DB;
using ArbolesMAUI.DB.ObjectManagers;
using ArbolesMAUI.Model;
using ArbolesMAUI.ViewModels;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using Google.Protobuf.WellKnownTypes;
using GoogleApi;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Places.AutoComplete.Request;
using GoogleApi.Entities.Places.AutoComplete.Response;
using GoogleApi.Entities.Places.Common;
using GoogleApi.Entities.Places.Search.Text.Request;
using GoogleApi.Entities.Places.Search.Text.Response;
using Java.Lang;
using MauiLocalization.Resources.Localization;
using Microsoft.Maui.Controls.Maps;
using System.Globalization;
using System.Text.RegularExpressions;
using static Android.Renderscripts.ScriptGroup;
using static ArbolesMAUI.DB.ObjectManagers.ReportManager;
using ImageMagick;

using Cloudmersive.APIClient.NET.ImageRecognition.Api;
using Cloudmersive.APIClient.NET.ImageRecognition.Client;
using Cloudmersive.APIClient.NET.ImageRecognition.Model;
using Microsoft.Maui.Graphics.Platform;

namespace ArbolesMAUI.Views;

/**
 * Authored by Jared Chan, Joe Dobbelaar, & Cole Parks
**/

public partial class UploadPage : ContentPage
{
    private CultureInfo culture = LocalizationResourceManager.Instance.Culture;
    private Language lang = Language.English;

    private double searchRadius = 4000.0;

    private FileResult imageFile;

    private byte[] imageByteArray;


    bool safeImage = false;
    bool checkingImage = true;


    UploadImage uploadImage { get; set; }


    public UploadPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetTabBarIsVisible(this, false);

        ViewMediator.chosePinAsLocation += ChosePinAsLocation;


        uploadImage = new UploadImage();

        if (culture.Name.ToLower() == "es-cr")
        {
            lang = Language.Spanish;
        }

    }


    // Runs when the user chooses a pin to use as a location in the TreePopup
    private void ChosePinAsLocation(object sender, EventArgs e)
    {
        //ViewMediator.Instance.SpeciesColorBlock = speciesColorBlock;

        //ViewMediator.Instance.SetSpeciesColorBlockToPin();

        speciesColorBlock.BackgroundColor = ViewMediator.ReportToAddTo.MauiColor;
    }

    private async void TakePhoto_Clicked(object sender, EventArgs e) 
    {
        imageFile = await uploadImage.TakePhoto();
        var uploadedImg = await uploadImage.Upload(imageFile);
        imageByteArray = uploadImage.StringToByteBase64(uploadedImg.byteBase64);
        Stream stream = uploadImage.ByteArrayToStream(imageByteArray);
        Image_Upload.Opacity = 1;
        Image_Upload.Source = ImageSource.FromStream(() =>
            stream
        );
        checkImage();
    }

    private async void checkImage() {
        checkingImage = true;
	string cloudmersiveApiKey = "API KEY"; // Arboles Magicos needs their own key. See README.
        Configuration.Default.AddApiKey("Apikey", cloudmersiveApiKey);
        var apiInstance = new NsfwApi();
        // Compress image
        MemoryStream imageStream = new MemoryStream(imageByteArray);
        Microsoft.Maui.Graphics.IImage compressedImage = PlatformImage.FromStream(imageStream);
        compressedImage = compressedImage.Downsize(300);
        byte[] compressedByteArray = compressedImage.AsBytes();
        try {
            // Not safe for work (NSFW) racy content classification
            MemoryStream compressedStream = new MemoryStream(compressedByteArray);
            NsfwResult result = await apiInstance.NsfwClassifyAsync(compressedStream);
            safeImage = (result.Score < 0.1);
            checkingImage = false;
        } catch (System.Exception apiEx) {
            Console.WriteLine("Exception when calling NsfwApi.NsfwClassify: " + apiEx.Message);
            checkingImage = false;
            safeImage = true;
        }
    }

    private async void UploadImage_Clicked(object sender, EventArgs e)
    {
        imageFile = await uploadImage.OpenMediaPickerAsync();
        var uploadedImg = await uploadImage.Upload(imageFile);
        imageByteArray = uploadImage.StringToByteBase64(uploadedImg.byteBase64);
        Stream stream = uploadImage.ByteArrayToStream(imageByteArray);
        Image_Upload.Source = ImageSource.FromStream(() =>
            stream
        );
        checkImage();
    }

    
    private void OnLocSearchFocused(object sender, EventArgs e)
    {
        string useCurrentLocationText;
        if (LocalizationResourceManager.Instance.Culture.Name.ToLower() == "es-cr") useCurrentLocationText = PlacesUtil.useCurrentLocationES;
        else useCurrentLocationText = PlacesUtil.useCurrentLocationEN;

        LocSearchResults.IsVisible = true;
        List<string> defaultList = new List<string>
        {
            useCurrentLocationText
        };
        LocSearchResults.ItemsSource = defaultList;
    }

    private void OnLocSearchUnfocused(object sender, EventArgs e)
    {
        LocSearchResults.IsVisible = false;
    }

    private async void OnLocTextChanged(object sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;

        List<string> readableResponseList = await PlacesUtil.HandleSearchBarPredictions(searchBar, LocSearchResults, lang);

        LocSearchResults.ItemsSource = readableResponseList;
    }

    private void OnLocItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        LocSearchBar.Text = args.SelectedItem.ToString();
        LocSearchResults.IsVisible = false;
        LocSearchBar.Unfocus();
    }

    private async void OnSelectSpeciesClicked(object sender, EventArgs e)
    {
        ViewMediator.MethodOfIdentification = await DisplayActionSheet("Identify the tree:", "Cancel", null, "By color", "By species");
        if (ViewMediator.MethodOfIdentification != "Cancel")
        {
            ViewMediator.SelectSpeciesButton = selectSpeciesButton;
            ViewMediator.SpeciesColorBlock = speciesColorBlock;
            await Navigation.PushAsync(new CulturePage());
        }
    }

    private async void OnCreateButtonClicked(object sender, EventArgs e)
    {
        Location loc;
        //Location loc = await PlacesUtil.ListViewPlaceToLocation(LocSearchResults, lang);
        if (ViewMediator.UsePinAsLocation == true)
        {
            loc = new Location(ViewMediator.ReportToAddTo.Location.Longitude, ViewMediator.ReportToAddTo.Location.Latitude);
        }
        else
        {
            loc = ViewMediator.UserSelectedLocation;

        }
        if (loc == null)
        {
            await Application.Current.MainPage.DisplayAlert("Location Error", "Please select a location.", "Ok");
            System.Diagnostics.Debug.WriteLine("NULL LOCATION");

        } else if (speciesColorBlock.BackgroundColor.ToHex().ToLower().Equals("#d3d3d3")) {
            await Application.Current.MainPage.DisplayAlert("Color Error", "Please select a species color", "Ok");
        }

        else {

            // Inputs are valid, check if innapropriate
            // Configure API key authorization: Apikey

            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Apikey", "Bearer");

            if (checkingImage) {
                await Application.Current.MainPage.DisplayAlert("Please Wait", "We are still checking that your contribution is not NSFW", "Ok");
            } else if (!safeImage) {
                await Application.Current.MainPage.DisplayAlert("Content Filter", "Image recognition returned NSFW", "Ok");
            } else {
                string treeName = "";
                if (selectSpeciesButton.Text != "Identify the tree") treeName = selectSpeciesButton.Text;
                // If UsePinAsLocation is true, this contribution is not the first contribution for that tree, and the contribution is added to the existing tree
                if (ViewMediator.UsePinAsLocation == true)
                {
                    MapUtil.AddContributionToReport(ViewMediator.ReportToAddTo, imageFile, imageByteArray);
                }
                else 
                { 
                    MapUtil.PushPinToDB(loc, datePicker.Date, imageFile, imageByteArray, treeName, speciesColorBlock.BackgroundColor);
                }
                ViewMediator.UploadPageOpen = false;
                Application.Current.MainPage = new NavigationPage(new AppShell());   
            }

            //CloseUploadPage();

            //var pageToRemove = Navigation.NavigationStack.FirstOrDefault(p => p.GetType() == typeof(MapPage));
            //if (pageToRemove != null)
            //{
            //    Navigation.RemovePage(pageToRemove);
            //    System.Diagnostics.Debug.WriteLine("REMOVED MainPage");
            //}

            //await Navigation.PushAsync(new MapPage());
        }
    }

    private async void LocPickerButton_Clicked(object sender, EventArgs e)
    {   
        // Set the pin selector to the defaults
        ViewMediator.UsePinAsLocation = false;
        
        // Sets the ViewMediator LocationPickerButton instance to the pointer of the LocationPicker button
        ViewMediator.LocationPickerButton = LocationPicker;
        await Navigation.PushAsync(new LocationPicker());
    }

    private void OnCancelButtonClicked(object sender, EventArgs e)
    {
        ViewMediator.UploadPageOpen = false;
        CloseUploadPage();
    }

    private async void CloseUploadPage()
    {
        await Navigation.PopAsync();
    }

}
