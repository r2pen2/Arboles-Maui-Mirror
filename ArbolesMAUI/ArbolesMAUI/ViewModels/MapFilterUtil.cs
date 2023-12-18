using ArbolesMAUI.DB.ObjectManagers;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.HotReload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArbolesMAUI.ViewModels.MapUtil;

/**
 * Authored by Jared Chan
 **/

namespace ArbolesMAUI.ViewModels
{
    /// <summary>
    /// Utility class to store filtering logic for map page
    /// </summary>
    public static class MapFilterUtil
    {
        /// <summary>
        /// Filters the pins on the map by the given color (Microsoft.Maui.Graphics.Color)
        /// </summary>
        /// <param name="color"></param>
        public static void FilterByColor(Color color)
        {
            ViewMediator.Map.Pins.Clear();
            foreach (ReportManager report in MapUtil.ReportManagers) 
                if (report.MauiColor.ToHex() == color.ToHex()) MapUtil.AddNewPin(report);
        }

        /// <summary>
        /// Filters the pins on the map by the given tree name (Microsoft.Maui.Graphics.Color)
        /// </summary>
        /// <param name="color"></param>
        public static void FilterByName(string name)
        {
            ViewMediator.Map.Pins.Clear();
            foreach (ReportManager report in MapUtil.ReportManagers)
                if (report.TreeName.Trim().ToLower() == name.Trim().ToLower()) MapUtil.AddNewPin(report);
        }

        /// <summary>
        /// Re-adds all the pins to the map (for refreshing the map view when filters are applied)
        /// </summary>
        public static void ReloadAllMapPins()
        {
            foreach (ReportManager report in ReportManagers) AddNewPin(report);
        }
    }
}
