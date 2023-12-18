import { useEffect, useState } from "react";
import './App.css';
import "./bootstrap.css";
import { auth, firestore, storage } from "./firebase";
import { doc, collection, getDoc, onSnapshot, setDoc, addDoc, deleteDoc } from "firebase/firestore";
import { signInWithEmailAndPassword } from "firebase/auth"
import { ref as storageRef, deleteObject } from "firebase/storage";


/**
 * Formats a UTC date string so that it's easier to read
 * @param {String} date string representing a UTC date
 * @returns {String} date formatted as a string
 */
function getSlashDateString(date) {
  const d = date.toDate();
  const day = d.getUTCDate();
  const month = d.getMonth();
  const year = d.getFullYear();

  var monthString = ""
  switch (month) {
      case 0:
          monthString = "January";
          break;
      case 1:
          monthString = "February";
          break;
      case 2:
          monthString = "March";
          break;
      case 3:
          monthString = "April";
          break;
      case 4:
          monthString = "May";
          break;
      case 5:
          monthString = "June";
          break;
      case 6:
          monthString = "July";
          break;
      case 7:
          monthString = "August";
          break;
      case 8:
          monthString = "September";
          break;
      case 9:
          monthString = "October";
          break;
      case 10:
          monthString = "November";
          break;
      case 11:
          monthString = "December";
          break;
      default:
          monthString = "";
          break;
  }

  return (month + 1) + "/" + day + "/" + year;
}


export default function App() {

  const [user, setUser] = useState(null);
  const [loginError, setLoginError] = useState(null);

  const [contributions, setContributions] = useState([]);
  const [reports, setReports] = useState({});

  const [alertStyle, setAlertStyle] = useState("alert-info");
  const [alertMessage, setAlertMessage] = useState("");

  const [focusedContribution, setFocusedContribution] = useState(null);

  useEffect(() => {

    async function fetchContributions() {
      if (user) {
        if (user.isAdmin) {
          const collectionRef = collection(firestore, "AMReport");
          // Add listener
          onSnapshot((collectionRef), (snap) => {
            let newContributions = [];
            const newReports = {};
            
            for (const doc of snap.docs) {
              newReports[doc.id] = doc.data();
              let contributionIndex = 0;
              for (const contribution of doc.data().contributions) {
                contribution["reportId"] = doc.id;
                contribution["index"] = contributionIndex;
                newContributions.push(contribution)
                contributionIndex++;
              }
            }
            
            // Sort contributions by date
            newContributions.sort((a, b) => {
              return b.date - a.date;
            })

            setReports(newReports);
            setContributions(newContributions);
          })
        }
      }
    }

    fetchContributions();
  }, [user]);

  function Login(props) {

    async function handleSubmit() {
      const email = document.getElementById('emailInput').value;
      const pass = document.getElementById('passwordInput').value;

      const u = await signInWithEmailAndPassword(auth, email, pass).catch(() => {
        setLoginError("Error: Username/password does not exist.");
      });

      if (u) {
        const userDoc = doc(firestore, "AMUser", u.user.uid);
        const docSnap = await getDoc(userDoc);
        if (docSnap) {
          setLoginError(null);
          setUser(docSnap.data());
        } else {
          setLoginError("Error: Invalid/missing user document.");
        }
      } else {
        setLoginError("Error: Username/password does not exist.");
      }
    }

    return (
      <div className="d-flex flex-column justify-content-center align-items-center h-100">
        <div className="form-group">
          <label htmlFor="emailInput">Email</label>
          <input type="email" className={"form-control " + (loginError ? "is-invalid" :"")} id="emailInput" aria-describedby="emailHelp" placeholder="Enter email" />
        </div>
        <div className="form-group">
          <label htmlFor="passwordInput">Password</label>
          <input type="password" className={"form-control " + (loginError ? "is-invalid" :"")} id="passwordInput" placeholder="Password" />
        </div>
        <button className="btn btn-primary" onClick={handleSubmit}>Submit</button>
        { loginError && <div className="alert alert-danger m-2">{loginError}</div> }
      </div>
    );
  }
  
  function Dashboard(props) {
    function SignOutButton() {
      return <button className="btn btn-primary" onClick={() => {auth.signOut(); setUser(null);}}>Sign Out</button>;
    }

    const notAdminPage = (
      <div className="d-flex flex-column align-items-center">
        <div className="alert alert-danger m-2">User is not admin</div>
        <SignOutButton />
      </div>
    )

    async function banUser(userId) {
      setAlertStyle("alert-warning");
      setAlertMessage("Banning user: " + userId);
      const userDoc = doc(firestore, "AMUser", userId);
      const userSnap = await getDoc(userDoc);
      const userData = userSnap.data();
      userData.isBanned = true;
      await setDoc(userDoc, userData);
      setAlertStyle("alert-warning");
      setAlertMessage("Removing user's contributions...");
      let deletedContributions = 0;
      for (const contribution of contributions) {
        if (contribution.contributorId === userId) {
          await deleteContribution(contribution);
          deletedContributions++;
        }
      }
      setAlertStyle("alert-success");
      setAlertMessage(`Banned user: [${userId}] and deleted ${deletedContributions} contributions.`);
    }

    async function deleteContribution(contribution) {
      // Remove contribution from report
      setAlertStyle("alert-warning");
      setAlertMessage(`Deleting contribution #${contribution.index} from report: ${contribution.reportId}`);
      const reportDoc = doc(firestore, "AMReport", contribution.reportId);
      const reportSnap = await getDoc(reportDoc);
      const reportData = reportSnap.data();
      reportData.contributions.splice(contribution.index, 1);
      if (reportData.contributions.length > 0) {
        await setDoc(reportDoc, reportData);
      } else {
        await deleteDoc(reportDoc);
        setAlertStyle("alert-info");
        setAlertMessage(`No more contributions— deleting report!`);
      }
      setAlertStyle("alert-info");
      setAlertMessage(`Done w/ report!`);

      // Remove contribution from user
      setAlertStyle("alert-warning");
      setAlertMessage(`Now deleting contribution from user: ${contribution.contributorId}`);
      const userDoc = doc(firestore, "AMUser", contribution.contributorId);
      const userSnap = await getDoc(userDoc);
      const userData = userSnap.data();
      userData.contributions[contribution.reportId] = userData.contributions[contribution.reportId].filter(url => url !== contribution.photoUrl);
      if (userData.contributions[contribution.reportId].length > 0) {
        delete userData.contributions[contribution.reportId];
      }
      await setDoc(userDoc, userData);
      setAlertStyle("alert-info");
      setAlertMessage(`Done w/ user!`);

      // Remove photo from storage
      setAlertStyle("alert-warning");
      setAlertMessage(`Deleting file from storage: ${contribution.fileName}`);
      const filePath = "reports/" + contribution.fileName;
      const deleteRef = storageRef(storage, filePath);
      await deleteObject(deleteRef).catch(() => {
        setAlertStyle("alert-warning");
        setAlertMessage("File did not exist in storage.");
      });
      setAlertStyle("alert-info");
      setAlertMessage("Done w/ storage!");
      
      setAlertStyle("alert-success");
      setAlertMessage("Done deleting contribution!");
    }

    function DeleteButton({contribution}) {
      return <button className="btn btn-danger" onClick={() => deleteContribution(contribution)}>Delete</button>;
    }

    async function handleVerifyClick(contribution) {
      if (contribution.verified) {
        // Unverify contribution
        setAlertStyle("alert-warning");
        setAlertMessage(`Unverifying contribution #${contribution.index} from report: ${contribution.reportId}`);
        const reportDoc = doc(firestore, "AMReport", contribution.reportId);
        const reportSnap = await getDoc(reportDoc);
        const reportData = reportSnap.data();
        const newContribution = reportData.contributions[contribution.index];
        newContribution.verified = false;
        reportData.contributions[contribution.index] = newContribution;
        await setDoc(reportDoc, reportData);
        setAlertStyle("alert-info");
        setAlertMessage(`Unverified contribution #${contribution.index} from report: ${contribution.reportId}`);
      } else {
        // Verify contribution
        setAlertStyle("alert-warning");
        setAlertMessage(`Verifying contribution #${contribution.index} from report: ${contribution.reportId}`);
        const reportDoc = doc(firestore, "AMReport", contribution.reportId);
        const reportSnap = await getDoc(reportDoc);
        const reportData = reportSnap.data();
        const newContribution = reportData.contributions[contribution.index];
        newContribution.verified = true;
        newContribution.flagged = false;
        reportData.contributions[contribution.index] = newContribution;
        await setDoc(reportDoc, reportData);
        setAlertStyle("alert-success");
        setAlertMessage(`Verified contribution #${contribution.index} from report: ${contribution.reportId}`);
      }
    }

    function renderContributions(flagOnly, hideVerified) {

      return contributions.map((contribution, index) => {
        function getColor() {
          return reports[contribution.reportId].color.colorHex;
        }

        return ((flagOnly && contribution.flagged) || (!flagOnly && (!contribution.verified && hideVerified || !hideVerified))) && (
          <div className="border border-dark w-100 d-flex flex-row w-100 p-2 align-items-center" style= {{cursor: "pointer", marginBottom: 5, height: 60, borderRadius: 5}} key={index} onClick={() => setFocusedContribution(contribution)}>
            <div className="d-flex flex-row align-items-center" style={{flex: 1}}>
              <div className="d-flex flex-column align-items-center justify-content-center" style={{padding: 3, backgroundColor: getColor(), borderRadius: 5}}>
                <img src={contribution.photoUrl} style={{borderRadius: 4, width: 40, height: 40, objectFit: "cover"}} alt="reportPhoto"/>
              </div>
              <div className="d-flex flex-column align-items-start justify-content-start" style={{marginLeft: 5}}>
                <div className="d-flex flex-row align-items-center">
                  <small>{`By: ${contribution.contributorName}`}</small>
                  <button className="btn btn-danger" style={{height: 20, width: 50, marginLeft: 5, fontSize: 12, textAlign: "center", padding: 0}} onClick={() => banUser(contribution.contributorId)}>Ban</button>
                </div>
                <small>{`Date: ${getSlashDateString(contribution.date)}`}</small>
              </div>
            </div>
            <div className="d-flex flex-row align-items-center justify-content-between" style={{flex: 1}}>
              <div className="d-flex flex-column align-items-start justify-content-start">
                <small>{`Rating: ${contribution.rating}`}</small>
                <div className="d-flex flex-row align-items-center">
                  { contribution.flagged && <small className={contribution.flagged ? "text-danger" : ""}>{`Flagged`}</small> }
                  <button className={"btn " + (contribution.verified ? "btn-success" : (contribution.flagged ? "btn-danger" : "btn-primary"))} style={{height: 20, width: 50, marginLeft: 5, fontSize: 12, textAlign: "center", padding: 0}} onClick={() => handleVerifyClick(contribution)}>{contribution.verified ? "Verified" : "Verify"}</button>
                </div>
              </div>
              <DeleteButton contribution={contribution} />
            </div>
          </div>
        )
      })
    }

    async function cleanDB() {
      setAlertStyle("alert-info");
      setAlertMessage("Cleaning up reports");
      let cleanedReports = 0;
      let cleanedContributions = 0;
      for (const reportId of Object.keys(reports)) {
        if (reports[reportId].contributions.length === 0) {
          const reportDoc = doc(firestore, "AMReport", reportId);
          await deleteDoc(reportDoc);
          cleanedReports++;
        }
      }
      setAlertStyle("alert-success");
      setAlertMessage("Cleaning up contributions");
      for (const cont of contributions) {
        if (!Object.keys(cont).includes("verified")) {      
          const reportDoc = doc(firestore, "AMReport", cont.reportId);
          const reportSnap = await getDoc(reportDoc);
          const reportData = reportSnap.data();
          const newCont = {...cont};
          newCont["verified"] = false;
          reportData.contributions[newCont.index] = newCont;
          await setDoc(reportDoc, reportData)
          cleanedContributions++;
        }
        if (!Object.keys(cont).includes("flagged")) {      
          const reportDoc = doc(firestore, "AMReport", cont.reportId);
          const reportSnap = await getDoc(reportDoc);
          const reportData = reportSnap.data();
          const newCont = {...cont};
          newCont["flagged"] = false;
          reportData.contributions[newCont.index] = newCont;
          await setDoc(reportDoc, reportData)
          cleanedContributions++;
        }
      }

      setAlertStyle("alert-success");
      setAlertMessage(`Cleaned up ${cleanedReports} reports and ${cleanedContributions} contributions!`);
    }

    function renderModal() {

      function getBackground() {
        return focusedContribution ? reports[focusedContribution.reportId].color.colorHex : "red"
      }

      return ( focusedContribution && reports[focusedContribution.reportId] && 
        <div className="my-modal d-flex flex-column justify-content-center align-items-center p-2" style={{opacity: focusedContribution ? 1 : 0, backgroundColor: getBackground()}}>
        <img className="my-modal-img" src={focusedContribution ? focusedContribution.photoUrl : ""} style={{objectFit: "cover"}} alt="focusedPhoto"/>
        <div className="d-flex flex-column w-100 px-2 align-items-center justify-content-center" style={{gap: 20}}>          
          <div className="d-flex flex-row align-items-center w-100 justify-content-between" style={{marginTop: 5}}>
            <div className="d-flex flex-row align-items-center">
              <small>{`By: ${focusedContribution.contributorName}`}</small>
              <button className="btn btn-danger" style={{height: 20, width: 50, marginLeft: 5, fontSize: 12, textAlign: "center", padding: 0}} onClick={() => banUser(focusedContribution.contributorId)}>Ban</button>
            </div>
            <small>{`Date: ${getSlashDateString(focusedContribution.date)}`}</small>
            <small>{`Rating: ${focusedContribution.rating}`}</small>
            <small className={focusedContribution.flagged ? "text-danger" : ""}>{`Flagged: ${focusedContribution.flagged}`}</small>
          </div>
          <div className="d-flex flex-row w-100 px-2 justify-content-center" style={{gap: 20}}>          
            <button className="btn btn-primary" onClick={() => setFocusedContribution(null)}>Close</button>
            <DeleteButton contribution={focusedContribution} />
          </div>
        </div>
      </div>
      )
    }

    const [hideVerified, setHideVerified] = useState(false);

    const adminPage = (
      <div className="vh-100 w-100 align-items-center" style={{overflow: 'hidden'}}>
        <div className="d-flex flex-column align-items-center vh-100 w-100">
          <nav className="navbar navbar-dark bg-dark justify-content-between w-100">
            <div className="d-flex flex-row align-items-center justify-content-center" style={{flex: 1}}>
              <a className="navbar-brand text-light">Arboles Maui</a>
            </div>
            <div className="d-flex flex-row align-items-center" style={{flex: 6}}>
              <div className={"text-center alert w-100 m-2 " + alertStyle} style={{fontSize: 16, padding: 4, opacity: (!alertMessage ? 0 : 1)}}>{alertMessage}</div>
            </div>
            <div className="d-flex flex-row align-items-center justify-content-end" style={{gap: 10, flex: 1.5}}>
              <button className="btn btn-secondary" onClick={() => cleanDB()}>Clean DB</button>
              <SignOutButton />
            </div>
          </nav>
          <div className="d-flex flex-row w-100 h-100 justify-content-between">
            <section className="d-flex flex-column align-items-center h-100 border border-dark p-2" style={{overflowY: "scroll", flex: 2}}>
              <div className="d-flex flex-row w-100 align-items-center justify-content-center" style={{gap: 20}}>
                <p>All Contributions</p>
                <button className={"btn " + (!hideVerified ? "btn-secondary" : "btn-primary")} style={{height: 40, width: 120, fontSize: 14}} onClick={()=>setHideVerified(!hideVerified)}>Hide Verified</button>
              </div>
              { renderContributions(false, hideVerified) }
            </section>
            <section className="d-flex flex-column align-items-center h-100 border border-dark p-2" style={{overflowY: "scroll", flex: 2}}>
              <p>Flagged Contributions</p>
              { renderContributions(true, true) }
            </section>
            <section className="d-flex flex-column align-items-center h-100 border border-dark p-2" style={{flex: 1}}>
              <p>Create Event</p>
            </section>
          </div>
        </div>

        { focusedContribution && renderModal() }        

      </div>
    )

    return !user.isAdmin ? notAdminPage : adminPage; 
  }

  return <div className="d-flex flex-column justify-content-start align-items-center h-100 vh-100">
    { user ? <Dashboard /> : <Login /> }
  </div>;
}