using ArbolesMAUI.DB;
using CommunityToolkit.Maui.Views;
using MauiLocalization.Resources.Localization;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace ArbolesMAUI.Views;

public partial class AccountPage : ContentPage
{
    string usernameLabelText = "Username: ";
    string emailLabelText = "Email: ";
    string userSinceLabelText = "User since: ";

    string emailText = "";
    string passwordText = "";
	
    public AccountPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        if (DBManager.currentUserManager != null) {
    		HideLoginPage();
        } else {
            ShowLoginPage();
        }

        if (LocalizationResourceManager.Instance.Culture.Name.ToLower() == "es-cr")
        {
            usernameLabelText = "Nombre de usuario: ";
            emailLabelText = "Correo electrónico: ";
            userSinceLabelText = "Usuario desde: ";
        }
    }

    protected override void OnAppearing() {
        base.OnAppearing();

        if (DBManager.currentUserManager != null) {
            HideLoginPage();
        } else {
            ShowLoginPage();
        }
    }

    void ShowLoginPage() {
        accountDetailsFrame.IsVisible = false;
        accountSettingsButton.IsVisible = false;
        loginForm.IsVisible = true;
        LogOutButton.IsVisible = false;
    }

    void HideLoginPage() {
        accountDetailsFrame.IsVisible = true;
        accountSettingsButton.IsVisible = true;
        signUpForm.IsVisible = false;
        loginForm.IsVisible = false;
        LogOutButton.IsVisible = true;
        LoadUserData();
    }

	async void LoadUserData() {
        await DBManager.currentUserManager.fetchData();
		emailLabel.Text = emailLabelText + DBManager.currentUserManager.getMail();
		usernameLabel.Text = usernameLabelText + DBManager.currentUserManager.getName();
		userSinceLabel.Text = userSinceLabelText + getCreateDateString();
	}

	string getCreateDateString() {
		DateTime date = DBManager.currentUserManager.getcreatedAt().ToDateTime();
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

    void switchToSignup() {
        loginForm.IsVisible = false;
        signUpForm.IsVisible = true;
    }

    void switchToLogin() {
        loginForm.IsVisible = true;
        signUpForm.IsVisible = false;
    }

    async void OnLoginButtonClicked(object sender, EventArgs e) {
        await DBManager.authClient.SignInWithRedirectAsync(Firebase.Auth.FirebaseProviderType.Google, async googleAuthUri => {

            try {
                WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new Uri(googleAuthUri),
                    new Uri("ArbolesMAUI://"));

                return authResult?.AccessToken;

                // Do something with the token
            } catch (TaskCanceledException e) {
                Console.WriteLine(e.Message);
                // Use stopped auth
            }
            return "";
        });
    }

    async void SignOutUser(object sender, EventArgs e) {
        await DBManager.setCurrentUser(null, null);
        await Task.Run(() => SecureStorage.Default.Remove("email"));
        await Task.Run(() => SecureStorage.Default.Remove("password"));
        string loginFilePath = FileSystem.Current.CacheDirectory + "/loggedIn.txt";
        File.Delete(loginFilePath);
        DBManager.authClient.SignOut();
        switchToLogin();
        ShowLoginPage();
    }

    void OnSignupButtonClicked(object sender, EventArgs e) {
        switchToSignup();
    }

    void OnEsClicked(object sender, EventArgs args) {
        LocalizationResourceManager.Instance.Culture = new CultureInfo("es-cr"); //CultureInfo must take in a BCP 47 Language Tag
    }
    void OnEnClicked(object sender, EventArgs args) {
        LocalizationResourceManager.Instance.Culture = new CultureInfo("en-us"); //CultureInfo must take in a BCP 47 Language Tag
    }

    

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
                HideLoginPage();
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
                HideLoginPage();
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

    private void changeLanguageButtonClicked(object sender, EventArgs e)
    {
        string culture;
        //Application.Current.MainPage.ShowPopup(new Settings());
        if (LocalizationResourceManager.Instance.Culture.Name.ToLower().Equals("en-us")) culture = "es-cr";
        else culture = "en-us";
        Application.Current.MainPage = new LoginPage();
        LocalizationResourceManager.Instance.Culture = new CultureInfo(culture); //CultureInfo must take in a BCP 47 Language Tag
    }

}