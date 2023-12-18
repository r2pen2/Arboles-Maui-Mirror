using ArbolesMAUI.ViewModels;
using Google.Cloud.Firestore;
using Color = Microsoft.Maui.Graphics.Color;

namespace ArbolesMAUI.DB.ObjectManagers
{
    public class ColorManager : ObjectManager {

        // Data for this specific Color
        int order;
        string colorHex;
        string colorId;
        public Color MauiColor { get; set; }
        public List<TreeManager> FilteredTreeManagers { get; set; }

        /// <summary>
        /// ObjectManager for Color
        /// </summary>
        /// <param name="_documentId">
        /// Firebase documentID of Color
        /// </param>
        public ColorManager(string? _documentId = null, DocumentSnapshot? _docSnap = null) : base(DatabaseObjectType.Color) {
            this.documentId = _documentId;
            this.docSnap = _docSnap;
            this.docRef = _documentId != null ? this.objectType.getCollectionReference().Document(_documentId) : null;
            if (_docSnap != null) {
                _ = this.fetchData();
            }
        }

        /// <summary>
        /// Fetches all data for this ColorManager
        /// </summary>
        /// <returns>
        /// True if fetch successful, false if not
        /// </returns>
        public async Task<bool> fetchData() {
            if (this.docSnap == null) {
                this.docSnap = await this.getDocumentSnapshot();
            }
            if (!docSnap.Exists) {
                // Invalid document reference
                return false;
            }
            // Document exists! Harvest data
            this.colorId = docSnap.GetValue<string>("colorId");
            this.order = docSnap.GetValue<int>("order");
            this.colorHex = docSnap.GetValue<string>("colorHex");
            InitBindableProperties();
            DBManager.colorManagers[this.documentId] = this;
            return true;
        }

        /// <summary>
        /// Get dictionary representation of this ColorManager
        /// </summary>
        /// <returns>Dictionary containing ColorManager data</returns>
        public override Dictionary<string, object> toDictionary() {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "colorId", this.colorId },
                { "colorHex", this.colorHex },
                { "order", this.order },
            };
            return data;
        }

        public void InitBindableProperties()
        {
            MauiColor = Color.FromArgb(this.colorHex);
            LoadListViewTrees();
        }

        /// <summary>
        /// Fetches and sets the ItemSource for treeView based on the colorID of the currently selected color
        /// </summary>
        private async void LoadListViewTrees()
        {
            List<TreeManager> filteredTrees = new List<TreeManager>();
            foreach (TreeManager treeManager in CultureUtil.Instance.TreeManagers)
            {
                if (treeManager.ColorIds.Contains(this.getColorId()))
                {
                    TreeManager treeManagerClone = treeManager.ShallowClone();
                    treeManagerClone.PaletteGroupHex = this.getColorHex();
                    filteredTrees.Add(treeManagerClone);
                }
            }
            FilteredTreeManagers = filteredTrees.OrderBy(tree => tree.NameAutoTranslation).ToList();
        }

        public string getColorId() {
            return this.colorId;
        }

        public void setColorId(string _colorId) {
            this.colorId = _colorId;
        }
        
        public int getOrder() {
            return this.order;
        }

        public void setOrder(int _order) {
            this.order = _order;
        }

        public string getColorHex() {
            return this.colorHex;
        }
        
        public void setColorHex(string _colorHex) {
            this.colorHex = _colorHex;
        }
    }
}
