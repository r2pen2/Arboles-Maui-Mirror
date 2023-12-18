using ArbolesMAUI.DB.ObjectManagers;
using Firebase.Auth.Providers;
using Firebase.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using UserManager = ArbolesMAUI.DB.ObjectManagers.UserManager;
using ArbolesMAUI.ViewModels;
using Firebase.Storage;
using Java.Util;
using Google.Api;
using Plugin.LocalNotification;

namespace ArbolesMAUI.DB
{
    internal class DBManager {
        public DBManager()
        {
        }

        public static Dictionary<string, ColorManager> colorManagers = new Dictionary<string, ColorManager>();
        public static Dictionary<string, ConfigurationManager> configurationManagers = new Dictionary<string, ConfigurationManager>();
        public static Dictionary<string, FloweringCultureManager> floweringCultureManagers = new Dictionary<string, FloweringCultureManager>();
        public static Dictionary<string, GeneralManager> generalManagers = new Dictionary<string, GeneralManager>();
        public static Dictionary<string, MessageManager> messageManagers = new Dictionary<string, MessageManager>();
        public static Dictionary<string, ReportManager> reportManagers = new Dictionary<string, ReportManager>();
        public static Dictionary<string, TreeManager> treeManagers = new Dictionary<string, TreeManager>();
        public static Dictionary<string, UserManager> userManagers = new Dictionary<string, UserManager>();
        public static Dictionary<string, ZoneManager> zoneManagers = new Dictionary<string, ZoneManager>();

        public static bool firstReportLoad = true;

        // Create Authentication credentials for Firebase
        static FirebaseAuthConfig authConfiguration = new FirebaseAuthConfig() {
            ApiKey = "AIzaSyDOoF_j5AjOUK7rsAs9J19aPKkNiI_ZxiM",
            AuthDomain = "ojeadores-6ee96.firebaseapp.com",
            Providers = new FirebaseAuthProvider[] {
                // new GoogleProvider(),
                new EmailProvider(),
            }
        };
        // Create a public static Authentication handler
        public static FirebaseAuthClient authClient = new FirebaseAuthClient(authConfiguration);

        // Create Google Service Account Credentials from JSON (string representation becuase file system is hard to work with)
        static GoogleCredential googleServiceAccountCredentials = GoogleCredential.FromJson(
            "{\"type\":\"service_account\"," +
            "\"project_id\":\"ojeadores-6ee96\"," +
            "\"private_key_id\":\"6527290c880878bfce5984c093642f9766bea18a\"," +
            "\"private_key\":\"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDAWZhlzSUpW8Lv\\n1BgmRqS/uURjttB5z4lzd23640m8YESRrkCOn4kUz0h77gmCFjt2BPMfj9S+9heY\\n8m+Q7QcSJhuWf/8rp0CQ88ovSF70tTPU4YxOEA7FThXVVtyalJ1hS4Jvw9OixsHd\\n9c7um6n+PRr8PC7cTH2S8+bcndkn1HgdAaQ+HxJU1eLOxtOgs9UY3qy0YddDjzhN\\n9Pw0lFMjytU0B08TPT7Ou9xv4fHz3Wx696d3ZIYfrDrdnSE+BrbsVbvJkfxGG6MZ\\n3waWepQgQcRBwRWKh0H8dXrNFiK5uZMla55/n/AuYCcXqcD8ztiyJ+WjzbKxArmm\\n8IwhMLlnAgMBAAECggEALQntmsy1ymDfnMuDWhcJhZEKd/S7DQKw8xYAbRIiVlcO\\nRzRPjnXBodtFZCthE5JEGwberMG/dXVnIhuGTx+JG9FLfWp8I8sWXbqP2ZFvFoqF\\nV+/2K+Mrqjx19U3Q+usYjMsjACuQ1xkGFQR1Dz7ox4ykUHKBIHlSoAG3SWo2ilIX\\nhJEeZ93HJmTEd7YXPykthtPd/o4+zdN/umIXMi8WCaH7AkZeDTe3H9RKbJgLNfhk\\nsJ0AIJ5BSoWz/JpV7fkGBFaGb7Tf9PFZDstU41sZBdM15ApqjyKX5j357hyCALQb\\nuwOqcUKZje0R6RxvFwRIjwGt7I5lo/wqctINcrsuVQKBgQD8VPWNN/q6WzM1iT+P\\nMD0ZvWBdjX6SqmuxKNCWY6nDxa0q7zPxs3WG2bESLuKirPZc1UTYph1/EgdO49aq\\n4aAqc2gb/8yNcf15iuOdna3ga2wKypaaMuWhOCNM+6j59U+Y70FZrxTN5DBXiJ+F\\nOa7ujkEmKcXk3YMbIRa97iB1MwKBgQDDJWqayC4vnhVv/1gwtUnR1jxsyhzkMHff\\nkFit2J5q9GHm9IkFv5MQLE3XPjqCrv/GAKw/Z7aNDvqaPlFidkaaIRYVMfS/93AQ\\njJksL7XEA8WsIc4kmeqYp1Q8g4mIvNabJRE+87WV35vshDLxiv+w9vrgXtdUeM7L\\nimNuUAOC/QKBgGAh3gsXMKae6DuVNmnO7vhHddcuePJXiv/LlOw2vA6h2P9qcle7\\nDyN4/ET5J87iJlnnxnrjjolCzRX5J2ei3epOXXONWNjqfK85gs4hrIS7aEYUOJw2\\nl9V8FbbWieHw24pgXH1Wfo6SSPiQTHXtLAAmohtv/rzax4AmcwuZFTSRAoGAQImd\\nSfcuHx1e5Z1C40jxUu5RKYFmMKk+27b3PPdI3QJZZnI05qvGzTB5xdeUrieF+0u5\\nq3Z3TwtHIb++VpvbGZobnYLZ+Kqyaya0eYKF0H+W3iiH7l7g0P91wlWWJJyfNMv7\\neiBLS3eaL4xA9NUNXs8vhQiYmh0h9vqbi532q1kCgYEA3qYV1w9alcxsqhjMqRgU\\njUusucySkJ36+XxaTJ3q4G5eAgVsIMV3DAZdyurvjWuetYhDlW0muS75ojXOwoK2\\noqUH0ipc2XGfuMENtTNBsplx+CIwlnFONgUOnobDKmWDoMHXWttksKKjKLzbDh8N\\nw0iJjjWXJMGETdRy8D+03cE=\\n-----END PRIVATE KEY-----\\n\"," +
            "\"client_email\":\"firebase-adminsdk-oo65w@ojeadores-6ee96.iam.gserviceaccount.com\"," +
            "\"client_id\":\"113346405533547130436\"," +
            "\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\"," +
            "\"token_uri\":\"https://oauth2.googleapis.com/token\"," +
            "\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\"," +
            "\"client_x509_cert_url\":\"https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-oo65w%40ojeadores-6ee96.iam.gserviceaccount.com\"}"
            );
        // Create a public static DB Client with Google Service Account Credentials
        public static FirestoreDb client = new FirestoreDbBuilder {
            Credential = googleServiceAccountCredentials,
            ProjectId = "ojeadores-6ee96" // This is in the credentials, but apparently we still need to specify it >:(
        }.Build();

        public static Firebase.Auth.UserCredential currentUser = null;
        public static UserManager currentUserManager = null;

        /// <summary>
        /// Upload an image to the reports directory in storage
        /// </summary>
        public static async Task<string> uploadImage(string fileName, byte[] byteArray) {

            var task = new FirebaseStorage("ojeadores-6ee96.appspot.com", new FirebaseStorageOptions { ThrowOnCancel = true })
                .Child("reports")
                .Child(fileName)
                .PutAsync(new MemoryStream(byteArray), CancellationToken.None, "image/jpg");

            // await the task to wait until upload completes and get the download url
            string downloadUrl = await task;
            return downloadUrl;
        }

        /// <summary>
        /// Remove an image from the reports directory in storage
        /// </summary>
        public static async void deleteImage(string fileName) {

            var task = new FirebaseStorage("ojeadores-6ee96.appspot.com", new FirebaseStorageOptions { ThrowOnCancel = true })
                .Child("reports")
                .Child(fileName).DeleteAsync();

            // await the task to wait until deletion completes
            await task;
        }

        public static void listenToReports() {
            // Create a FirestoreChangeListener that handles messages from DB for added or removed reports
            FirestoreChangeListener reportListener = DatabaseObjectType.Report.getCollectionReference().Listen((snapshot) => {

                foreach (DocumentChange change in snapshot.Changes) {
                    Action changeAction = () => Console.WriteLine("No change");

                    if (change.ChangeType == DocumentChange.Type.Added) {
                        if (!DBManager.reportManagers.ContainsKey(change.Document.Id)) {
                            ReportManager rm = DBManager.getReportManager(change.Document.Id, change.Document);
                            DBManager.reportManagers[change.Document.Id] = rm;
                            changeAction = () => MapUtil.AddNewPin(rm);
                        }
                    } else if (change.ChangeType == DocumentChange.Type.Modified) {
                        ReportManager rm = DBManager.getReportManager(change.Document.Id, change.Document);
                        DBManager.reportManagers[change.Document.Id] = rm;
                    } else if (change.ChangeType == DocumentChange.Type.Removed) {
                        DBManager.reportManagers.Remove(change.Document.Id);
                        changeAction = () => MapUtil.RemovePin(change.Document.Id);
                    }

                    MainThread.BeginInvokeOnMainThread(changeAction);
                }
            });
        }

        /// <summary>
        /// Download an image from any directory in storage
        /// </summary>
        public static async Task<string> downloadImage(string directory, string fileName)
        {
            var task = new FirebaseStorage("ojeadores-6ee96.appspot.com", new FirebaseStorageOptions { ThrowOnCancel = true })
                .Child(directory)
                .Child(fileName)
                .GetDownloadUrlAsync();

            // await the task to wait until upload completes and get the download url
            string downloadUrl = await task;
            return downloadUrl;
        }


        public static async Task<UserManager> setCurrentUser(Firebase.Auth.UserCredential newUser, UserManager newManager = null) {
            if (newUser == null) {
                currentUser = null;
                currentUserManager = null;
                return null;
            }
            currentUser = newUser;
            if (newManager == null) {
                currentUserManager = DBManager.getUserManager(newUser.User.Uid);
                await currentUserManager.fetchData();
            } else {
                currentUserManager = newManager;
            }

            FirestoreChangeListener userListener = currentUserManager.docRef.Listen((snapshot) => {
                currentUserManager = DBManager.getUserManager(newUser.User.Uid, snapshot);
            });

            return currentUserManager;
        }

        /// <summary>
        /// Create a ConfigurationManager that references a specific Configuration on the database
        /// </summary>
        /// <param name="id">ID of Configuration document</param>
        /// <returns>A ConfigurationManager with a docRef to desired Configuration</returns>
        public static ConfigurationManager getConfigurationManager(string? id, DocumentSnapshot? docSnap = null) {
            return configurationManagers[id] != null ? configurationManagers[id] : new ConfigurationManager(id, docSnap);
        }

        /// <summary>
        /// Create a FloweringCultureManager that references a specific FloweringCulture on the database
        /// </summary>
        /// <param name="id">ID of FloweringCulture document</param>
        /// <returns>A FloweringCultureManager with a docRef to desired FloweringCulture</returns>
        public static FloweringCultureManager getFloweringCultureManager(string? id, DocumentSnapshot? docSnap = null) {
            return floweringCultureManagers[id] != null ? floweringCultureManagers[id] : new FloweringCultureManager(id, docSnap);
        }

        /// <summary>
        /// Create a ReportManager that references a specific Report on the database
        /// </summary>
        /// <param name="id">ID of Report document</param>
        /// <returns>A ReportManager with a docRef to desired Report</returns>
        public static ReportManager getReportManager(string? id, DocumentSnapshot? docSnap = null) {
            if (reportManagers.ContainsKey(id)) {
                return reportManagers[id];
            }
            return new ReportManager(id, docSnap);
        }

        /// <summary>
        /// Create an TreeManager that references a specific Tree on the database
        /// </summary>
        /// <param name="id">ID of Tree document</param>
        /// <returns>A TreeManager with a docRef to desired Tree</returns>
        public static TreeManager getTreeManager(string? id, DocumentSnapshot? docSnap = null) {
            if (treeManagers.ContainsKey(id)) {
                return treeManagers[id];
            }
            return new TreeManager(id, docSnap);
        }

        /// <summary>
        /// Create a UserManager that references a specific User on the database
        /// </summary>
        /// <param name="id">ID of User document</param>
        /// <returns>A UserManager with a docRef to desired User</returns>
        public static UserManager getUserManager(string? id, DocumentSnapshot? docSnap = null) {
            if (userManagers.ContainsKey(id) && docSnap == null) {
                return userManagers[id];
            }
            return new UserManager(id, docSnap);
        }

        /// <summary>
        /// Create a ColorManager that references a specific Color on the database
        /// </summary>
        /// <param name="id">ID of Color document</param>
        /// <returns>An ColorManager with a docRef to desired Color</returns>
        public static ColorManager getColorManager(string? id, DocumentSnapshot? docSnap = null) {
            if (colorManagers.ContainsKey(id)) {
                return colorManagers[id];
            }
            return new ColorManager(id, docSnap);
        }

        /// <summary>
        /// Create a ZoneManager that references a specific Zone on the database
        /// </summary>
        /// <param name="id">ID of Zone document</param>
        /// <returns>An ZoneManager with a docRef to desired Zone</returns>
        public static ZoneManager getZoneManager(string? id, DocumentSnapshot? docSnap = null) {
            if (zoneManagers.ContainsKey(id)) {
                return zoneManagers[id];
            }
            return new ZoneManager(id, docSnap);
        }

        /// <summary>
        /// Create a GeneralManager that references a specific General on the database
        /// </summary>
        /// <param name="id">ID of General document</param>
        /// <returns>An GeneralManager with a docRef to desired General</returns>
        public static GeneralManager getGeneralManager(string? id, DocumentSnapshot? docSnap = null) {
            if (generalManagers.ContainsKey(id)) {
                return generalManagers[id];
            }
            return new GeneralManager(id, docSnap);
        }

        /// <summary>
        /// Create a MessageManager that references a specific Message on the database
        /// </summary>
        /// <param name="id">ID of Message document</param>
        /// <returns>An MessageManager with a docRef to desired Message</returns>
        public static MessageManager getMessageManager(string? id, DocumentSnapshot? docSnap = null) {
            if (messageManagers.ContainsKey(id)) {
                return messageManagers[id];
            }
            return new MessageManager(id, docSnap);
        }
    }
}
