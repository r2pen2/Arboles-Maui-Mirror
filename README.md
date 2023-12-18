# [Arboles Maui](https://arbolesmagicos.org)
This is the developer manual for ArbolesMAUI. Its goal is to provide a comprehensive guide through all of the code and tools used, intended to facilitate a smooth 
handoff between development teams.

## Purpose
This project aims to port the existing Arboles Magicos iPhone app to the Android platform, while also adding new features and improving the overall codebase. Our goal is to ensure that the app is easy to maintain and expand upon in the future.
#### Development Environment/Tools
![Microsoft Visual Studio](https://img.shields.io/badge/Microsoft%20Visual%20Studio-%20-blue?style=for-the-badge&logo=visual-studio&logoColor=white&color=5C2D91) ![GitHub](https://img.shields.io/badge/Github-%20-blue?style=for-the-badge&logo=github&logoColor=white&color=181717) ![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-%20-blue?style=for-the-badge&logo=visual-studio&logoColor=white&color=512BD4) ![Git](https://img.shields.io/badge/git-%20-blue?style=for-the-badge&logo=git&logoColor=white&color=F05032)
#### Database Deployment/Management
![Google Firebase](https://img.shields.io/badge/Firebase-%20-blue?style=for-the-badge&logo=firebase&logoColor=white&color=ffca28)
#### Development Languages
![C#](https://img.shields.io/badge/C%20Sharp-%20-blue?style=for-the-badge&logo=c-sharp&logoColor=white&color=239120) ![XAML](https://img.shields.io/badge/XAML-%20-blue?style=for-the-badge&logo=xaml&logoColor=white&color=0C54C2)

## Contact
In the event that anything is unclear after reading through this manual, here’s a list of our contacts:
| Name                                           | Email                     |
|------------------------------------------------|---------------------------|
| [Joe Dobbelaar](https://github.com/r2pen2)     | joedobbelaar@gmail.com    |
| [Lex Graziano](https://github.com/lexgraziano) | alexismgraziano@gmail.com |
| [Cole Parks](https://github.com/cfp02)         | coleparks13@gmail.com     |
| [Jared Chan](https://github.com/CarrotPeeler)  | jared24mc@gmail.com       |

## Tools
The following is a collection of all tools used in this project as of 2/26/23:
- [Microsoft Visual Studio](https://visualstudio.microsoft.com/): Microsoft's comprehensive integrated development environment (IDE) that provides a range of tools and services for building applications across various platforms and languages.
- [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui?view=net-maui-7.0): Microsoft's new cross-platform framework for building native mobile and desktop apps with C# and XAML. Write once, run anywhere.
- [Google Firebase](https://firebase.google.com/): Google's cloud based platform for developing and distributing applications. The entire ArbolesMAUI back-end is built on Google Firebase.
- [Cloudmersive Image API](https://cloudmersive.com/image-recognition-and-processing-api): API for ranking images on how likely they are to be NSFW.

## File System / Project Structure Overview
The .NET Multi-platform App UI (MAUI) project structure is designed to provide a unified development experience across multiple platforms, including iOS, Android, Windows, and macOS. The project structure is built around a single project file that includes platform-specific folders, such as iOS, Android, and macOS, each containing platform-specific implementations of the shared codebase. This approach allows developers to write a single codebase that can be shared across multiple platforms while still enabling platform-specific implementation where necessary. Additionally, the project structure includes a shared project folder, which contains the majority of the codebase that is platform-agnostic, as well as any shared assets, resources, or dependencies.

## Application Architecture
In the development of "ArbolesMAUI," the code is divided into two main sections: a back-end or database section responsible for managing the application's data, and a front-end section that handles the user interface and overall user experience. The database section is responsible for storing and retrieving data, managing user authentication and authorization, and ensuring data consistency and integrity. The front-end section, on the other hand, is responsible for presenting the data in a visually appealing and intuitive manner, and handling user interactions. By splitting the code into these two distinct sections, developers can focus on each section's specific requirements and ensure that the application as a whole is performant, reliable, and user-friendly.

### General
This section covers topics localization, styling, and the static ViewMediator class, which are crucial to ensuring a consistent and high-quality user experience.

#### API Keys
Api keys for Google Maps and Cloudmersive have been removed so that Arboles can create their own accounts. These two missing keys are in the UploadPage and in the android manifest:
```jsx
<meta-data android:name="com.google.android.geo.API_KEY" android:value="API KEY" />
```

#### Localization
Authorities: [Jared Chan](#contact)  

The code for creating localization was heavily adapted and modified from [VladislavAntonyuk](https://github.com/VladislavAntonyuk/MauiSamples/tree/main/MauiLocalization).

Localization is managed by a singleton class, LocalizationResourceManager. 

To change the language, perform the following in C#:
```c#
//Setting culture language to English, United States
LocalizationResourceManager.Instance.Culture = new CultureInfo("en-us"); //CultureInfo must take in a BCP 47 Language Tag
```
OR
```c#
//Setting culture language to Spanish, Costa Rica
LocalizationResourceManager.Instance.Culture = new CultureInfo("es-cr"); //CultureInfo must take in a BCP 47 Language Tag
```

Changing the language creates a call to the C# file, AppResources.cs, which is an autogenerated file by ResGen.exe that manages all AppResources.resx files. AppResources.resx files contain the variable name to reference by in the left column and a corresponding translation in the right column. AppResources.resx is the default translation layer file, which contains English translations. AppResources.es.resx contains Spanish translations. 

Using the TranslateExtension.cs file gives a custom markup tag in xaml, making translation binding simple. You just need to state the localization namespace in the content page parameters and use the ```localization:Translate``` markup tag:
```JSX
<ContentPage 
  xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
  xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
  xmlns:localization="clr-namespace:MauiLocalization.Resources.Localization"
  x:Class="ArbolesMAUI.Views.SamplePage"
>
  <HorizontalStackLayout>
    <Button Text="{localization:Translate TREE-REPORT-PHOTO-SELECT}"/>
  </HorizontalStackLayout>
```

The binding variable used with the markup tag should correspond to a variable name set in the left column of all of the AppResources.resx files. 

#### Styles
Authorities: [Lex Graziano](#contact) and [Joe Dobbelaar](#contact)  

.NET MAUI has partial CSS support, but is best styled using XAML. In the Resources/Styles directory there is a Styles.xaml that is used to create styles. 

All styles have types associated with them and a series of Property setters. The x:Key acts like a CSS class name.
```JSX
<Style TargetType="Button" x:Key="buttonBase">
  <Setter Property="TextColor" Value="#0A1930" />
  <Setter Property="BackgroundColor" Value="#fefefe" />
  <Setter Property="FontFamily" Value="OpenSansRegular"/>
  <Setter Property="FontAttributes" Value="Bold" />
  <Setter Property="BorderColor" Value="#0A1930" />
  <Setter Property="BorderWidth" Value="1" />
  <Setter Property="FontSize" Value="14"/>
  <Setter Property="CornerRadius" Value="10"/>
  <Setter Property="Padding" Value="10,10"/>
  <Setter Property="Margin" Value="0,5,5,0" />
  <Setter Property="VisualStateManager.VisualStateGroups">
    <VisualStateGroupList>
      <VisualStateGroup x:Name="CommonStates">
        <VisualState x:Name="Normal" />
        <VisualState x:Name="Disabled">
          <VisualState.Setters>
            <Setter Property="BackgroundColor" Value="#E4E4E4" />
            <Setter Property="BorderColor" Value="#E4E4E4" />
            <Setter Property="TextColor" Value="#8C8C8C" />
          </VisualState.Setters>
        </VisualState>
        </VisualStateGroup>
    </VisualStateGroupList>
  </Setter>
</Style>
```

TO attach the buttonBase style to a button, set the style prop.
```JSX
<Button 
  Style="{StaticResource buttonBase}"               // <-- buttonBase is the x:Key on the style defined above
  Text="{localization:Translate FILTER-TREECOLOR}"
  Clicked="OnColorFilterClicked"
/>
```

#### ViewMediator
Authorities: [Jared Chan](#contact)  

The ViewMediator class is static and follows the mediator design pattern. It enables views to talk to each other without coupling them. 

The following code demonstrates how references to xaml objects of one view can be stored in the ViewMediator, so other views can access them and change their properties.
```c#
//Culture Page getters/setters

/// <summary>
/// Reference to identify tree species button
/// </summary>
public static Button SelectSpeciesButton { get; set; }

/// <summary>
/// Reference to frame that displays selected color/color of tree species
/// </summary>
public static Frame SpeciesColorBlock { get; set; }
```

Here, xaml object references from the upload page are stored. They can be accessed by the culture page in order to change their background color or text properties based on which color is selected by the user on the palette. 
```c#
private async void OnColorTapped(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        TreeColorGroup group = (TreeColorGroup)btn.BindingContext;
        TreeManager firstItem = group.Trees.First(); 

        //palette.ScrollTo(firstItem, group, ScrollToPosition.Start, true);

        //separate functionality if culture page is accessed within upload page
        if (ViewMediator.UploadPageOpen)
        {
            //get the selected color group and set the upload page color block to that color
            ViewMediator.SpeciesColorBlock.BackgroundColor = ((Button)sender).BackgroundColor;

            //if user identifies by color, return user back to upload page, prevent tree data from loading
            if (ViewMediator.MethodOfIdentification == "By color")
            {
                ViewMediator.SelectSpeciesButton.Text = "Identify the tree";
                await Navigation.PopAsync();
            }
        }

        //similar logic to upload page but used for filter page
        else if (ViewMediator.IsFilterPageOpen)
        {
            if (ViewMediator.MethodOfIdentification == "By color")
            {
                ViewMediator.FilterColorBlock.BackgroundColor = ((Button)sender).BackgroundColor;
                await Navigation.PopAsync();
            }
        }
    }
```

### DB
Authorities: [Joe Dobbelaar](#contact)  

This section covers everything contained within the DB folder, handling all backend operations of the ArbolesMAUI application. It covers the DBManager class, the ObjectManager abstract class, the DatabaseObjectType enum, and important methods contained within them.

#### DBManager
Authorities: [Joe Dobbelaar](#contact)  

All database interactions are handled in real-time by the DBManager, a factory class with a set of static methods for fetching, storing, and making changes to document data and user authentication. The DBManager also actively listens for changes in the Firestore database and updates local data accordingly.

To store data locally, DBManager has a set of dictionaries associating database document Ids with their respective ObjectManagers that are updated whenever a document is pushed or fetched:
```c#
class DBManager {
    public static Dictionary<string, ColorManager> colorManagers = new Dictionary<string, ColorManager>();
    public static Dictionary<string, ConfigurationManager> configurationManagers = new Dictionary<string, ConfigurationManager>();
    public static Dictionary<string, FloweringCultureManager> floweringCultureManagers = new Dictionary<string, FloweringCultureManager>();
    public static Dictionary<string, GeneralManager> generalManagers = new Dictionary<string, GeneralManager>();
    public static Dictionary<string, MessageManager> messageManagers = new Dictionary<string, MessageManager>();
    public static Dictionary<string, ReportManager> reportManagers = new Dictionary<string, ReportManager>();
    public static Dictionary<string, TreeManager> treeManagers = new Dictionary<string, TreeManager>();
    public static Dictionary<string, UserManager> userManagers = new Dictionary<string, UserManager>();
    public static Dictionary<string, ZoneManager> zoneManagers = new Dictionary<string, ZoneManager>();
}
```

The DBManager also abstracts the creation of ObjectManagers, which are discussed in greater detail below. To create an ObjectManager using the DBManager's methods, call DBManager.getObjectManager() for the requested object type.
```c#
class DBManager {
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
}

// Get a ReportManager for the report with documentId "myReport"
ReportManager myReportManager = DBManager.getReportManager("myReport");
GeoPoint myReportLocation = myReportManager.Location; // Will be null until data is fetched
await myReportManager.fetchData();
myReportLocation = myReportManager.Location; // Location from Database
```
...and if you want to create an ObjectManager with existing data, you can pass in that object's Firebase DocumentSnapshot:
```c#
DocumentReference myReportReference = DBManager.client.Collection("AMReport").Document("myReport");
DocumentSnapshot myReportSnap = await myReportReference.getDocumentSnapshot();
ReportManager myReportManager = DBManager.getReportManager("myReport", myReportSnap);
// All fields in myReportManager will me filled with data from the DocumentSnapshot
GeoPoint myReportLocation = myReportManager.Location; // Location from Database
```

To listen for changes on the database, the DBManager sets up a FirestoreChangeListener for the entire "AMReport" collection. When the entire "AMReport" collection is fetched by DatabaseObjectType (explored in greater detail below), the listenToReports() method is called.

When changes are detected on the database, the Listener passes a new snapshot of the entire collection to the callback function. Here, the callback function is defined anonymously.
```c#
public static void listenToReports() {
    // Create a FirestoreChangeListener that handles messages from DB for added or removed reports
    FirestoreChangeListener reportListener = DatabaseObjectType.Report.getCollectionReference().Listen((snapshot) => 
        // For each change that ocurred..
        foreach (DocumentChange change in snapshot.Changes) {
            Action changeAction = () => Console.WriteLine("No change")
            if (change.ChangeType == DocumentChange.Type.Added) {
                // There's a new report! Add it to the dictionary and place a pin on the map
                if (!DBManager.reportManagers.ContainsKey(change.Document.Id)) {
                    ReportManager rm = DBManager.getReportManager(change.Document.Id, change.Document);
                    DBManager.reportManagers[change.Document.Id] = rm;
                    changeAction = () => MapUtil.AddNewPin(rm);
                }
            } else if (change.ChangeType == DocumentChange.Type.Modified) {
                // This report was altered. Change the dictionary entry.
                ReportManager rm = DBManager.getReportManager(change.Document.Id, change.Document);
                DBManager.reportManagers[change.Document.Id] = rm;
            } else if (change.ChangeType == DocumentChange.Type.Removed) {
                // Report deleted! Remove it from the dictionary and remove the pin from the map
                DBManager.reportManagers.Remove(change.Document.Id);
                changeAction = () => MapUtil.RemovePin(change.Document.Id);
            
            // Execute update action on main thread (important for interacting with Map UI)
            MainThread.BeginInvokeOnMainThread(changeAction);
        }
    });
}
```

#### ObjectManagers
Authorities: [Joe Dobbelaar](#contact)  

ObjectManager is an abstract class that is extended by several subclasses. To get an ObjectManager for a specific document, use the DBManager as described above.
As of 2/28/23, only the following subclasses are used the application:
```c#
public class ColorManager : ObjectManager { ... } 
public class ReportManager : ObjectManager { ... } 
public class TreeManager : ObjectManager { ... } 
public class UserManager : ObjectManager { ... } 
```

The constructor for the ObjectManager just takes a DatabaseObjectType. All ObjectManagers contain the following fields and methods:
```c#
public class ObjectManager {
        protected DatabaseObjectType objectType;    // Type of object that this ObjectManager refers to
        public string documentId;                   // Id of document that this ObjectManager points to
        public DocumentReference docRef;            // Reference to this ObjectManager's document (if there's a documentId)
        protected DocumentSnapshot docSnap;         // Snapshot of this document (if fetched)

        public async Task<DocumentSnapshot> getDocumentSnapshot() { }   // Get the document snapshot by DocumentReference 
        public async Task<bool> documentExists() { }                    // Get whether or not a document with this ID exists on the Database
        public void changeCollection(string newCollection) { }          // Change the collection that this ObjectManager refers to (only used to move data from one collection to another)
        public async Task<string> push() { }                            // Call this.toDictonary() and push data to current collection
}
```
All subclasses must implement the toDictionaryMethod(). This is because the Firestore database is very picky about what kind of data it is given. All data sent to Firestore must be JSON parseable, so we turn the ObjectManager into a Dictionary<string, object> before pushing it to the DB. Since every ObjectManager has different fields, this method must be implemented in each subclass.
```c#
// ObjectManager abstraction
public class ObjectManager {
        public abstract Dictionary<string, object> toDictionary();
}

// Implementation example
public class UserManager : ObjectManager {
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
}
```

#### DatabaseObjectType
Authorities: [Joe Dobbelaar](#contact)  

DatabaseObjectType is an enum used by ObjectManagers to keep track of and call methods related to their specific type (Users, Reports, etc.). All ObjectManagers have their DatabaseObjectType automatically assigned upon creation. 

DatabaseObjectType.js contains the enum as well as a public static class DatabaseObjectTypeExtensions. DatabaseObjectTypeExtensions allows for several methods to be called on DatabaseObjectType as if it were a class. Ex:
```c#
// Database interaction methods
CollectionReference colorCollection = DatabaseObjectType.Color.getCollectionReference(); // User by all ColorManagers to get their collection
DatabaseObjectType.Color.fetchAll(); // Async fetches the entire Color collection, creates colorManagers, and saves all data into DBManager's colorManagers Dictionary

// Misc methods (unused as of 2/28/23)
string colorString = DatabaseObjectType.Color.toString();   // colorString = "AMColor"
DatabaseObjectType colorEnum = DatabaseObjectType.getValue("AMColor");    // Inverse of toString()
```

#### Firestore Database
Authorities: [Joe Dobbelaar](#contact)  

ArbolesMAUI uses the same Firestore database as all previous versions of the Ojeadores app. To distinguish between collections used in ArbolesMAUI and older versions, our collections all start with "AM".

| Collection Name       | Utility                                                |
|-----------------------|-------------------------------------------------------|
| AMColor               | Arboles Magicos color catalogue                       |
| AMConfiguration       | Unused                                                |
| AMFloweringCulture    | Unused                                                |
| AMGeneral             | Unused                                                |
| AMMessage             | Unused                                                |
| AMReport              | Map markers, contain lists of "contributions"         |
| AMTree                | Arboles Magicos tree catalogue                        |
| AMUser                | Data on all users, including their "contributions"    |
| AMZone                | Unused                                                |

#### Firebase Storage
Authorities: [Joe Dobbelaar](#contact)  

All storage operations are handled by—you guessed it—the DBManager! While it was a terrible struggle to get working at first, uploading and deleting photos has been abstracted so much that it should now be very straight forward.

DBManager contains these three static methods for interacting with storage.
```c#
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
        .Child(fileName).DeleteAsync()
    // await the task to wait until deletion completes
    await task;
}

/// <summary>
/// Download an image from any directory in storage
/// </summary>
public static async Task<string> downloadImage(string directory, string fileName)
{
    var task = new FirebaseStorage("ojeadores-6ee96.appspot.com", new FirebaseStorageOptions { ThrowOnCancel = true })
        .Child(directory)
        .Child(fileName)
        .GetDownloadUrlAsync()
    // await the task to wait until upload completes and get the download url
    string downloadUrl = await task;
    return downloadUrl;
}
```
They're a little gross looking, but they're really easy to use.
```c#
byte[] imageByteArray; // Any image should be able to be turned into a byte array
string imageUrl = await DBManager.uploadImage("myImage.jpg", imageByteArray); // Upload image with name myImage.jpg
string theSameUrl = await DBManager.downloadImage("reports", "myImage.jpg"); // Get that image's download url (if needed)
await deleteImage("myImage.jpg"); // Delete the image so we're not taking up any unnecessary storage
```

#### Firebase Authentication
Authorities: [Joe Dobbelaar](#contact)  

While at first we had hoped to use Google as a sign-in method, we ran into some issues with the .NET MAUI in-app web browser that set off a bunch of red flags on Google's end. Since this didn't work, we decided to just use an email and password sign-in scheme.

All authentication is done through Firebase, so we're not worried about handling any sensitive user information.

DBManager has a GoogleAuthenticationClient that is used by the Login Page. When a user tries to login, the GoogleAuthenticationClient handles everything. If the login is successful, we fetch the document from AMUser associated with the user's UID returned from the login attempt. If that document doesn't exist, we make one. The currentUserManager is stored in the DBManager and a listener is set up to catch any changes. The Signup Page does much the same, but also adds the user to the authentication panel if their account didn't already exist.

### About Arboles Page 
Authorities: [Cole Parks](#contact)  

The about Árboles Mágicos page is the leftmost page in the appShell navigation. The data (text) from this page was taken from the published *Ojeadores* app, and contains a simple paragraph about the organization, along with the logo and other graphics requested. Under the text are the Árboles Mágicos social icons, which redirect you to the organization's website, Facebook page, Instagram page, and email address in the default email client.
### Culture Page
Provides a visually appealing way to explore and view tree species by color. This is the second left most page on the AppShell navigation bar. The design for this page was taken and modified from Diana Zuleta. 
#### Palette
Authorities: [Jared Chan](#contact)  

The palette page has two basic elements: a top control bar with a search bar and a button to reload the view, and a collection view.

The collection view loads the colors and their corresponding tree species using MVVM. The collection view is set up to use groups in order to categorize trees by their color.

The entire culture page binding context is set to an instance of the ```CultureViewModel``` class. The collection view's item source is data bound to a bindable property of the class instance, ```TreeColorGroups```. This property is set at instance creation and is a list of ```TreeColorGroup``` model objects. The model class, ```TreeColorGroup```, contains bindable properties that represent the group color, order, color id, etc., and the class inherits from ```List<TreeManager>```, meaning the class also contains a list of ```TreeManager```. Using this MVVM format for grouping enables the collection view to populate and categorize data by color. For each individual tree that appears under a color tab in the collection view, their data is populated using bindable properties from their corresponding ```TreeManager```.

To clarify, ```TreeColorGroup``` is the binding context for the collection view group headers. ```TreeManager``` is the binding context for each item in the collection view. 

Populating and removing items from the collection view is handled via commands sent to the culture page's viewmodel, ```CultureViewModel```.

**Commands currently implemented**
1) ```AddOrRemoveGroupDataCommand```: populates a color header, when tapped on the palette, with ```TreeManager``` items; clears the items of all other color header groups.
2) ```SearchForTreesCommand```: handles which color header groups are populated and which trees appear under each based on the searched tree name.
3) ```ClearItemsCommand```: clears the items of all color group headers; used for refreshing the culture page palette and clearing the search bar entry text. 

#### Tree Detail Page
Authorities: [Lex Graziano](#contact)  

The Tree Detail Page only accessible through the [Culture Page](#culture-page). It loads data for the tree selected from [DBManager's](#dbmanager) treeManagers Dictionary.

The top of the page shows a carousel of the focused tree's official Arboles Magicos pictures. In the middle of the page is a description of the tree, along with a calendar of when the tree flowers. This calendar starts off empty and fills in green blocks for each week in the tree's floweringWeeks array.

To add or modify trees, Arboles Magicos should access the [Firestore Database](#firestore-database) and add an AMTree. The Culture page and Tree Detail Page will render all trees in this collection.

### Map Page
Authorities: [Cole Parks](#contact), [Jared Chan](#contact), and [Joe Dobbelaar](#contact)  

The Map Page is the main screen of ArbolesMAUI. It displays the Microsoft.Maui.Map object stored in ViewMediator to the user, gives them a set of map controls, and the option to upload photos.
#### Loading the Map
Authorities: [Joe Dobbelaar](#contact)  

Due to some issues we had with keeping the map consistent between the Map Page and the Location Picker (both should display the same map, but pins were only getting added on whichever the user was currently looking at), The Map Page places the map stored in [ViewMediator](#viewmediator) in a frame when the page first loads.

We then load all pins from the database.

```JSX
<Frame x:Name="mapFrame" Padding="0" Margin="0"/>
```
```c#
public MapPage()
{
    InitializeComponent();
    mapFrame.Content = ViewMediator.Map; // Set the content of the mapFrame to the Map in the ViewMediator
    MapUtil.AddAllPinsFromDB();
}
```
#### MapUtil
Authorities: [Cole Parks](#contact), [Jared Chan](#contact), and [Joe Dobbelaar](#contact)  

MapUtil is a static class for interacting with the [ViewMediator's](#viewmediator) Map object. MapUtil is intended to abstract all of the disgusting logic necessary for making changes to the Microsoft.Maui.Map object. It contains methods for adding pins, removing pins, getting all pins from the database, updating the map in real-time when new pins are added by other users, filtering pins, and more. 
#### Adding / Removing Pins
Authorities: [Joe Dobbelaar](#contact)  

The addNewPin() and removePin() methods have been abstracted enough that they should be really easy to work with, though they were a nightmare to implement.

To add a pin to the map, call addNewPin() on a reportManager. To remove a pin, call removePin() on a reportId.
```c#
ReportManager myReport = DBManager.getReportManager("report1");
MapUtil.addNewPin(myReport);   // Reads report and adds a pin of the correct color to the map
MapUtil.removePin(myReport.documentId); // Removes the pin associated with report of given ID
```
Please take a look at how addNewPin() works. Adding and removing pins MUST happen on the main thread, otherwise the change won't be reflected on the map's UI. The pin will still be "added" to the map, but you won't be able to see it. This was such an unbelievably frustrating problem to solve. I wouldn't wish trying to figure it out again on my worst enemy.
```c#
/// <summary>
/// Authored by Jared Chan & Joe Dobbelaar
/// Adds a new pin to the map
/// </summary>
public static async void AddNewPin(ReportManager report)
{
    // First we create a new pin from the ReportManager provided
    var pin = new CustomPin() {
        Report = report,
        Type = PinType.Place,
        Location = new Location(report.Location.Latitude, report.Location.Longitude),
        Label = "",
        Address = "",
        ImageSource = report.getPinImageSource(),
    };
    
    // We attach listener to the pin that displays a TreePopup when clicked
    pin.MarkerClicked += async (s, args) =>
    {
        args.HideInfoWindow = true;
        if (ViewMediator.UploadPageOpen)
        {
            ViewMediator.LastClickedPinInLocationselector = pin;
            ViewMediator.ReportToAddTo = report;
        }
        // TreePopup takes a report for data binding
        TreePopup popup = new TreePopup(report);
        popup.BindingContext = report;
        Application.Current.MainPage.ShowPopup(popup);
    };

    // THIS IS SO IMPORTANT!!! The pin MUST be added on the MAIN THREAD.
    // If we don't use the main thread, the application will recognize that a pin was added but it will NOT update the UI.
    MainThread.BeginInvokeOnMainThread(() => {
        ViewMediator.Map.Pins.Add(pin);
    });

    // We can also send a push notification if we want
    if(ViewMediator.FirstMapLoad == false) PushNotifUtil.AddPushNotification(report);
}
```
#### Camera Button
Authorities: [Joe Dobbelaar](#contact)  

The camera button on the map takes the user to the upload page. So long as the user is signed in and isn't banned (a boolean on their AMUser document), the camera button will be shown.

In the event that a user gets banned while the map is open, we still check that a user isn't banned before pushing any contributions to the database.
#### Map Controls
Authorities: [Cole Parks](#contact)  

The map controls are contained in an expander object with a caret as the icon. When the icon is tapped, the three control functions are displayed:
1. Center map on user
    This uses the last known location of the user's phone using the gps, and sets the center of the map to this location.
2. Center map on searched location
    When tapped, a search bar pops up at the top of the map that lets the user search for a Google Maps location (Google Maps Places API). These places are autofilled into a search results VerticalStacklayout, and tapping on one centers the map on that location. 
3. Filter
    When tapped, an instance of the Filter Page is opened, which allows the user to select filters and update the visible map pins accordingly. See the Filter Page section for more information.
#### Filters
Authorities: [Jared Chan](#contact)  

The filter page is a separate page that applies filters to the map page. Filters are currently underdeveloped and somewhat broken. The current implementation for filters is stored in the static utility class, ```MapFilterUtil```.

Three methods currently exist:
1) ```FilterByColor(Color color)```: removes all pins from the map and then re-adds only the pins that match the given color parameter.
2) ```FilterByName(string name)```: removes all pins from the map and then re-adds only the pins that match the given string name parameter.
3) ```ReloadAllMapPins()```: re-adds all existing pins (broken implementation)

Known issues: Filter implementation should not delete or add pins from the map due to potentially bad interaction with the live-updates database listener. Changing implementation to instead hide or show pins should fix the current issues. However, toggling pin visibility is a known challenge to implement. 
#### Location Searching
Authorities: [Jared Chan](#contact)  

Location searching functionality utilizes Google Places API. All functions that make requests and handle responses from the API are stored in the static utility class, ```PlacesUtil```. 

Google Places API Services Used:
1) AutoComplete: Returns a list of suggested of addresses/places based on a given string address/place (used for when users start searching for a location by address/name)
2) Search.TextSearch: Returns the specific location information, including coordinates, of given address/name of a place (used when the user selects a suggested address/name returned by AutoComplete) - NOTE: AutoComplete does not give coordinate information in its responses, which is why TextSearch is used to perform a backward search. 

How to send requests and handle responses for AutoComplete:
```c#
//send request of data to API
PlacesAutoCompleteRequest request = new PlacesAutoCompleteRequest
{

    Key = API_KEY,
    Input = searchBar.Text,
    Language = lang,
    LocationBias = new LocationBias
    {
        Location = new Coordinate(currLoc.Latitude, currLoc.Longitude),
        Radius = searchRadius
    }
};

//Retrieve response from our request
PlacesAutoCompleteResponse response = await GooglePlaces.AutoComplete.QueryAsync(request);
```

You can then iterate through the list of predictions to obtain the returned readable data:
```c#
//populate list view search results
foreach (Prediction pred in response.Predictions.ToList())
{
    readableResponseList.Add(pred.Description);
}
```

How to send requests and handle responses for Search.TextSearch:
```c#
PlacesTextSearchRequest request = new PlacesTextSearchRequest
{

    Key = API_KEY,
    Query = LocSearchResults.SelectedItem.ToString(),
    Language = lang,
    Location = new Coordinate(currLoc.Latitude, currLoc.Longitude),
    Radius = searchRadius
};

//Handle response from our request
PlacesTextSearchResponse response = await GooglePlaces.Search.TextSearch.QueryAsync(request);
Coordinate coords = response.Results.FirstOrDefault().Geometry.Location;
```

### Upload Page
Authorities: [Cole Parks](#contact) 

The Upload Page is a modal that allows a user to upload a photo of a tree to the database, and it opens when the Camera icon on the Map Page is tapped. From top down, there is a photo placeholder (filled in upon selecting or taking a photo) buttons to take a photo or select one from the user's camera roll, a button to open the LocationPicker page to select a location for the tree, a date selector, a button to open the Palette page to select a color for the tree, and a button to upload the tree to the database as well as a cancel button to close the modal. 
In addition, the LocationSelector button and the tree identification button are updated with a location and color when the user selects a location and a color, respectively.
#### Links
Authorities: [Cole Parks](#contact)  

The LocationPicker and Palette pages are opened as modals when their respective buttons are tapped. This is done using the NavigationPage stack. This is the code that opens the LocationPicker page, and a similar function exists for the Palette page.
```c#
private async void LocPickerButton_Clicked(object sender, EventArgs e)
{   
    // Set the pin selector to the defaults
    ViewMediator.UsePinAsLocation = false;
    
    // Sets the ViewMediator LocationPickerButton instance to the pointer of the LocationPicker button
    ViewMediator.LocationPickerButton = LocationPicker;
    await Navigation.PushAsync(new LocationPicker());
}

```

#### Image Filtering
Authorities: [Joe Dobbelaar](#contact)  

We ensure that images uploaded to the database are inoffensive by making calls to the [Cloudmersive NSFW API](#cloudmersive-image-api). When a user selects an image, we send it to the API and block the user from uploading until the API returns a response. The image is allowed to be sent to the database if it gets a score lower than the threshold. Currently, this threshold is really low. Ideally it won't block any images of trees, but there could be tuning necessary.

The API receives a byte array containing the picture. With our current plan, the image may be no larger than 3.5MB. We use the MAUI IImage interface to compress images before sending them to the API.
```c#
private async void checkImage() {
    checkingImage = true; // Note that we're still checking whether or not this image is NSFW
    Configuration.Default.AddApiKey("Apikey", "7642d63d-55d5-44db-a768-29986d02bce5");
    var apiInstance = new NsfwApi();
    // Compress image
    MemoryStream imageStream = new MemoryStream(imageByteArray);
    Microsoft.Maui.Graphics.IImage compressedImage = PlatformImage.FromStream(imageStream);
    compressedImage = compressedImage.Downsize(300);
    byte[] compressedByteArray = compressedImage.AsBytes();

    try {
        // Not safe for work (NSFW) racy content classification
        MemoryStream compressedStream = new MemoryStream(compressedByteArray);
        NsfwResult result = await apiInstance.NsfwClassifyAsync(compressedStream);
        
        // This is our "threshold". Cloudmersive says anything over .8 is high certainty NSFW.
        // We found that even a score of .5 was probably enough to stop if from being appropriate for an app for pictures of trees
        // The threshold as of 3/1/23 is VERY conservative. It will occasionally block pictures of people from upload
        safeImage = (result.Score < 0.1);

        checkingImage = false;
    } catch (System.Exception apiEx) {
        // If we've reached our quota (or some other error), just let the image through
        Console.WriteLine("Exception when calling NsfwApi.NsfwClassify: " + apiEx.Message);
        checkingImage = false;
        safeImage = true;
    }
}
```

Cloudmersive only allows us 800calls/month^2. See [Scalability](#cloudmersive-image-api). Images will not be blocked from upload when the quota is reached.

### Location Picker
Authorities: [Cole Parks](#contact)  

The LocationPicker page exists to allow the user to either select a location on the map, or use an existing location. The page is opened as a modal from the Upload Page, and it contains an instance of map with a crosshair in the center as well as a cancel button and a confirmation button. 
1. To use a map location with the crosshairs, the user simply centers the crosshairs on the desired location and taps the confirmation button. The Location of the center of the map Map.VisibleRegion.Center is saved in the ViewMediator class, and the LocationPicker page is closed.
2. To use an existing tree's location, the user taps on a pin on the map. In the MapUtil class, when a pin is clicked, the Report that the pin represents is saved in the ViewMediator class. 
```c#
ViewMediator.ReportToAddTo = report;
```
#### Loading the Map
Authorities: [Joe Dobbelaar](#contact)  

To ensure consistency, the Location Picker implements the map the same way the Map Page does. See [Map Page: Loading the Map](#loading-the-map)
#### Crosshair
The crosshairs is a png image that is placed in the center of the screen. When the map is panned, they stay in the same spot and are exactly the Map.VisibleRegion.Center Location. This location is used when the user taps the confirmation button to select the location.
#### "Use This Tree"
The "Use This Tree" button appears in the pin popup when a user taps on a pin (only when on the LocationPickerPage). When the user taps the button, the Location of the pin is saved in the ViewMediator class, and both the TreePopup and LocationPicker pages are closed. The LocationPicker button on the Upload Page is updated with the location of the pin, which in this case is taken from the Location stored in the report field of the pin that was tapped.

### Events Page
There's an empty page for displaying Arboles Magicos events to the user. Arboles should be able to create events in the [Admin Portal](#admin-portal). These events will show up on the map with special markers, as well as send all users a push notification. For more information, see [Events](#requested-features--recommendations).

### Account Page
Authorities: [Joe Dobbelaar](#contact)  

When the user is signed in, the Account Page shows the user's personal information and button to sign-out. When the user is signed out, the Account Page shows the same Login Form from the Login Page. The Account Page always shows language options.

We had initially planned for the Account Page to show a list of the user's contributions. Contributions are stored on the user, so this would be easy to implement. We just, unfortunately, didn't have time to add this feature.
### Login Page
Authorities: [Joe Dobbelaar](#contact)  

Login Page allows the user to sign into an existing account or create a new one. We also attempt to sign the user in automatically if we detect that there is a session active on the device.
#### Sign-In Workflow
See [Firebase Authentication](#firebase-authentication).
#### Sign-Up Workflow
See [Firebase Authentication](#firebase-authentication).
#### Auto Sign-In
As of 3/1/23, the auto sign-in method is really scuffed. None of us knew how it should be done so [Joe](#contact) implemented a system that, as far as we know, works (albeit slowly). When the user logs in, we create an empty file in the application's cache directory called loggedIn.txt. Signing out deletes this file. On startup, if the file exists, we attempt to sign the user in with GoogleAuthenticationClient stored credentials.

There's no doubt a better way to persist a user's login session. We just didn't have time to investigate further and this seemed to work for our purposes.
### Admin Portal
Authorities: [Joe Dobbelaar](#contact)  

There's an administration portal written in React.js in the Admin/arboles-admin directory. The portal is not currently hosted anywhere.

The portal fetches all contributions and allows anyone with an administrator account (same login as ArbolesMAUI) to delete posts, verify posts, and ban users. There's also an area to add an Event Creation form. See [Moderation Levels](#requested-features--recommendations) and [Events](#events-page).

## Challenges
Here's a list of some challenges we encountered and their solutions.

- Adding and removing pins on the map must happen on the main thread! See [Adding / Removing Pins](#adding--removing-pins).
- The Map Page and Location Picker were not showing the same map for the longest time. We solved this by making absolutely everything static and loading the [ViewMediator's](#viewmediator) map into both pages, rather than loading the Map Page's map into the [ViewMedaitor](#viewmediator). See [Loading the Map](#loading-the-map).
- Firebase Storage uploading didn't at all. The solution was to stop using [Google's NuGet Package](https://cloud.google.com/storage/docs/reference/libraries#client-libraries-usage-csharp) and switch to the third party [FirebaseStorage.net](https://github.com/step-up-labs/firebase-storage-dotnet).
- Firebase Authentication didn't work either. The solution was, again, to switch from [Google's Authentication NuGet Package](https://cloud.google.com/storage/docs/reference/libraries#client-libraries-usage-csharp) to a third party package: [FirebaseAuthentication.net](https://github.com/step-up-labs/firebase-authentication-dotnet).
## Scalability
Authorities: [Joe Dobbelaar](#contact)  

Since ArbolesMAUI uses some external APIs / tools, there will be costs associated with scaling up the application. All current quotas are from the Free-Tier versions of these services.

### [Google Firebase](https://firebase.google.com/pricing/)
Google Firebase has a free plan (called "Spark") that is enough to support ArbolesMAUI for the time being. For larger scale projects, they offer a "pay as you" plan that charges based on the number of monthly active users an application has.

#### Firestore Database
| Quota             | Free Until| Then          |
|-------------------|-----------|---------------|
| GB Stored         | 1GB       | $0.18/GB/Month|
| Document Writes   | 20,000/Day| $0.18/100,000 |
| Document Reads    | 50,000/Day| $0.06/100,000 |
| Document Deletes  | 20,000/Day| $0.02/100,000 |
#### Firebase Storage
| Quota         | Free Until| Then          |
|---------------|-----------|---------------|
| GB Stored     | 5GB       | $0.026/GB     |
| DB Downloaded | 1GB/Day   | $0.12/GB      |
| File Uploads  | 20,000/Day| $0.05/10,000  |
| File Downloads| 50,000/Day| $0.004/10,000 |
#### Firebase Authentication
| Monthly Active Users  | Price per MAU|
|-----------------------|--------------|
| 0 - 49,999            | $0           |
| 50,000 - 99,999       | $0.0055      |
| 100,000 - 999,999     | $0.0046      |
| 1,000,000 - 9,999,999 | $0.0032      |
| 10,000,000+           | $0.0025      |

### [Cloudmersive Image API](https://cloudmersive.com/pricing-small-business#:~:text=To%20test%20the%20API%20and,can%20always%20grow%20from%20there.)
Cloudmersive image recognition API is free up to 800 Calls/month^2
| Quotas vs. Price  | $0.00/Month   | $19.99/Month      | $49.99/Month      | etc.                                                                                                                                   |
|-------------------|---------------|-------------------|-------------------|----------------------------------------------------------------------------------------------------------------------------------------|
<<<<<<< HEAD
| API Calls         | 800/Month^2  | 20,000/Month^2   | 50,000/Month^2   | [See Pricing](https://cloudmersive.com/pricing-small-business#:~:text=To%20test%20the%20API%20and,can%20always%20grow%20from%20there.) |
=======
| API Calls         | 800/Month^2   | 20,000/Month^2    | 50,000/Month^2    | [See Pricing](https://cloudmersive.com/pricing-small-business#:~:text=To%20test%20the%20API%20and,can%20always%20grow%20from%20there.) |
>>>>>>> 1bd41e0862aaee7c9f7e60a51d2d1cb5c7c7b197
| API Call Frequency| 1 Call/Second | 2 Calls/Second    | 2 Calls/Second    | [See Pricing](https://cloudmersive.com/pricing-small-business#:~:text=To%20test%20the%20API%20and,can%20always%20grow%20from%20there.) |
| Max File Size     | +-3.5MB       | +-1GB             | +-4GB             | [See Pricing](https://cloudmersive.com/pricing-small-business#:~:text=To%20test%20the%20API%20and,can%20always%20grow%20from%20there.) |
## Requested Features / Recommendations
Arboles Magicos expressed interest in many features that we didn't have time to implement. Here's a complete list of requested features and our thoughts on how they may be implemented.

- Show the user's contributions on the account page
    - This should be easy to do. Links to the user's contribution photos are stored on the user's database document. We didn't have time to design and implement this, but it would be quick to build out and make the Account Page much more useful.
- IOS Version
    - .NET MAUI allows for easy cross-platform development. The current map component, however, is android specific. We didn't have time to create an IOS version of the map, but we're confident that the rest of the app will work on Apple devices without needing much attention. Any developer trying to write for IOS will need a computer running MacOS and XCode. 
- Hidden Rating System
    - Users should be able to react to photos. Arboles Magicos would like the reactions to be emojis that users can attach to pictures (hearts, flowers, smiles, etc.). These reactions may contribute to how close to the top of the stack the photo shows up. The photo with the highest hidden rating will be displayed first.
    - It has also been suggested that photos are just displayed in order of when they were posted, newer photos first.
    - Some photos should be "starred" by Arboles Magicos. These starred photos will always show up before un-starred photos.
- Hotspots
    - Zooming out on the map creates a big blob of map markers. It would be nice to have pins turn into a heatmap once the user has zoomed out far enough.
    - The best way we came up with to do this is to assign each marker a coordinate on a 2d grid that covers the entire map. Like Minesweeper, one could determine if a pin is close to another by checking the 8 coordinates surrounding it. This way we wouldn't have to check the distance of every pin against each other to determine if there's a hotspot, only some values in a hashmap that will (likely) be null.
- Moderation levels
    - There should be multiple levels of moderators. Right now, anyone who is an admin can ban users, remove photos, verify photos, etc. There should be multiple permission levels.
- Community Moderation
    - Users should be able to notify each other with notes on their posts. For example, a misidentified tree probably doesn't need to be flagged. Someone else could just press a button and the original poster gets a notification that another user thinks their submission may be incorrect.
    - Keeping administration out of these things will save Arboles Magicos time and let people feel like they're a greater part of the community.
- Events
    - Arboles Magicos should be able to create "Events" that send push notifications to users and show up as special markers on the map.
- Calendar Page?
    - It would be nice to see a calendar with events for when each tree flowers. This may be doable with just an embedded Google Calendar. Using a Google Calendar would also let users subscribe to the calendar and display it on their personal calendars.

## Known Issues
The following is a list of known issues with ArbolesMAUI, along with our thoughts on how they may be solved.

- Clearing filters doesn't replace pins on the map
    - We didn't have time to investigate this in detail. All of the reports remain in the DBManager's reportManagers Dictionary, so you should just be able to add every value back to the map.
    - We found that map pins don't get added if the addNewPin() method is not [executed on the main thread](#adding--removing-pins) (because we're changing the UI?). The same goes for removing pins, though, and applying filters seems to work fine. The addNewPin() and removePin() methods should also automatically elevate the call to the main thread.
- Auto Sign-In "works"
    - The current method for persisting a user's login session seems to work, but it could probably use some attention. See [Auto Sign-In](#auto-sign-in).
- NSFW Filtering is too strict
    - We didn't have time to tune the Cloudmersive Image Recognition API. Sometimes it says that non-NSFW images are NSFW, blocking upload. This isn't REALLY an issue, but it would be best to resolve this. See [Image Filtering](#image-filtering).
- White trees have pins that look blank
    - Maybe white tree pins get an outline? Maybe the tree icon is shown in black? There are many ways to solve this.
- You may find that the MAUI ListView is bugged. If you use a CollectionView, everything renders properly. 

### Publishing / Updating the App
Arboles Magicos is expected to handle publishing, maintaining, and updating ArbolesMAUI. Publishing an app to Google Play is free and easy. Publishing an app to the App App Store requires applying for permission and an Apple developer account. An Apple developer account costs $100/year. We recommend focusing on the Android version at first so that ArbolesMAUI can get some inexpensive user testing before putting the time, energy, and money required to get an app on the Apple App Store.
