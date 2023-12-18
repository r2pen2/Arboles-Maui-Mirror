using Google.Cloud.Firestore;

namespace ArbolesMAUI.DB.ObjectManagers
{
    public class ZoneManager : ObjectManager {

        // Data for this specific Zone
        string imageMap;
        string imageReg;
        string nameEN;
        string nameES;
        int order;
        string regionId;

        /// <summary>
        /// ObjectManager for Zone
        /// </summary>
        /// <param name="_documentId">
        /// Firebase documentID of Zone
        /// </param>
        public ZoneManager(string? _documentId = null, DocumentSnapshot? _docSnap = null) : base(DatabaseObjectType.Zone) {
            this.documentId = _documentId;
            this.docSnap = _docSnap;
            this.docRef = _documentId != null ? this.objectType.getCollectionReference().Document(_documentId) : null;
            if (_docSnap != null) {
                _ = this.fetchData();
            }
        }

        /// <summary>
        /// Fetches all data for this ZoneManagaer
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
            this.imageMap = docSnap.GetValue<string>("imageMap");
            this.imageReg = docSnap.GetValue<string>("imageReg");
            this.nameEN = docSnap.GetValue<string>("nameEN");
            this.nameES = docSnap.GetValue<string>("nameES");
            this.order = docSnap.GetValue<int>("order");
            this.regionId = docSnap.GetValue<string>("regionId");
            DBManager.zoneManagers[this.documentId] = this;
            return true;
        }

        /// <summary>
        /// Get dictionary representation of this ZoneManager
        /// </summary>
        /// <returns>Dictionary containing ZoneManager data</returns>
        public override Dictionary<string, object> toDictionary() {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "imageMap", this.imageMap },
                { "imageReg", this.imageReg },
                { "nameEN", this.nameEN },
                { "nameES", this.nameES },
                { "order", this.order },
                { "regionId", this.regionId },
            };
            return data;
        }

        public string getImageMap() {
            return this.imageMap;
        }

        public void setImageMap(string newImageMap) {
            this.imageMap = newImageMap;
        }

        public string getImageReg() {
            return this.imageReg;
        }

        public void setImageReg(string newImageReg) {
            this.imageReg = newImageReg;
        }

        public string getNameEN() {
            return this.nameEN;
        }

        public void setNameEN(string newNameEN) {
            this.nameEN = newNameEN;
        }

        public string getNameES() {
            return this.nameES;
        }

        public void setNameES(string newNameES) {
            this.nameES = newNameES;
        }

        public int getOrder() {
            return this.order;
        }

        public void setOrder(int newOrder) {
            this.order = newOrder;
        }

        public string getRegionId() {
            return this.regionId;
        }

        public void setRegionId(string newRegionId) {
            this.regionId = newRegionId;
        }
    }
}
