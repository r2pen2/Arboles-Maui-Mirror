using Google.Cloud.Firestore;

namespace ArbolesMAUI.DB.ObjectManagers
{
    public class ConfigurationManager : ObjectManager {

        // Data for this specific Configuration
        string aboutEN;
        string aboutES;

        /// <summary>
        /// ObjectManager for Configuration
        /// </summary>
        /// <param name="_documentId">
        /// Firebase documentID of Configuration
        /// </param>
        public ConfigurationManager(string? _documentId = null, DocumentSnapshot? _docSnap = null) : base(DatabaseObjectType.Configuration) {
            this.documentId = _documentId;
            this.docSnap = _docSnap;
            this.docRef = _documentId != null ? this.objectType.getCollectionReference().Document(_documentId) : null;
            if (_docSnap != null) {
                _ = this.fetchData();
            }
        }

        /// <summary>
        /// Fetches all data for this ConfigurationManagaer
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
            this.aboutEN = docSnap.GetValue<string>("aboutEN");
            this.aboutES = docSnap.GetValue<string>("aboutES");
            DBManager.configurationManagers[this.documentId] = this;
            return true;
        }

        /// <summary>
        /// Get dictionary representation of this ConfigurationManager
        /// </summary>
        /// <returns>Dictionary containing ConfigurationManager data</returns>
        public override Dictionary<string, object> toDictionary() {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "aboutEN", this.aboutEN},
                { "aboutES" , this.aboutES }
            };
            return data;
        }

        public string getAboutEN() {
            return this.aboutEN;
        }

        public void setAboutEN(string newAboutEN) {
            this.aboutEN= newAboutEN;
        }

        public string getAboutES() {
            return this.aboutES;
        }

        public void setAboutES(string newAboutES) {
            this.aboutES = newAboutES;
        }
    }
}
