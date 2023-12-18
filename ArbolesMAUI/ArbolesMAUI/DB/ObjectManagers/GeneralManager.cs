using Google.Cloud.Firestore;

namespace ArbolesMAUI.DB.ObjectManagers
{
    public class GeneralManager : ObjectManager {

        // Data for this specific General
        string stringKey;
        string textEN;
        string textES;

        /// <summary>
        /// ObjectManager for General
        /// </summary>
        /// <param name="_documentId">
        /// Firebase documentID of General
        /// </param>
        public GeneralManager(string? _documentId = null, DocumentSnapshot? _docSnap = null) : base(DatabaseObjectType.General) {
            this.documentId = _documentId;
            this.docSnap = _docSnap;
            this.docRef = _documentId != null ? this.objectType.getCollectionReference().Document(_documentId) : null;
            if (_docSnap != null) {
                _ = this.fetchData();
            }
        }

        /// <summary>
        /// Fetches all data for this GeneralManagaer
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
            this.stringKey = docSnap.GetValue<string>("stringKey");
            this.textEN = docSnap.GetValue<string>("textEN");
            this.textES = docSnap.GetValue<string>("textES");
            DBManager.generalManagers[this.documentId] = this;
            return true;
        }

        /// <summary>
        /// Get dictionary representation of this GeneralManager
        /// </summary>
        /// <returns>Dictionary containing GeneralManager data</returns>
        public override Dictionary<string, object> toDictionary() {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "stringKey", this.stringKey },
                { "textEN", this.textEN },
                { "textES", this.textES },
            };
            return data;
        }

        public string getStringKey() {
            return this.stringKey;
        }

        public void setStringKey(string newStringKey) {
            this.stringKey = newStringKey;
        }

        public string getTextEN() {
            return this.textEN;
        }

        public void setTextEN(string newTextEN) {
            this.textEN = newTextEN;
        }

        public string getTextES() {
            return this.textES;
        }

        public void setTextES(string newTextES) {
            this.textES = newTextES;
        }
    }
}
