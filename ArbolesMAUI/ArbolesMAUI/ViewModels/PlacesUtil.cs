using Android.Gms.Common.Api.Internal;
using ArbolesMAUI.DB;
using GoogleApi.Entities.Places.Search.Text.Request;
using GoogleApi.Entities.Places.Search.Text.Response;
using GoogleApi;
using Java.Lang;
using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Globalization;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Common;
using ArbolesMAUI.DB.ObjectManagers;
using static ArbolesMAUI.DB.ObjectManagers.ReportManager;
using GoogleApi.Entities.Places.AutoComplete.Request;
using GoogleApi.Entities.Places.AutoComplete.Response;
using GoogleApi.Entities.Places.Common;
using MauiLocalization.Resources.Localization;

/**
 * Authored by Jared Chan
 **/

namespace ArbolesMAUI.ViewModels
{
    /// <summary>
    /// Utility class for logic related to handling Google Places API requests/responses 
    /// </summary>
    public static class PlacesUtil
    {
        public static readonly string API_KEY = "AIzaSyDGhlFJCaue9S4BOnceNusr57o_a07lvMQ";

        /// <summary>
        /// Search radius used in API requests (responses are only within this radius)
        /// </summary>
        private static double searchRadius = 4000.0;
        public static string useCurrentLocationEN = "Use Current Location";
        public static string useCurrentLocationES = "Usar Current Location";
        private static string useCurrentLocationText;

        /// <summary>
        /// Logic for handling how a listview is populated with addresses/places based on text search within a search bar
        /// </summary>
        /// <param name="searchBar">SearchBar object</param>
        /// <param name="listView">ListView Object</param>
        /// <param name="lang">Currently set culture</param>
        /// <returns></returns>
        public static async Task<List<string>> HandleSearchBarPredictions(SearchBar searchBar, ListView listView, Language lang)
        {
            //translates the "Use Current Location" text 
            if (LocalizationResourceManager.Instance.Culture.Name.ToLower() == "es-cr") useCurrentLocationText = useCurrentLocationES;
            else useCurrentLocationText = useCurrentLocationEN;

            ListView LocSearchResults = listView;

            Location currLoc = await MapUtil.GetCurrentLocation();

            //Initially populates the search results list view with just "Use Current Location" text
            List<string> readableResponseList = new List<string>
            {
                useCurrentLocationText
            };

            //if search bar is empty or user writes only whitespace, set list view visible,
            // & sends a request to Google API; populates list view search results from API response 
            if (!string.IsNullOrEmpty(searchBar.Text))
            {
                LocSearchResults.IsVisible = true;

                //send request of data to API
                PlacesAutoCompleteRequest request = new PlacesAutoCompleteRequest
                {

                    Key = API_KEY,
                    Input = searchBar.Text,
                    Language = lang,
                    LocationBias = new LocationBias
                    {
                        Location = new Coordinate(currLoc.Latitude, currLoc.Longitude),
                        Radius = searchRadius
                    }
                };

                //Retrieve response from our request
                PlacesAutoCompleteResponse response = await GooglePlaces.AutoComplete.QueryAsync(request);

                //populate list view search results
                foreach (Prediction pred in response.Predictions.ToList())
                {
                    readableResponseList.Add(pred.Description);
                }
            }
            return readableResponseList;
        }

        /// <summary>
        /// Converts a selected place/address in a listview to its corresponding Location coordinates
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="lang"></param>
        /// <returns>location coordinates of the address selected</returns>
        public static async Task<Location> ListViewPlaceToLocation(ListView listView, Language lang)
        {

            ListView LocSearchResults = listView;

            Location currLoc = await MapUtil.GetCurrentLocation();

            //Ensure an item is selected within the list view 
            if (LocSearchResults.SelectedItem != null && !string.IsNullOrEmpty(LocSearchResults.SelectedItem.ToString()))
            {
                //If user selects "Use Current Location" (translation handled as well)
                if (LocSearchResults.SelectedItem.ToString() == PlacesUtil.useCurrentLocationES || LocSearchResults.SelectedItem.ToString() == PlacesUtil.useCurrentLocationEN) return await MapUtil.GetCurrentLocation();
                
                //If user selects an address/place, send a request to Google Places API
                else
                {
                    PlacesTextSearchRequest request = new PlacesTextSearchRequest
                    {

                        Key = API_KEY,
                        Query = LocSearchResults.SelectedItem.ToString(),
                        Language = lang,
                        Location = new Coordinate(currLoc.Latitude, currLoc.Longitude),
                        Radius = searchRadius
                    };

                    //Handle response from our request
                    PlacesTextSearchResponse response = await GooglePlaces.Search.TextSearch.QueryAsync(request);
                    Coordinate coords = response.Results.FirstOrDefault().Geometry.Location;
                    return new Location(coords.Latitude, coords.Longitude);
                }
            }
            return null;
        }
    }
}
