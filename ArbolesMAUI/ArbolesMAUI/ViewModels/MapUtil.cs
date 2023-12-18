using Microsoft.Maui.Controls.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;
using ArbolesMAUI.DB;
using ArbolesMAUI.DB.ObjectManagers;
using Microsoft.Maui.Maps;
using static ArbolesMAUI.DB.ObjectManagers.ReportManager;
using Google.Cloud.Firestore;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using Android.Graphics.Drawables;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Platform;
using IMap = Microsoft.Maui.Maps.IMap;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Maui.Views;
using ArbolesMAUI.Views;
using static Android.Gms.Common.Apis.Api;
using Android.OS;
using Java.Lang;
using Plugin.LocalNotification;

/**
 * Authored by Jared Chan, Joe Dobbelaar, and Cole Parks
 **/

namespace ArbolesMAUI.ViewModels
{
    /// <summary>
    /// ViewModel Class for MainPage (map) 
    /// Functionality is stored here
    /// </summary>
   
    public class MapUtil
    {
        private MapUtil() { }

        private static CancellationTokenSource _cancelTokenSource;
        private static bool _isCheckingLocation;

        private static ObservableCollection<ReportManager> reportManagers = null;

        /// <summary>
        /// Stores collection of Report Managers retrieved from DB
        /// </summary>
        public static ObservableCollection<ReportManager> ReportManagers
        {
            get
            {
                return reportManagers;
            }
        }

        /// <summary>
        /// Authored by Cole Parks
        ///  Moves map view to user's location
        /// </summary>
        public static async Task<Location> MoveViewToUser()
        {
            try
            {
                Location location = await Geolocation.Default.GetLastKnownLocationAsync();


                if (location != null)
                    if (ViewMediator.FirstMapLoad == true)
                    {
                        MoveViewToAbsoluteZoom(location, ViewMediator.MapZoomLevel);
                        ViewMediator.FirstMapLoad = false;
                    }
                    else 
                    { 
                        MoveViewTo(location);
                    }
                return location;

                //return $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}";
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine(fnsEx.Message);
                // Handle not supported on device exception
                return null;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Console.WriteLine(fneEx.Message);
                // Handle not enabled on device exception
                return null;

            }
            catch (PermissionException pEx)
            {
                Console.WriteLine(pEx.Message);
                // Handle permission exception
                return null;

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Unable to get location
                return null;

            }

        }

        /// <summary>
        /// Authored by Cole Parks
        ///  Moves the map view to the a given location
        /// </summary>
        /// <param name="loc"></param>
        public static void MoveViewTo(Location loc)
        {
            ViewMediator.Map.MoveToRegion(MapSpan.FromCenterAndRadius(loc, ViewMediator.Map.VisibleRegion.Radius));

        }

        /// <summary>
        ///  Moves the map view to the a given location and zoom level
        /// </summary>
        /// <param name="loc"></param>
        public static void MoveViewToAbsoluteZoom(Location loc, double zoom)
        {
            ViewMediator.MapZoomLevel = zoom;
            double latlongDegrees = 360 / (System.Math.Pow(2, zoom));
            ViewMediator.Map.MoveToRegion(new MapSpan(loc, latlongDegrees, latlongDegrees));
            //ViewMediator.Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(loc.Latitude, loc.Longitude), Distance.FromMiles(1)));
        }

        /// <summary>
        /// Authored by Cole Parks
        ///  Moves the map view to the a given location and zoom level
        /// </summary>
        /// <param name="loc"></param>
        public static void MoveViewToRelativeZoom(Location loc, double zoom)
        { 
            ViewMediator.Map.MoveToRegion(MapSpan.FromCenterAndRadius(loc, ViewMediator.Map.VisibleRegion.Radius).WithZoom(zoom));

        }




        /// <summary>
        /// Authored by Jared Chan
        /// Retrieves last cached location of the device
        /// </summary>
        /// <returns>
        /// Last cached Location
        /// </returns>

        public static async Task<Location> GetCachedLocation()
        {
            try
            {
                Location location = await Geolocation.Default.GetLastKnownLocationAsync();

                if (location != null)
                    return location;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine(fnsEx.Message);
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Console.WriteLine(fneEx.Message);
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx) {
                Console.WriteLine(pEx.Message);
                // Handle permission exception
            }
            catch (System.Exception ex) {
                Console.WriteLine(ex.Message);
                // Unable to get location
            }

            return null;
        }

        /// <summary>
        /// Authored by Jared Chan
        /// Retrieves current real-time location of the device
        /// </summary>
        /// <returns>
        /// Real-time current Location
        /// </returns>
        /// 
        public static async Task<Location> GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                    return location;
            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (System.Exception ex) 
            {
                Console.WriteLine(ex.Message);
                // Unable to get location
            }
            finally
            {
                _isCheckingLocation = false;
            }
            return null;
        }

        /// <summary>
        /// Authored by Jared Chan
        /// Cancels GetCurrentLocation retrieval
        /// </summary>
 
        public static void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }

        //Will be refactored to use Pin class as param; pin creation will be decoupled from this function

        /// <summary>
        /// Authored by Jared Chan & Joe Dobbelaar
        /// 
        /// Adds a new pin to the map
        /// 
        /// See Adding / Removing Pins in the README
        /// </summary>
        /// <param name="pinType">Pin Type</param>
        /// <param name="location">Location</param>
        /// <param name="label">Title for the pin</param>
        /// <param name="address">Pin address</param>

        public static async void AddNewPin(ReportManager report)
        {
            var pin = new CustomPin() {
                Report = report,
                Type = PinType.Place,
                Location = new Location(report.Location.Latitude, report.Location.Longitude),
                Label = "",
                Address = "",
                ImageSource = report.getPinImageSource(),
            };

            pin.MarkerClicked += async (s, args) =>
            {
                args.HideInfoWindow = true;
                if (ViewMediator.UploadPageOpen)
                {
                    ViewMediator.LastClickedPinInLocationselector = pin;
                    ViewMediator.ReportToAddTo = report;
                    //Console.WriteLine("Pin clicked!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!: " + pin.Report.documentId);

                }
                //MapUtil.MoveViewTo(pin.Location);
                //listener to display popup of tree description/details (fetched from DB)
                TreePopup popup = new TreePopup(report);
                popup.BindingContext = report;
                Application.Current.MainPage.ShowPopup(popup);
            };

            MainThread.BeginInvokeOnMainThread(() => {
                ViewMediator.Map.Pins.Add(pin);
            });

            if(ViewMediator.FirstMapLoad == false) PushNotifUtil.AddPushNotification(report);
        }

        /// <summary>
        /// Authored by Joe Dobbelaar
        /// 
        /// Removes a pin from the map view
        /// 
        /// </summary>
        /// <param name="reportId"></param>
        public static void RemovePin(string reportId) {
            foreach (CustomPin p in ViewMediator.Map.Pins) {
                if (p.Report.documentId.Equals(reportId)) {
                    MainThread.BeginInvokeOnMainThread(() => {
                        ViewMediator.Map.Pins.Remove(p);
                    });
                    return;
                }
            }
        }

        /// <summary>
        /// Authored by Jared Chan
        /// 
        /// Pushes newly created pin data to the DB
        /// 
        /// </summary>
        /// <param name="selectedLoc"></param>
        /// <param name="date"></param>
        /// <param name="imageFile"></param>
        /// <param name="imageByteArray"></param>
        /// <param name="treeName"></param>
        /// <param name="treeColor"></param>
        public static async void PushPinToDB(Location selectedLoc, 
            DateTime date, 
            FileResult imageFile, 
            byte[] imageByteArray, 
            string treeName,
            Color treeColor)
        {

            if (DBManager.currentUserManager.getIsBanned()) {
                return;
            }

            // Upload image to Firebase
            string uploadUrl = await DBManager.uploadImage(imageFile.FileName, imageByteArray);

            
            ReportManager newReport = new ReportManager();
            newReport.ThisReportColor = ReportColorFromHex(treeColor.ToHex()); 
            //newReport.setApprovalDate(1583025939);
            newReport.CreateDate = Timestamp.FromDateTime(date.ToUniversalTime());
            newReport.Location = new GeoPoint(selectedLoc.Latitude, selectedLoc.Longitude);

            ReportContribution contr = new ReportContribution(uploadUrl, imageFile.FileName, (DBManager.currentUser != null ? DBManager.currentUser.User.Uid : "Anonymous"), DBManager.currentUserManager != null ? DBManager.currentUserManager.getName() : "Anonymous");
            // This is the first contribution, so set rating to 1
            contr.Rating = 1;
            newReport.Contributions.Add(contr);

            newReport.Status = true;

            string treeId;
            if (string.IsNullOrEmpty(treeName))
            {
                treeName = "";
                treeId = "";
            }
            else treeId = GetTreeIdFromName(treeName);

            newReport.TreeName = treeName;
            newReport.TreeId = treeId;

            string newId = await newReport.push();

            bool userCanUpload = DBManager.currentUserManager.addContribution(newReport.documentId, uploadUrl);
            DBManager.currentUserManager.push();
        }

        /// <summary>
        /// Authored by Joe Dobbelaar 
        /// 
        /// Adds newly created pin data to an exisiting pin
        /// 
        /// </summary>
        /// <param name="report"></param>
        /// <param name="imageFile"></param>
        /// <param name="imageByteArray"></param>
        public static async void AddContributionToReport(ReportManager report, FileResult imageFile, byte[] imageByteArray)
        {
            // Upload image to Firebase
            string uploadUrl = await DBManager.uploadImage(imageFile.FileName, imageByteArray);

            ReportContribution contr = new ReportContribution(uploadUrl, imageFile.FileName, (DBManager.currentUser != null ? DBManager.currentUser.User.Uid : "Anonymous"), DBManager.currentUserManager != null ? DBManager.currentUserManager.getName() : "Anonymous");
            // This is not first contribution, so set rating to 0
            contr.Rating = 0;
            report.Contributions.Add(contr);

            report.push();
        }   

        /// <summary>
        /// Authored by Jared Chan
        /// 
        /// Converts Hexcode to a ReportColor object
        /// 
        /// </summary>
        /// <param name="hexcode"></param>
        /// <returns>ReportColor object</returns>
        private static ReportColor? ReportColorFromHex(string hexcode)
        {
            foreach(ColorManager colorManager in CultureUtil.Instance.ColorManagers)
            {
                if (colorManager.getColorHex().Trim().ToLower().Equals(hexcode.Trim().ToLower())) {
                    return new ReportColor(colorManager.getColorId(), colorManager.getColorHex(), colorManager.getOrder());
                }
            }
            return null;
        }

        /// <summary>
        /// Authored by Jared Chan
        /// 
        /// Converts a given tree name to its FireBase document ID
        /// 
        /// </summary>
        /// <param name="treeName"></param>
        /// <returns>the string document ID</returns>
        private static string GetTreeIdFromName(string treeName)
        {
            foreach(TreeManager tree in CultureUtil.Instance.TreeManagers)
            {
                if (treeName == tree.NameAutoTranslation) return tree.DocumentId;
            }
            return null;
        }

        /// <summary>
        /// Authored by Jared Chan (deprecated method)
        /// 
        /// Event handler for when reports are added to reportManagers
        /// Ideally, reports are continuously pulled from DB and added to reportManagers
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnReportManagersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //event handler
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                System.Diagnostics.Debug.WriteLine("ADDED PIN");
                //Add pin to map
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                //your code
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                //your code
            }
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                //your code
            }
        }

        /// <summary>
        /// Authored by Jared Chan
        /// 
        /// Fetches pin data from DB and adds them to map
        /// 
        /// </summary>
        public static async void LoadAllPinsFromDB()
        {
            await DatabaseObjectType.Report.fetchAll();
            reportManagers = new ObservableCollection<ReportManager>();

            foreach (ReportManager reportManager in DBManager.reportManagers.Values)
            {
                reportManagers.Add(reportManager);
            }
            reportManagers.CollectionChanged += new NotifyCollectionChangedEventHandler(OnReportManagersChanged);
        }

        /// <summary>
        /// Authored by Jared Chan (deprecated method)
        /// 
        /// Loads all reports from DBManager (without contacting database) and adds those pins to the map
        /// 
        /// </summary>
        public static void LoadAllPinsFromDBManager() {
            reportManagers = new ObservableCollection<ReportManager>();

            foreach (ReportManager reportManager in DBManager.reportManagers.Values) {
                reportManagers.Add(reportManager);
            }
            reportManagers.CollectionChanged += new NotifyCollectionChangedEventHandler(OnReportManagersChanged);
    }

        /// <summary>
        /// Authored by Joe Dobbelaar
        /// 
        /// Places all pins on the map from DB
        /// 
        /// </summary>
        public static void AddAllPinsFromDB()
        {
            DatabaseObjectType.Report.fetchAll();
        }

        /// <summary>
        /// Authored by Joe Dobbelaar
        /// 
        /// Custom pin handler class for changing the image associated with pins
        /// 
        /// </summary>
        public class CustomPin : Pin {

            public static readonly BindableProperty ReportProperty =
                BindableProperty.Create(nameof(Report), typeof(ReportManager), typeof(CustomPin),
                                        propertyChanged: OnReportChanged);

            public static readonly BindableProperty ImageSourceProperty =
                BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(CustomPin),
                                        propertyChanged: OnImageSourceChanged);

            public ReportManager Report
            {
                get => (ReportManager)GetValue(ReportProperty);
                set => SetValue(ReportProperty, value);
            }

            public ImageSource? ImageSource {
                get => (ImageSource?)GetValue(ImageSourceProperty);
                set => SetValue(ImageSourceProperty, value);
            }

            public Microsoft.Maui.Maps.IMap? Map { get; set; }

            static async void OnReportChanged(BindableObject bindable, object oldValue, object newValue)
            {
            }

            static async void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue) {
                var control = (CustomPin)bindable;
                if (control.Handler?.PlatformView is null) {
                    // Workaround for when this executes the Handler and PlatformView is null
                    control.HandlerChanged += OnHandlerChanged;
                    return;
                }

                #if IOS || MACCATALYST
		            await control.AddAnnotation();
                #else
                    await Task.CompletedTask;
                #endif

                void OnHandlerChanged(object? s, EventArgs e) {
                    OnImageSourceChanged(control, oldValue, newValue);
                    control.HandlerChanged -= OnHandlerChanged;
                }
            }
        }

        /// <summary>
        /// Custom map handler for changing the look of pins / map controls. 
        /// Inherits from Microsoft MapHandler
        /// </summary>
        public class CustomMapHandler : MapHandler {
            public static readonly IPropertyMapper<IMap, IMapHandler> CustomMapper =
                new PropertyMapper<IMap, IMapHandler>(Mapper) {
                    [nameof(IMap.Pins)] = MapPins,
                };

            public CustomMapHandler() : base(CustomMapper, CommandMapper) {
            }

            public CustomMapHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null) : base(
                mapper ?? CustomMapper, commandMapper ?? CommandMapper) {
            }

            /// <summary>
            /// Keep a list of all pins on the map
            /// </summary>
            public List<Marker>? Markers { get; private set; }

            /// <summary>
            /// Add this custom handler to a map
            /// </summary>
            /// <param name="platformView">Map for display</param>
            protected override void ConnectHandler(MapView platformView) {
                base.ConnectHandler(platformView);
                var mapReady = new MapCallbackHandler(this);
                PlatformView.GetMapAsync(mapReady);
            }

            /// <summary>
            /// Method for handling add / removing map pins
            /// </summary>
            private static new void MapPins(IMapHandler handler, IMap map) {
                if (handler is CustomMapHandler mapHandler) {
                    if (mapHandler.Markers != null) {
                        foreach (var marker in mapHandler.Markers) {
                            marker.Remove();
                        }

                        mapHandler.Markers = null;
                    }

                    mapHandler.AddPins(map.Pins);
                }
            }

            /// <summary>
            /// Add all pins to map
            /// </summary>
            /// <param name="mapPins">List of pins to add</param>
            private void AddPins(IEnumerable<IMapPin> mapPins) {
                if (Map is null || MauiContext is null) {
                    return;
                }

                Markers ??= new List<Marker>();
                foreach (var pin in mapPins) {
                    var pinHandler = pin.ToHandler(MauiContext);
                    if (pinHandler is IMapPinHandler mapPinHandler) {
                        var markerOption = mapPinHandler.PlatformView;
                        if (pin is CustomPin cp) {
                            cp.ImageSource.LoadImage(MauiContext, result =>
                            {
                                if (result?.Value is BitmapDrawable bitmapDrawable) {
                                    markerOption.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmapDrawable.Bitmap));
                                }

                                AddMarker(Map, pin, Markers, markerOption);
                            });
                        } else {
                            AddMarker(Map, pin, Markers, markerOption);
                        }
                    }
                }
            }

            /// <summary>
            /// Add visible pin to map
            /// </summary>
            /// <param name="map"><Current map/param>
            /// <param name="pin">Pin data to put in marker</param>
            /// <param name="markers">Current list of markers</param>
            /// <param name="markerOption">Options for marker</param>
            private static void AddMarker(GoogleMap map, IMapPin pin, List<Marker> markers, MarkerOptions markerOption) {
                var marker = map.AddMarker(markerOption);
                pin.MarkerId = marker.Id;
                markers.Add(marker);
            }
        }

        class MapCallbackHandler : Java.Lang.Object, IOnMapReadyCallback {
            private readonly IMapHandler mapHandler;

            public MapCallbackHandler(IMapHandler mapHandler) {
                this.mapHandler = mapHandler;
            }

            public void OnMapReady(GoogleMap googleMap) {
                mapHandler.UpdateValue(nameof(IMap.Pins));
                googleMap.UiSettings.ZoomControlsEnabled = false;
                googleMap.UiSettings.MyLocationButtonEnabled = false;
            }
        }
    }
}
