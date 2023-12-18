// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
import { getFirestore } from 'firebase/firestore'
import { getStorage } from "firebase/storage";
import { getAuth } from 'firebase/auth';
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
const firebaseConfig = {
  apiKey: "AIzaSyDOoF_j5AjOUK7rsAs9J19aPKkNiI_ZxiM",
  authDomain: "ojeadores-6ee96.firebaseapp.com",
  databaseURL: "https://ojeadores-6ee96.firebaseio.com",
  projectId: "ojeadores-6ee96",
  storageBucket: "ojeadores-6ee96.appspot.com",
  messagingSenderId: "692560511581",
  appId: "1:692560511581:web:4f64f7648737f5399483b4"
};

// Initialize Firebase
export const app = initializeApp(firebaseConfig);
export const firestore = getFirestore();
export const auth = getAuth(app);
export const storage = getStorage(app);