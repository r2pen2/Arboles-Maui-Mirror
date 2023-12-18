using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace ArbolesMAUI.DB.ObjectManagers
{
    public class UserManager : ObjectManager {

        // Data for this specific User
        bool isAdmin;
        string mail;
        string name;
        Timestamp createdAt;
        bool isBanned;
        Dictionary<string, List<string>> contributions = new Dictionary<string, List<string>>();

        /// <summary>
        /// ObjectManager for User
        /// </summary>
        /// <param name="_documentId">
        /// Firebase documentID of User
        /// </param>
        public UserManager(string? _documentId = null, DocumentSnapshot? _docSnap = null) : base(DatabaseObjectType.User) {
            this.documentId = _documentId;
            this.docSnap = _docSnap;
            this.docRef = _documentId != null ? this.objectType.getCollectionReference().Document(_documentId) : null;
            this.isBanned = false;
            if (_docSnap != null) {
                _ = this.fetchData();
            }
        }

        /// <summary>
        /// Fetches all data for this UserManagaer
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
            this.isAdmin = docSnap.GetValue<bool>("isAdmin");
            this.mail = docSnap.GetValue<string>("mail");
            this.name = docSnap.GetValue<string>("name");
            this.createdAt = docSnap.GetValue<Timestamp>("createdAt");
            this.contributions = docSnap.GetValue<Dictionary<string, List<string>>>("contributions");
            this.isBanned = docSnap.GetValue<bool>("isBanned");
            DBManager.userManagers[this.documentId] = this;
            return true;
        }

        /// <summary>
        /// Get dictionary representation of this ReportManager
        /// </summary>
        /// <returns>Dictionary containing ReportManager data</returns>
        public override Dictionary<string, object> toDictionary() {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "isAdmin", this.isAdmin },
                { "mail", this.mail },
                { "name", this.name },
                { "createdAt", this.createdAt },
                { "contributions", this.contributions },
                { "isBanned", this.isBanned }
            };
            return data;
        }

        public bool getIsAdmin() {
            return this.isAdmin;
        }

        public void setIsAdmin(bool newIsAdmin) {
            this.isAdmin = newIsAdmin;
        }

        public string getMail() {
            return this.mail;
        }

        public void setMail(string newMail) {
            this.mail = newMail;
        }

        public string getName() {
            return this.name;
        }

        public void setName(string newName) {
            this.name = newName;
        }

        public Timestamp getcreatedAt() {
            return this.createdAt;
        }

        public void setCreatedAt(Timestamp newCreatedAt) {
            this.createdAt = newCreatedAt;
        }

        public bool addContribution(string reportId, string photoUrl) {
            if (!this.contributions.ContainsKey(reportId)) {
                this.contributions.Add(reportId, new List<string>());
            }
            if (this.contributions[reportId].Count < 3) {
                this.contributions[reportId].Add(photoUrl);
                return true;
            }
            return false;
        }
        
        public async Task<bool> removeContribution(string reportId, string photoUrl, bool doDelete = true) {
            if (this.contributions[reportId] != null) {
                this.contributions[reportId].Remove(photoUrl);

                if (this.contributions[reportId].Count == 0) {
                    this.contributions.Remove(reportId);
                }

                if (doDelete) {
                    ReportManager rm = DBManager.getReportManager(reportId);
                    foreach (ReportManager.ReportContribution cont in rm.Contributions) {
                        if (cont.PhotoURL.Equals(photoUrl)) {
                            rm.Contributions.Remove(cont);
                            await Task.Run(() => DBManager.deleteImage(cont.FileName));
                            rm.push();
                        }
                    }
                }

                this.push();
                return true;
            }
            return false;
        }
        public bool getIsBanned() {
            return this.isBanned;
        }
    }

}
