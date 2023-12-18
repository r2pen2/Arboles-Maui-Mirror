using Google.Cloud.Firestore;
using Microsoft.Maui.Graphics;
using System.Drawing;
using Color = Microsoft.Maui.Graphics.Color;

namespace ArbolesMAUI.DB.ObjectManagers
{
    public class ReportManager : ObjectManager {

        // Data for this specific Report
        public ReportColor ThisReportColor { get; set; }
        public Timestamp ApprovalDate { get; set; }
        public Timestamp CreateDate { get; set; }
        public GeoPoint Location { get; set; }
        public List<ReportContribution> Contributions { get; set; }
        public string FirstContributionDisplayName { get; set; }
        public string FirstContributionPhotoUrl { get; set; }
        public bool Status { get; set; }
        public string TreeId { get; set; }
        public string TreeName { get; set; }
        public Color MauiColor { get; set; }
        public string CreateDateString { get; set; }

        public class ReportColor {
            public string ColorId { get; set; }
            public int Order { get; set; }
            public string ColorHex { get; set; }

            /// <summary>
            /// Object for storing AMReport Color Data
            /// </summary>
            /// <param name="colorId">Id of report color</param>
            /// <param name="colorHex">HTML Hex for report color</param>
            /// <param name="order">Color order</param>
            public ReportColor(string colorId, string colorHex, int order) {
                ColorId = colorId;
                ColorHex = colorHex;
                Order = order;
            }

            /// <summary>
            /// Get dictionary representation of this ReportColor
            /// </summary>
            /// <returns>Dictionary containing ReportColor data</returns>
            public Dictionary<string, object> toDictionary() {
                Dictionary<string, object> data = new Dictionary<string, object>() {
                { "colorId", ColorId },
                { "colorHex", ColorHex },
                { "order", Order },
            };
                return data;
            }
        }

        public class ReportContribution {
            public string PhotoURL { get; set; }
            public string FileName { get; set; }
            public string ContributorId { get; set; }
            public string ContributorName { get; set; }
            public double Rating { get; set; }
            public bool Flagged { get; set; }
            public Timestamp Date { get; set; }

            public bool Verified { get; set; }

            /// <summary>
            /// Object for storing a contribution to this report
            /// </summary>
            /// <param name="photoUrl">Id of report color</param>
            /// <param name="fileName">HTML Hex for report color</param>
            /// <param name="contributorId">Color order</param>
            /// <param name="contributorName">DisplayName of contributor</param>
            public ReportContribution(string photoUrl, string fileName, string contributorId, string contributorName, bool flag = false, bool verified = false) {
                PhotoURL = photoUrl;
                FileName = fileName;
                ContributorName = contributorName;
                ContributorId = contributorId;
                Rating = 0;
                Flagged = flag;
                Verified = verified;
                Date = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
            }

            /// <summary>
            /// Get dictionary representation of this ReportColor
            /// </summary>
            /// <returns>Dictionary containing ReportColor data</returns>
            public Dictionary<string, object> toDictionary() {
                Dictionary<string, object> data = new Dictionary<string, object>() {
                    { "photoUrl", PhotoURL },
                    { "fileName", FileName },
                    { "contributorName", ContributorName },
                    { "contributorId", ContributorId },
                    { "rating", Rating },
                    { "flagged", Flagged },
                    { "date", Date },
                    { "verified", Verified },
                };
                return data;
            }
        }


        /// <summary>
        /// ObjectManager for Report
        /// </summary>
        /// <param name="_documentId">
        /// Firebase documentID of Report
        /// </param>
        public ReportManager(string? _documentId = null, DocumentSnapshot? _docSnap = null) : base(DatabaseObjectType.Report) {
            this.documentId = _documentId;
            this.docSnap = _docSnap;
            this.docRef = _documentId != null ? this.objectType.getCollectionReference().Document(_documentId) : null;
            Contributions = new List<ReportContribution>();
            if (_docSnap != null) {
                _ = this.fetchData();
            }
        }

        /// <summary>
        /// Fetches all data for this ReportManager
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

            // Create an MFColorManager and set color props.
            string colorHex = docSnap.GetValue<string>(new FieldPath("color", "colorHex"));
            string colorId = docSnap.GetValue<string>(new FieldPath("color", "colorId"));
            int colorOrder = docSnap.GetValue<int>(new FieldPath("color", "order"));
            ThisReportColor = new ReportColor(colorId, colorHex, colorOrder);

            ApprovalDate = docSnap.GetValue<Timestamp>("approvalDate");
            CreateDate = docSnap.GetValue<Timestamp>("createDate");
            Location = docSnap.GetValue<GeoPoint>("location");

            List<ReportContribution> newContributions = new List<ReportContribution>();
            List<Dictionary<string, object>> rawContributions = docSnap.GetValue<List<Dictionary<string, object>>>("contributions");
            foreach (Dictionary<string, object> d in rawContributions) {
                ReportContribution contr = new ReportContribution((string)d["photoUrl"], (string)d["fileName"], (string)d["contributorId"], (string)d["contributorName"], (bool)d["flagged"], (bool)d["verified"]);
                double rating = Convert.ToDouble(d["rating"]);
                contr.Rating = rating;
                newContributions.Add(contr);
            }
            Contributions = newContributions.OrderByDescending(r => r.Rating).ToList();
            FirstContributionDisplayName = Contributions[0].ContributorName;
            FirstContributionPhotoUrl = Contributions[0].PhotoURL;

            Status = docSnap.GetValue<bool>("status");
            TreeId = docSnap.GetValue<string>("treeId");
            TreeName = docSnap.GetValue<string>("treeName");
            DBManager.reportManagers.Add(this.documentId, this);

            InitBindableProperties();

            return true;
        }
        public void InitBindableProperties() {
            MauiColor = Color.FromArgb(ThisReportColor.ColorHex);
            CreateDateString = getDateString(CreateDate.ToDateTime());
        }

        string getDateString(DateTime date) {
            string month;
            switch (date.Month) {
                case 1:
                    month = "January";
                    break;
                case 2:
                    month = "February";
                    break;
                case 3:
                    month = "March";
                    break;
                case 4:
                    month = "April";
                    break;
                case 5:
                    month = "May";
                    break;
                case 6:
                    month = "June";
                    break;
                case 7:
                    month = "July";
                    break;
                case 8:
                    month = "August";
                    break;
                case 9:
                    month = "September";
                    break;
                case 10:
                    month = "October";
                    break;
                case 11:
                    month = "November";
                    break;
                case 12:
                    month = "December";
                    break;
                default:
                    month = "Error";
                    break;
            }
            return month + " " + date.Day + ", " + date.Year;
        }

        /// <summary>
        /// Get dictionary representation of this ReportManager
        /// </summary>
        /// <returns>Dictionary containing ReportManager data</returns>
        public override Dictionary<string, object> toDictionary() {

            Dictionary<string, object> reportColor = ThisReportColor.toDictionary();
            List<Dictionary<string, object>> jsonContributions = new List<Dictionary<string, object>>();
            foreach (ReportContribution contr in Contributions) {
                jsonContributions.Add(contr.toDictionary());
            }

            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "color", reportColor },
                { "approvalDate", ApprovalDate },
                { "createDate", CreateDate },
                { "location", Location },
                { "contributions", jsonContributions },
                { "status", Status },
                { "treeId", TreeId },
                { "treeName", TreeName },
            };
            return data;
        }

        /// <summary>
        /// Return correct map pin img
        /// </summary>
        /// <returns></returns>
        public ImageSource getPinImageSource() {
            string filePath = "marker_tree_" + ThisReportColor.ColorHex.ToLower().Substring(1) + ".svg";
            return ImageSource.FromFile(filePath);
        }

        public async void removeContribution(ReportContribution cont, bool doPush = true) {
            this.Contributions.Remove(cont);
            UserManager um = DBManager.getUserManager(cont.ContributorId);
            _ = um.removeContribution(this.documentId, cont.PhotoURL, false);
            DBManager.deleteImage(cont.FileName);
            if (doPush) {
                if (this.Contributions.Count == 0) {
                    this.cleanDelete();
                } else {
                    _ = this.push();
                }
            }
        }

        public async void cleanDelete() {
            foreach (ReportContribution rc in this.Contributions) {
                this.removeContribution(rc, false);
            }
            await this.docRef.DeleteAsync();
        }
    }
}
