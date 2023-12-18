using ArbolesMAUI.DB.ObjectManagers;
using Google.Cloud.Firestore;

namespace ArbolesMAUI.DB {
    public enum DatabaseObjectType {
        Color,
        Configuration,
        Tree,
        FloweringCulture,
        Report,
        User,
        General,
        Message,
        Zone,

        None
    }

    // Extension methods for DatabaseObjectType enum
    public static class DatabaseObjectTypeExtensions {

        /// <summary>
        /// Get string representation of DatabaseObjectType
        /// </summary>
        /// <param name="dbType">DatabaseObjectType enum</param>
        /// <returns>String representation of DatabaseObjectType</returns>
        public static string toString(this DatabaseObjectType dbType) {
            return dbType switch {
                DatabaseObjectType.Color => "AMColor",
                DatabaseObjectType.Configuration => "AMConfiguration",
                DatabaseObjectType.Tree => "AMTree",
                DatabaseObjectType.FloweringCulture => "AMFloweringCulture",
                DatabaseObjectType.Report => "AMReport",
                DatabaseObjectType.User => "AMUser",
                DatabaseObjectType.Zone => "Zone",
                DatabaseObjectType.General => "AMGeneral",
                DatabaseObjectType.Message => "AMMessage",
                _ => null,
            };
        }

        /// <summary>
        /// Get DatabaseObjectType from string
        /// </summary>
        /// <param name="str">String representation of DatabseObjectType</param>
        /// <returns>DatabaseObjectType value for string</returns>
        public static DatabaseObjectType getValue(string str) {
            return str switch {
                "AMColor" => DatabaseObjectType.Color,
                "AMConfiguration" => DatabaseObjectType.Configuration,
                "AMTree" => DatabaseObjectType.Tree,
                "AMFloweringCulture" => DatabaseObjectType.FloweringCulture,
                "AMReport" => DatabaseObjectType.Report,
                "AMUser" => DatabaseObjectType.User,
                "AMZone" => DatabaseObjectType.Zone,
                "AMGeneral" => DatabaseObjectType.General,
                "AMMessage" => DatabaseObjectType.Message,
                _ => DatabaseObjectType.None,
            };
        }

        /// <summary>
        /// Get reference to DatabaseObjectType's FireStore collection
        /// </summary>
        /// <param name="dbType">DatabaseObjectType to get collection for</param>
        /// <returns>CollectionReference to DatabaseObjectType's FireStore collection</returns>
        public static CollectionReference getCollectionReference(this DatabaseObjectType dbType) {
            return dbType switch {
                DatabaseObjectType.Color => DBManager.client.Collection("AMColor"),
                DatabaseObjectType.Configuration => DBManager.client.Collection("AMConfiguration"),
                DatabaseObjectType.Tree => DBManager.client.Collection("AMTree"),
                DatabaseObjectType.FloweringCulture => DBManager.client.Collection("AMFloweringCulture"),
                DatabaseObjectType.Report => DBManager.client.Collection("AMReport"),
                DatabaseObjectType.User => DBManager.client.Collection("AMUser"),
                DatabaseObjectType.Zone => DBManager.client.Collection("AMZone"),
                DatabaseObjectType.General => DBManager.client.Collection("AMGeneral"),
                DatabaseObjectType.Message => DBManager.client.Collection("AMMessage"),
                _ => null,
            };
        }

        /// <summary>
        /// Fetches all documentIDs for this DatabseObjectType
        /// </summary>
        /// <returns>
        /// List of documentIds as strings
        /// </returns>
        public static async Task<bool> fetchAll(this DatabaseObjectType dbType) {
            var list = new List<string>();
            if (dbType == DatabaseObjectType.None) {
                // DatabaseObjectType is "None", so we don't want to fetch any information
                return false;
            }
            if (dbType == DatabaseObjectType.Report) {
                DBManager.listenToReports();
            }
            QuerySnapshot allDocumentsQuery = await dbType.getCollectionReference().GetSnapshotAsync();
            string s = allDocumentsQuery.ToString();
            foreach (DocumentSnapshot documentSnapshot in allDocumentsQuery.Documents) {
                AddManager(dbType, documentSnapshot);
            }
            return true;
        }

        static void AddManager(DatabaseObjectType dbType, DocumentSnapshot documentSnapshot) {
            switch (dbType) {
                case DatabaseObjectType.Color:
                    ColorManager colorManager = DBManager.getColorManager(documentSnapshot.Id, documentSnapshot);
                    DBManager.colorManagers[documentSnapshot.Id] = colorManager;
                    return;
                case DatabaseObjectType.Configuration:
                    ConfigurationManager configurationManager = DBManager.getConfigurationManager(documentSnapshot.Id, documentSnapshot);
                    DBManager.configurationManagers[documentSnapshot.Id] = configurationManager;
                    return;
                case DatabaseObjectType.FloweringCulture:
                    FloweringCultureManager floweringCultureManager = DBManager.getFloweringCultureManager(documentSnapshot.Id, documentSnapshot);
                    DBManager.floweringCultureManagers[documentSnapshot.Id] = floweringCultureManager;
                    return;
                case DatabaseObjectType.General:
                    GeneralManager generalManager = DBManager.getGeneralManager(documentSnapshot.Id, documentSnapshot);
                    DBManager.generalManagers[documentSnapshot.Id] = generalManager;
                    return;
                case DatabaseObjectType.Message:
                    MessageManager messageManager = DBManager.getMessageManager(documentSnapshot.Id, documentSnapshot);
                    DBManager.messageManagers[documentSnapshot.Id] = messageManager;
                    return;
                //case DatabaseObjectType.Report:
                //    ReportManager reportManager = DBManager.getReportManager(documentSnapshot.Id, documentSnapshot);
                //    DBManager.reportManagers[documentSnapshot.Id] = reportManager;
                //    return;
                case DatabaseObjectType.Tree:
                    TreeManager treeManager = DBManager.getTreeManager(documentSnapshot.Id, documentSnapshot);
                    DBManager.treeManagers[documentSnapshot.Id] = treeManager;
                    return;
                case DatabaseObjectType.User:
                    UserManager userManager = DBManager.getUserManager(documentSnapshot.Id, documentSnapshot);
                    DBManager.userManagers[documentSnapshot.Id] = userManager;
                    return;
                case DatabaseObjectType.Zone:
                    ZoneManager zoneManager = DBManager.getZoneManager(documentSnapshot.Id, documentSnapshot);
                    DBManager.zoneManagers[documentSnapshot.Id] = zoneManager;
                    return;
                default:
                    return;
            }
        }
    }
}