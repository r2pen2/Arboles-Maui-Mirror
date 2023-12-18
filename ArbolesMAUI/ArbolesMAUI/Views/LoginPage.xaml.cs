using ArbolesMAUI.DB;
using ArbolesMAUI.ViewModels;
using Firebase.Auth;
using MauiLocalization.Resources.Localization;
using Plugin.LocalNotification;
using System.Globalization;

namespace ArbolesMAUI.Views;

public partial class LoginPage : ContentPage
{
    string loginFilePath = FileSystem.Current.CacheDirectory + "/loggedIn.txt";
    bool loggedIn;

	public LoginPage()
	{
        CultureUtil.LoadTreesFromDB();
        loggedIn = File.Exists(loginFilePath);
        tryLogin();
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        MapUtil.LoadAllPinsFromDB();

        loginForm.IsVisible = !loggedIn;
        spinner.IsVisible = loggedIn;

        this.Appearing += RequestNotifPerm;
    }

    private async void RequestNotifPerm(object sender, EventArgs e)
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }
    }
    async void tryLogin() {
        if (!loggedIn) {
            return;
        }
        string email = await SecureStorage.Default.GetAsync("email");
        string pass = await SecureStorage.Default.GetAsync("password");
        if (email != null && pass != null) {
            try {
                Firebase.Auth.UserCredential newUser = await DBManager.authClient.SignInWithEmailAndPasswordAsync(email, pass);
                if (newUser != null) {
                    await DBManager.setCurrentUser(newUser);
                    File.Create(loginFilePath);
                    Application.Current.MainPage = new NavigationPage(new AppShell());
                }
            } catch (Exception ex) {
                // Some error
                Console.WriteLine(ex.Message);
            }
        }
    }

    void switchToSignup() {
        // loginForm.FadeTo(0, 500, Easing.SinIn);
        // loginLogo.FadeTo(0, 500, Easing.SinIn);
        // signUpForm.FadeTo(1, 500, Easing.SinIn);
        // signUpLogo.FadeTo(1, 500, Easing.SinIn);

        loginForm.IsVisible = false;
        loginLogo.IsVisible = false;
        signUpForm.IsVisible = true;
        signUpLogo.IsVisible = true;
    }

    void switchToLogin() {
        loginForm.IsVisible = true;
        loginLogo.IsVisible = true;
        signUpForm.IsVisible = false;
        signUpLogo.IsVisible = false;
    }

    void OnGuestButtonClicked(object sender, EventArgs e) {
        Application.Current.MainPage = new AppShell();
    }

    void OnSignupButtonClicked(object sender, EventArgs e) {
        switchToSignup();
    }

    void OnEsClicked(object sender, EventArgs args)
    {
        LocalizationResourceManager.Instance.Culture = new CultureInfo("es-cr"); //CultureInfo must take in a BCP 47 Language Tag
    }
    void OnEnClicked(object sender, EventArgs args)
    {
        LocalizationResourceManager.Instance.Culture = new CultureInfo("en-us"); //CultureInfo must take in a BCP 47 Language Tag
    }

    string emailText = "";
    string passwordText = "";

    bool LoginEntryValid() {
        return (emailText.Length > 0) && (passwordText.Length > 0) && (emailText.Contains("@"));
    }

    private void OnEmailEntryChanged(object sender, TextChangedEventArgs e) {
        emailText = e.NewTextValue.Replace(" ", "");
        LoginButton.IsEnabled = LoginEntryValid();
    }

    private void OnEmailEntryCompleted(object sender, EventArgs e) {
        emailText = ((Entry)sender).Text.Replace(" ", "");
        LoginButton.IsEnabled = LoginEntryValid();
    }

    private void OnPasswordEntryChanged(object sender, TextChangedEventArgs e) {
        passwordText = e.NewTextValue;
        LoginButton.IsEnabled = LoginEntryValid();
    }
    private void OnPasswordEntryCompleted(object sender, EventArgs e) {
        passwordText = ((Entry)sender).Text;
        LoginButton.IsEnabled = LoginEntryValid();
    }

    private async void OnLoginClicked(object sender, EventArgs e) {
        try {
            Firebase.Auth.UserCredential newUser = await DBManager.authClient.SignInWithEmailAndPasswordAsync(emailText, passwordText);
            if (newUser != null) {
                await DBManager.setCurrentUser(newUser);
                await Task.Run(() => SecureStorage.Default.SetAsync("email", emailText));
                await Task.Run(() => SecureStorage.Default.SetAsync("password", passwordText));
                File.Create(loginFilePath);
                Application.Current.MainPage = new NavigationPage(new AppShell());
            }
        } catch (Exception ex) {
            // Some error
            Console.WriteLine(ex.Message);
            LoginErrorLabel.IsVisible = true;
        }
    }



    string usernameText = "";
    string verifyText = "";

    private void OnUsernameEntryChanged(object sender, TextChangedEventArgs e) {
        usernameText = e.NewTextValue;
        SubmitButton.IsEnabled = credentialsValid();
    }

    private void OnUsernameEntryCompleted(object sender, EventArgs e) {
        usernameText = ((Entry)sender).Text;
        SubmitButton.IsEnabled = credentialsValid();
    }

    private void OnVerifyEntryChanged(object sender, TextChangedEventArgs e) {
        verifyText = e.NewTextValue;
        SubmitButton.IsEnabled = credentialsValid();
    }
    private void OnVerifyEntryCompleted(object sender, EventArgs e) {
        verifyText = ((Entry)sender).Text;
        SubmitButton.IsEnabled = credentialsValid();
    }

    private bool credentialsValid() {
        bool emailValid = emailText.Length > 0 && emailText.Contains("@");
        bool passwordValid = passwordText.Length > 6 && verifyText.Length > 6 && passwordText.Equals(verifyText);
        bool usernameValid = usernameText.Length > 0;
        return emailValid && passwordValid && usernameValid;
    }

    private async void OnSubmitClicked(object sender, EventArgs e) {
        if (!credentialsValid()) {
            return;
        }
        try {
            Firebase.Auth.UserCredential newUser = await DBManager.authClient.CreateUserWithEmailAndPasswordAsync(emailText, passwordText, usernameText);
            if (newUser != null) {
                DB.ObjectManagers.UserManager newUserManager = DBManager.getUserManager(newUser.User.Uid);
                newUserManager.setName(usernameText);
                newUserManager.setMail(emailText);
                newUserManager.setIsAdmin(false);
                newUserManager.setCreatedAt(Google.Cloud.Firestore.Timestamp.FromDateTime(newUser.User.Credential.Created.ToUniversalTime()));
                await newUserManager.push();
                await DBManager.setCurrentUser(newUser, newUserManager);
                File.Create(loginFilePath);
                Application.Current.MainPage = new AppShell();
                
            }
        } catch (Exception ex) {
            // User probably already exists
            Console.WriteLine(ex.Message);
            Application.Current.MainPage = new LoginPage();
        }
    }

    private void OnBackButtonClicked(object sender, EventArgs args) {
        switchToLogin();
    }
}
