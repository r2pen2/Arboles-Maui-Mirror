using ArbolesMAUI.DB;
using ArbolesMAUI.DB.ObjectManagers;

/**
 * Authored by Jared Chan
 **/

namespace ArbolesMAUI.ViewModels
{
    /// <summary>
    /// Utility class that stores logic for loading trees and colors on Culture Page
    /// </summary>
    public class CultureUtil
    {
        private CultureUtil() { }

        private static CultureUtil instance = null;

        private static List<ColorManager> colorManagers = null;

        private static List<TreeManager> treeManagers = null;

        public static CultureUtil Instance
        {
            get
            {
                if (instance == null) instance = new CultureUtil();
                return instance;
            }
        }

        public List<ColorManager> ColorManagers
        {
            get 
            { 
                if(colorManagers == null) colorManagers = new List<ColorManager>();
                return colorManagers; 
            }
            set { colorManagers = value; }
        }

        public List<TreeManager> TreeManagers
        {
            get
            {
                if (treeManagers == null) treeManagers = new List<TreeManager>();
                return treeManagers;
            }
            set { treeManagers = value; }
        }

        /// <summary>
        /// Loads all color objects from the DB and puts them into a list 
        /// </summary>
        public static async void LoadColorsFromDB()
        {
            await DatabaseObjectType.Color.fetchAll();
            List<string> colorIDList = DBManager.colorManagers.Keys.ToList();
            List <ColorManager> colorList = new List<ColorManager>();

            foreach (string colorID in colorIDList)
            {
                ColorManager color = DBManager.getColorManager(colorID);
                await color.fetchData();
                colorList.Add(color);
            }
            Instance.ColorManagers = colorList;
        }

        /// <summary>
        /// Loads all tree objects from the DB and puts them into a list 
        /// </summary>
        public static async void LoadTreesFromDB()
        {
            await DatabaseObjectType.Tree.fetchAll();
            List<string> treeIDList = DBManager.treeManagers.Keys.ToList();
            List <TreeManager> treeList = new List<TreeManager>();

            foreach (string treeID in treeIDList)
            {
                TreeManager tree = DBManager.getTreeManager(treeID);
                await tree.fetchData();
                treeList.Add(tree);
            }
            Instance.TreeManagers = treeList;
        }

    }
}
