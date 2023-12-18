using ArbolesMAUI.DB.ObjectManagers;
using ArbolesMAUI.Model;
using MauiLocalization.Resources.Localization;
using System.Collections.ObjectModel;
using System.Windows.Input;

/**
 * Authored by Jared Chan
 **/

namespace ArbolesMAUI.ViewModels
{
    /// <summary>
    /// Viewmodel for Culture Page
    /// 
    /// Provides binding context w/ grouping data and commands for page elements to use
    /// </summary>
    public class CultureViewModel
    {
        /// <summary>
        /// Collection of tree color groups used as collection view source
        /// </summary>
        public ObservableCollection<TreeColorGroup> TreeColorGroups { get; set; } = new ObservableCollection<TreeColorGroup>();

        public CultureViewModel()
        {
            LoadTreeColorGroups();
        }

        /// <summary>
        /// Populates the TreeColorGroups collection with data and sorts the collection by color order
        /// </summary>
        private async void LoadTreeColorGroups()
        {
            foreach (ColorManager color in CultureUtil.Instance.ColorManagers)
            {
                TreeColorGroups.Add(new TreeColorGroup(color, new ObservableCollection<TreeManager>()));
            }
            TreeColorGroups = new ObservableCollection<TreeColorGroup>(TreeColorGroups.OrderBy(group => group.Order));
        }

        /// <summary>
        /// Adds/removes TreeColorGroups based on which color group header is selected from color palette
        /// </summary>
        public ICommand AddOrRemoveGroupDataCommand => new Command<TreeColorGroup>((item) =>
        {
            if (ViewMediator.MethodOfIdentification != "By color")
            {
                if (item.Count > 0)
                {
                    foreach (TreeColorGroup group in TreeColorGroups)
                    {
                        group.Clear();
                    }
                }
                else if (item.Count == 0)
                {
                    foreach (TreeColorGroup group in TreeColorGroups)
                    {
                        if (group.Name == item.Name)
                        {
                            foreach (TreeManager tree in group.Trees)
                            {
                                item.Add(tree);
                            }
                        }
                        else group.Clear();
                    }
                }
            }
        });

        /// <summary>
        /// Adds individual Trees grouped under their respective color group header based on non-scientific name search
        /// 
        /// Receives a string (searched tree name) as a command parameter
        /// Compares this string to tree names in each tree color group
        /// Populates tree color groups with tree data that matches
        /// </summary>
        public ICommand SearchForTreesCommand => new Command<string>((e) =>
        {
            foreach (TreeColorGroup group in TreeColorGroups)
            {
                if (string.IsNullOrWhiteSpace(e)) group.Clear();
                else
                {
                    foreach (TreeManager tree in group.Trees)
                    {
                        if (!group.Contains(tree) && tree.NameAutoTranslation.ToLower().Contains(e.ToLower())) group.Add(tree);
                        else if (group.Contains(tree) && !tree.NameAutoTranslation.ToLower().Contains(e.ToLower())) group.Remove(tree);
                    }
                }
            }
        });
        
        /// <summary>
        /// Clears the tree list data for each tree color group 
        /// </summary>
        public ICommand ClearItemsCommand => new Command(() => 
        {
            foreach (TreeColorGroup group in TreeColorGroups)
            {
                group.Clear();
            }
        });
    }
}
