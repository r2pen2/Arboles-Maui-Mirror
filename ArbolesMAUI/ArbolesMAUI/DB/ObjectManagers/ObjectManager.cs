using Google.Cloud.Firestore;
using System.Data;

namespace ArbolesMAUI.DB.ObjectManagers
{
    /// <summary>
    /// Abstract class inherited by other ObjectManagers in the DB.ObjectManagers directory
    /// </summary>
    /// <author>Joe Dobbelaar</author>
    public abstract class ObjectManager {
        protected DatabaseObjectType objectType;    // Type of object that this ObjectManager refers to
        public string documentId;                   // Id of document that this ObjectManager points to
        public DocumentReference docRef;            // Reference to this ObjectManager's document (if there's a documentId)
        protected DocumentSnapshot docSnap;         // Snapshot of this document (if fetched)

        /// <summary>
        /// Create an object manager with specified DatabaseObjectType
        /// </summary>
        /// <param name="_objectType">DatabseObjectType to use on this ObjectManager</param>
        /// <author>Joe Dobbelaar</author>
        public ObjectManager(DatabaseObjectType _objectType) {
            this.objectType = _objectType;
        }

        /// <summary>
        /// Set documentId and update docRef
        /// </summary>
        /// <param name="newId">New document ID</param>
        /// <author>Joe Dobbelaar</author>
        public void setDocumentId(string newId) {
            this.documentId = newId;
            this.docRef = this.objectType.getCollectionReference().Document(newId);
        }

        /// <summary>
        /// Get document snapshot for this ObjectManager
        /// </summary>
        /// <returns>DocumentSnapshop for this ObjectManager</returns>
        /// <author>Joe Dobbelaar</author>
        public async Task<DocumentSnapshot> getDocumentSnapshot() {
            return await this.docRef.GetSnapshotAsync();
        }

        /// <summary>
        /// Determine whether or not docRef for this ObjectManager exists in FireStore database
        /// </summary>
        /// <returns>Task resolved true if document exists in DB, otherwise false</returns>
        /// <author>Joe Dobbelaar</author>
        public async Task<bool> documentExists() {
            return (await this.getDocumentSnapshot()).Exists;
        }

        /// <summary>
        /// Change this ObjectManager's collection but keep the same document ID
        /// </summary>
        /// <param name="newCollection">New collection name</param>
        /// <author>Joe Dobbelaar</author>
        public void changeCollection(string newCollection) {
            this.docRef = DBManager.client.Collection(newCollection).Document(this.documentId);
        }

        /// <summary>
        /// Turn this ObjectManager into a dictionary
        /// </summary>
        /// <returns>Dictionary representation of ObjectManager</returns>
        /// <author>Joe Dobbelaar</author>
        public abstract Dictionary<string, object> toDictionary();

        /// <summary>
        /// Push this ObjectManager's data to FireStore
        /// </summary>
        /// <returns>ID of document</returns>
        /// <author>Joe Dobbelaar</author>
        public async Task<string> push() {
            if (this.documentId != null) {
                // This document already exists
                await this.docRef.SetAsync(this.toDictionary());
            } else {
                // Document ID is null. Add doc and return id
                this.docRef = await this.objectType.getCollectionReference().AddAsync(this.toDictionary());
                this.documentId = this.docRef.Id;
            }
            return this.documentId;
        }
    }
}
