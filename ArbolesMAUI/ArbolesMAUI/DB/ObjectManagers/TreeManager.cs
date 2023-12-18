using ArbolesMAUI.ViewModels;
using Google.Cloud.Firestore;
using MauiLocalization.Resources.Localization;

namespace ArbolesMAUI.DB.ObjectManagers
{
    public class TreeManager : ObjectManager {

        public class TreeImage {
            public string Image { get; set; }
            public string ImageId { get; set; }
            public string ImageUrl { get; set; }
            public bool IsFlower { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }

            /// <summary>
            /// Object for storing MFTree Image Data
            /// </summary>
            /// <param name="image">Path to image</param>
            /// <param name="imageId">Id of image</param>
            /// <param name="isFlower">Whether or not this image is a flower</param>
            /// <param name="name">Name of image</param>
            /// <param name="order">Image order</param>
            public TreeImage(string image, string imageId, bool isFlower, string name, int order) {
                Image = image;
                ImageId = imageId;
                IsFlower = isFlower;
                Name = name;
                Order = order;
                FetchImageUrl();
            }

            private async void FetchImageUrl()
            {
                ImageUrl = await DBManager.downloadImage("Trees", this.Image);
            }
        }

        // Data for this specific MFTree
        List<string> colorIds;
        string dispersorEN;
        string dispersorES;
        string floweringDays;
        List<int> floweringWeeks;
        string groupId;
        List<TreeImage> images;
        string nameEN;
        string nameES;
        string originEN;
        string originES;
        List<string> regions;
        string scientificName;
        string sizeBottom;
        string sizeHeight;
        string sizeWidth;
        string sourceEN;
        string sourceES;
        string sponsorLogo;
        string sponsorName;
        string statusEN;
        string statusES;

        public List<string> ColorIds { get; set; }
        public string DispersorEN { get; set; }
        public string DispersorES { get; set; }
        public string FloweringDays { get; set; }
        public List<int> FloweringWeeks { get; set; }
        public string GroupId { get; set; }
        public List<TreeImage> Images { get; set; }
        public List<string> ImageUrls { get; set; }
        public string Thumbnail { get; set; }
        public string NameEN { get; set; }
        public string NameES { get; set; }
        public string OriginEN { get; set; }
        public string OriginES { get; set; }
        public List<string> Regions { get; set; }
        public string ScientificName { get; set; }
        public string SizeBottom { get; set; }
        public string SizeHeight { get; set; }
        public string SizeWidth { get; set; }
        public string SourceEN { get; set; }
        public string SourceES { get; set; }
        public string SponsorLogo { get; set; }
        public string SponsorName { get; set; }
        public string StatusEN { get; set; }
        public string StatusES { get; set; }

        public string NameAutoTranslation { get; set; }
        public string OriginAutoTranslation { get; set; }
        public string SourceAutoTranslation { get; set; }
        public string StatusAutoTranslation { get; set; }
        public bool IsSelectionAvailable { get; set; } = false;
        public string PaletteGroupHex { get; set; } = "";

        public string DocumentId
        {
            get
            {
                return this.documentId;
            }
        }

        /// <summary>
        /// ObjectManager for MFTree
        /// </summary>
        /// <param name="_documentId">
        /// Firebase documentID of MFTree
        /// </param>
        public TreeManager(string? _documentId = null, DocumentSnapshot? _docSnap = null) : base(DatabaseObjectType.Tree) {
            this.documentId = _documentId;
            this.docSnap = _docSnap;
            this.docRef = _documentId != null ? this.objectType.getCollectionReference().Document(_documentId) : null;
            if (_docSnap != null) {
                _ = this.fetchData();
            }
        }

        /// <summary>
        /// Fetches all data for this MFTreeManagaer
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
            this.colorIds = docSnap.GetValue<List<string>>("colorIds");
            this.dispersorEN = docSnap.GetValue<string>("dispersorEN");
            this.dispersorES = docSnap.GetValue<string>("dispersorES");
            this.floweringWeeks = docSnap.GetValue<List<int>>("floweringWeeks");
            this.floweringDays = docSnap.GetValue<string>("floweringDays");
            this.groupId = docSnap.GetValue<string>("groupId");
            this.nameEN = docSnap.GetValue<string>("nameEN");
            this.nameES = docSnap.GetValue<string>("nameES");
            this.originEN = docSnap.GetValue<string>("originEN");
            this.originES = docSnap.GetValue<string>("originES");
            this.regions = docSnap.GetValue<List<string>>("regions");
            this.scientificName = docSnap.GetValue<string>("scientificName");
            this.sizeBottom = docSnap.GetValue<string>("sizeBottom");
            this.sizeHeight = docSnap.GetValue<string>("sizeHeight");
            this.sizeWidth = docSnap.GetValue<string>("sizeWidth");
            this.sourceEN = docSnap.GetValue<string>("sourceEN");
            this.sourceES = docSnap.GetValue<string>("sourceES");
            this.sponsorLogo = docSnap.GetValue<string>("sponsorLogo");
            this.sponsorName = docSnap.GetValue<string>("sponsorName");
            this.statusEN = docSnap.GetValue<string>("statusEN");
            this.statusES = docSnap.GetValue<string>("statusES");

            // Make image objects
            List<Dictionary<string, Object>> dbImagesRaw = docSnap.GetValue<List<Dictionary<string, Object>>>("images");
            List<TreeImage> newImages = new List<TreeImage>();
            foreach (Dictionary<string, Object> rawImage in dbImagesRaw) {
                string imagePath = (string)rawImage["image"];
                string imageId = (string)rawImage["imageId"];
                bool isFlower = (bool)rawImage["isFlower"];
                string name = (string)rawImage["name"];
                int imgOrder = Convert.ToInt32(rawImage["order"]); // "order" is a boxed int! Must use Convert.ToInt32() instead of (int) to avoid an invalid cast
                TreeImage img = new TreeImage(imagePath, imageId, isFlower, name, imgOrder);
                newImages.Add(img);
            }
            images = newImages;

            InitBindableProperties();

            DBManager.treeManagers[this.documentId] = this;

            return true;
        }

        /// <summary>
        /// Get dictionary representation of this TreeManager
        /// </summary>
        /// <returns>Dictionary containing TreeManager data</returns>
        public override Dictionary<string, object> toDictionary() {

            // Turn TreeImages into Dictionaries
            List<Dictionary<string, object>> imageList = new List<Dictionary<string, object>>();
            foreach (TreeImage treeImg in images) {
                Dictionary<string, object> imageData = new Dictionary<string, object>() {
                    { "image", treeImg.Image },
                    { "imageId", treeImg.ImageId },
                    { "isFlower", treeImg.IsFlower },
                    { "name", treeImg.Name },
                    { "order", treeImg.Order }
                };
                imageList.Add(imageData);
            }

            // Create dictionary for this ObjectManager
            Dictionary<string, object> data = new Dictionary<string, object>() {
               { "colorIds", this.colorIds },
                { "dispersorEN", this.dispersorEN },
                { "dispersorES", this.dispersorES },
                { "floweringDays", this.floweringDays },
                { "floweringWeeks", this.floweringWeeks },
                { "groupId", this.groupId },
                { "images", imageList },
                { "nameEN", this.nameEN },
                { "nameES", this.nameES },
                { "originEN", this.originEN },
                { "originES", this.originES },
                { "regions", this.regions },
                { "scientificName", this.scientificName },
                { "sizeBottom", this.sizeBottom },
                { "sizeHeight", this.sizeHeight },
                { "sizeWidth", this.sizeWidth },
                { "sourceEN", this.sourceEN },
                { "sourceES", this.sourceES },
                { "sponsorLogo", this.sponsorLogo },
                { "sponsorName", this.sponsorName },
                { "statusEN", this.statusEN },
                { "statusES", this.statusES },

            };
            return data;
        }

        public async void InitBindableProperties()
        {
            ColorIds = colorIds;
            DispersorEN = dispersorEN;
            DispersorES = dispersorES;
            FloweringDays = floweringDays;
            FloweringWeeks = floweringWeeks;
            GroupId = groupId;
            Images = images;
            NameEN = nameEN;
            NameES = nameES;
            OriginEN = originEN;
            OriginES = originES;
            Regions = regions;
            ScientificName = scientificName;
            SizeBottom = sizeBottom;
            SizeHeight = sizeHeight;
            SizeWidth = sizeWidth;
            SourceEN = sourceEN;
            SourceES = sourceES;
            SponsorLogo = sponsorLogo;
            SponsorName = sponsorName;
            StatusEN = statusEN;
            StatusES = statusES;

            //Initialize language dependent fields 
            SetCultureBasedFields(); 

            //Handle culture change after this treemanager is intitialized
            LocalizationResourceManager.Instance.PropertyChanged += OnCultureChanged;

            await FetchImagesUrls();   
        }

        /// <summary>
        /// Converts TreeImage objects to string URLs for this TreeManager
        /// Also sets the thumbnail image for this TreeManager
        /// </summary>
        /// <returns></returns>
        private async Task FetchImagesUrls() {
            List<string> imageUrls = new List<string>();
            bool hasOrder1 = false;

            foreach(TreeImage treeImage in this.images)
            {
                string url = await DBManager.downloadImage("Trees", treeImage.Image);
                imageUrls.Add(url);

                if (treeImage.Order == 1)
                {
                    Thumbnail = url;
                    hasOrder1 = true;
                }
            }
            ImageUrls = imageUrls;
            if (hasOrder1 == false) Thumbnail = ImageUrls.First(); 
        }


        public void addColorId(string newColorId) {
            ColorIds.Add(newColorId);
        }

        public void removeColorId(string id) {
            ColorIds.Remove(id);
        }

        public void addFloweringWeek(int newFloweringWeek) {
            FloweringWeeks.Add(newFloweringWeek);
        }

        public void removeFlowerinWeek(int floweringWeek) {
            FloweringWeeks.Remove(floweringWeek);
        }

        public void addImage(TreeImage newImage) {
            Images.Add(newImage);
        }

        public void removeImage(TreeImage image) {
            Images.Remove(image);
        }

        public void addRegion(string newRegion) {
            Regions.Add(newRegion);
        }

        public void removeRegion(string region) {
            Regions.Remove(region);
        }

        public TreeManager ShallowClone()
        {
            TreeManager newTreeManager = (TreeManager)this.MemberwiseClone();
            newTreeManager.PaletteGroupHex = new string(this.PaletteGroupHex);
            return newTreeManager;
        }

        /// <summary>
        /// Event handler for culture changing via LocalizationResourceManager
        /// </summary>
        private void OnCultureChanged(object sender, EventArgs e)
        {
            SetCultureBasedFields();
        }

        private void SetCultureBasedFields()
        {
            if (LocalizationResourceManager.Instance.Culture.Name.ToLower() == "en-us")
            {
                NameAutoTranslation = this.nameEN;
                OriginAutoTranslation = this.originEN;
                SourceAutoTranslation = this.sourceEN;
                StatusAutoTranslation = this.statusEN;
            }
            else
            {
                NameAutoTranslation = this.nameES;
                OriginAutoTranslation = this.originES;
                SourceAutoTranslation = this.sourceES;
                StatusAutoTranslation = this.statusES;
            }
        }
    }
}
