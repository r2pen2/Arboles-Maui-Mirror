using Android.Net.Wifi.Aware;
using ArbolesMAUI.DB.ObjectManagers;
using ArbolesMAUI.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using System.Globalization;
using ArbolesMAUI.ViewModels;
using Map = Microsoft.Maui.Controls.Maps.Map;
using System.ComponentModel;

/**
 * Authored by Jared Chan, Joe Dobbelaar, and Cole Parks
**/


namespace ArbolesMAUI.ViewModels
{
    // This class uses Mediator Design Pattern to allow decoupled communication between views
    // ViewMediator is basically a messenger; it receives messages from views and sends them as well
    public class ViewMediator
    {
        /// <summary>
        /// indicates whether upload page is open or not
        /// </summary>
        private static bool uploadPageOpen = false;

        private static CultureInfo culture;

        /// <summary>
        /// Stores reference to AppShell tab bar
        /// </summary>
        public static TabBar TabBar { get; set; }

        //Culture Page getters/setters

        /// <summary>
        /// Reference to identify tree species button
        /// </summary>
        public static Button SelectSpeciesButton { get; set; }
        
        /// <summary>
        /// Reference to frame that displays selected color/color of tree species
        /// </summary>
        public static Frame SpeciesColorBlock { get; set; }

        /// <summary>
        /// culture page object reference
        /// </summary>
        public static CulturePage CulturePage { get; set; }

        public static void SetSpeciesColorBlockToPin()
        {
            if (ReportToAddTo != null && SpeciesColorBlock != null)
            {
                SpeciesColorBlock.BackgroundColor = ReportToAddTo.MauiColor;
            }
            else if (SpeciesColorBlock != null)
            {
                SpeciesColorBlock.BackgroundColor = Color.FromRgba("00000000");
            }
        }

        //Filter Page getters/setters

        /// <summary>
        /// Indicates whether filter page in user view or not
        /// </summary>
        public static bool IsFilterPageOpen { get; set; } = false;

        /// <summary>
        /// Reference to color frame that displays selected color of tree species
        /// </summary>
        public static Frame FilterColorBlock { get; set; }

        /// <summary>
        /// Reference to filter species name button
        /// </summary>
        public static Button FilterSpeciesButton { get; set; }

        /// <summary>
        /// Indicates whether the user is opting to identify a tree by its name or only its color 
        /// 
        /// Used string options:
        ///     "By color"
        ///     "By species"
        /// </summary>
        public static string MethodOfIdentification { get; set; } = "";


        public static bool FirstMapLoad { get; set; } = true;

        public static double MapZoomLevel { get; set; } = 0.5;

        public static bool UploadPageOpen { get; set; }

        public static Location UserLocationFocus{ get; set; }

        public static Location UserSelectedLocation { get; set; }

        public static Pin LastClickedPinInLocationselector { get; set; }
        
        public static event EventHandler chosePinAsLocation;

        public static void SetLocationAsPin()
        {
            chosePinAsLocation?.Invoke(null, EventArgs.Empty);
        }
        
        public static ReportManager ReportToAddTo { get; set; }   
        //public Pin LastClickedPinInLocationselector { get; set; } = null;

        public static bool UsePinAsLocation { get; set; } = false; 
        
        public static Button LocationPickerButton { get; set; }

        /// <summary>
        /// Gets/Sets the stored map instance
        /// </summary>
        /// <returns>Null or a Map instance</returns>
        private static Map map = new Map() {
            MapType = Microsoft.Maui.Maps.MapType.Hybrid,
            IsShowingUser = true,
            IsTrafficEnabled = false
        };

        /// <summary>
        /// Stores reference to map view from map page
        /// </summary>
        public static Map Map { get { return map; } }
        private ViewMediator() {
        }

        /// <summary>
        /// Sets the currently visible page of the tab bar from within the AppShell
        /// </summary>
        /// <param name="page"></param>
        public static void SetTabBarCurrentItem(ContentPage page) {
            TabBar.IsVisible = true;
            ShellContent shellContent = new ShellContent();
            shellContent.Content = page;
            TabBar.CurrentItem = shellContent;
        }


    }
}
