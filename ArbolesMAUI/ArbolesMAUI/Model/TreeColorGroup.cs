using ArbolesMAUI.DB.ObjectManagers;
using System.Collections.ObjectModel;
using Color = Microsoft.Maui.Graphics.Color;

/**
 * Authored by Jared Chan
 **/

namespace ArbolesMAUI.Model
{
    /// <summary>
    /// Class used for grouping trees by a specific color 
    /// Developed for the culture/color palette page
    /// </summary>
    public class TreeColorGroup : ObservableCollection<TreeManager>
    {
        /// <summary>
        /// Name to identify the group by; this should be a color ID
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Stores the Microsoft.Maui.Graphics color type
        /// </summary>
        public Color MauiColor { get; set; }

        /// <summary>
        /// The order in which this color group appears on the color palette
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Master list copy of all trees associated with this color group
        /// </summary>
        public ObservableCollection<TreeManager> Trees { get; set; }

        public TreeColorGroup(ColorManager colorManager, ObservableCollection<TreeManager> trees) : base(trees)
        {
            Name = colorManager.getColorId();
            MauiColor = colorManager.MauiColor;
            Order = colorManager.getOrder();
            Trees = new ObservableCollection<TreeManager>(colorManager.FilteredTreeManagers);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

