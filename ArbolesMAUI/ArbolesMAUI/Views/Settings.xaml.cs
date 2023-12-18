using CommunityToolkit.Maui.Views;
using Java.Security;
using MauiLocalization.Resources.Localization;
using System.Globalization;

namespace ArbolesMAUI.Views;

/**
 * Authored by Jared Chan
**/

public partial class Settings : Popup
{
    private int langIndexInitialState;
    private string culture;
	public Settings()
	{
		InitializeComponent();

        //Initialize language options
        LangPicker.Items.Add("English version");
        LangPicker.Items.Add("Versión en español");

        //check which language is currently set & set picker accordingly
        if (LocalizationResourceManager.Instance.Culture.Name.ToLower() == "en-us") LangPicker.SelectedIndex = 0;
        else LangPicker.SelectedIndex = 1;

        langIndexInitialState = LangPicker.SelectedIndex;  
    }

    private void LanguageChanged(object sender, EventArgs e)
    {
        if (LangPicker.SelectedIndex == 1) culture = "es-cr";
        else culture = "en-us";
    }

    private void OnCloseClicked(object sender, EventArgs e) => Close();

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if(langIndexInitialState != LangPicker.SelectedIndex)
        {
            string title = "Warning";
            string description = "Due to a change in language, you will be taken back to the login screen.";
            string confirm = "Confirm";
            string cancel = "Cancel";

            if (LocalizationResourceManager.Instance.Culture.Name.ToLower() == "es-cr")
            {
                title = "Advertencia";
                description = "Debido a un cambio de idioma, volverá a la pantalla de inicio de sesión.";
                confirm = "Confirmar";
                cancel = "Cancelar";
            }

            //if language changed, display an alert that user will be returned to login screen
            bool response = await Application.Current.MainPage.DisplayAlert(title, description, confirm, cancel);
           
            //if user answers alert with "yes" response, proceed to login screen and set new language
            if (response)
            {
                Application.Current.MainPage = new LoginPage();
                LocalizationResourceManager.Instance.Culture = new CultureInfo(culture); //CultureInfo must take in a BCP 47 Language Tag
            }
        }
        Close();
    }
}
