using Google.Cloud.Firestore;

namespace ArbolesMAUI.DB.ObjectManagers
{
    public class FloweringCultureManager : ObjectManager {

        // Data for this specific FloweringCulture
        string detailsEN;
        string detailsES;
        string floweringId;
        string image;
        string titleEN;
        string titleES;
        int order;

        /// <summary>
        /// ObjectManager for FloweringCulture
        /// </summary>
        /// <param name="_documentId">
        /// Firebase documentID of FloweringCulture
        /// </param>
        public FloweringCultureManager(string? _documentId = null, DocumentSnapshot? _docSnap = null) : base(DatabaseObjectType.FloweringCulture) {
            this.documentId = _documentId;
            this.docSnap = _docSnap;
            this.docRef = _documentId != null ? this.objectType.getCollectionReference().Document(_documentId) : null;
            if (_docSnap != null) {
                _ = this.fetchData();
            }
        }

        /// <summary>
        /// Fetches all data for this FloweringCultureManagaer
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
            this.detailsEN = docSnap.GetValue<string>("detailsEN");
            this.detailsES = docSnap.GetValue<string>("detailsES");
            this.floweringId = docSnap.GetValue<string>("floweringId");
            this.image = docSnap.GetValue<string>("image");
            this.titleEN = docSnap.GetValue<string>("titleEN");
            this.titleES = docSnap.GetValue<string>("titleES");
            this.order = docSnap.GetValue<int>("order");
            DBManager.floweringCultureManagers[this.documentId] = this;
            return true;
        }

        /// <summary>
        /// Get dictionary representation of this FloweringCultureManager
        /// </summary>
        /// <returns>Dictionary containing FloweringCultureManager data</returns>
        public override Dictionary<string, object> toDictionary() {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "detailsEN", this.detailsEN },
                { "detailsES", this.detailsES },
                { "floweringId", this.floweringId },
                { "image", this.image },
                { "titleEN", this.titleEN },
                { "titleES", this.titleES },
                { "order", this.order },
            };
            return data;
        }
        
        public string getDetailsEN() {
            return this.detailsEN;
        }

        public void setDetailsEN(string newDetailsEN) {
            this.detailsEN = newDetailsEN;
        }

        public string getDetailsES() {
            return this.detailsES;
        }

        public void setDetailsES(string newDetailsES) {
            this.detailsES = newDetailsES;
        }

        public string getFloweringID() {
            return this.floweringId;
        }

        public void setFloweringId(string newFloweringId) {
            this.floweringId = newFloweringId;
        }

        public string getImage() {
            return this.image;
        }

        public void setImage(string newImage) {
            this.image= newImage;
        }

        public string getTitleEN() {
            return this.titleEN;
        }

        public void setTitleEN(string newTitleEN) {
            this.titleEN = newTitleEN;
        }

        public string getTitleES() {
            return this.titleES;
        }
        
        public void setTitleES(string newTitleES) {
            this.titleES = newTitleES;
        }

        public int getOrder() {
            return this.order;
        }

        public void setOrder(int newOrder) {
            this.order = newOrder;
        }
    }
}
