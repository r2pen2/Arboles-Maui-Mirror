using Google.Cloud.Firestore;

namespace ArbolesMAUI.DB.ObjectManagers
{
    public class MessageManager : ObjectManager {

        // Data for this specific Message
        Timestamp date;
        string description;
        string sponsored;
        string title;
        string url;

        /// <summary>
        /// ObjectManager for Message
        /// </summary>
        /// <param name="_documentId">
        /// Firebase documentID of Message
        /// </param>
        public MessageManager(string? _documentId = null, DocumentSnapshot? _docSnap = null) : base(DatabaseObjectType.Message) {
            this.documentId = _documentId;
            this.docSnap = _docSnap;
            this.docRef = _documentId != null ? this.objectType.getCollectionReference().Document(_documentId) : null;
        }

        /// <summary>
        /// Fetches all data for this MessageManagaer
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
            this.date = docSnap.GetValue<Timestamp>("date");
            this.description = docSnap.GetValue<string>("description");
            this.sponsored = docSnap.GetValue<string>("sponsored");
            this.title = docSnap.GetValue<string>("title");
            this.url = docSnap.GetValue<string>("url");
            DBManager.messageManagers[this.documentId] = this;
            return true;
        }

        /// <summary>
        /// Get dictionary representation of this MessageManager
        /// </summary>
        /// <returns>Dictionary containing MessageManager data</returns>
        public override Dictionary<string, object> toDictionary() {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "date", this.date},
                { "description", this.description },
                { "sponsored", this.sponsored },
                { "title", this.title },
                { "url", this.url },
            };
            return data;
        }

        public Timestamp getDate() {
            return this.date;
        }

        public void setDate(Timestamp newDate) {
            this.date = newDate;
        }

        public string getDescription() {
            return this.description;
        }

        public void setDescription(string newDescription) {
            this.description = newDescription;
        }

        public string getSponsored() {
            return this.sponsored;
        }

        public void setSponsored(string newSponsored) {
            this.sponsored = newSponsored;
        }

        public string getTitle() {
            return this.title;
        }

        public void setTitle(string newTitle) {
            this.title = newTitle;
        }

        public string getUrl() {
            return this.url;
        }

        public void setUrl(string newUrl) {
            this.url = newUrl;
        }
    }
}
