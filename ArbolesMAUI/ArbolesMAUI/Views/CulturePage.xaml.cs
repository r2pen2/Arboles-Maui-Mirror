using ArbolesMAUI.DB;
using ArbolesMAUI.DB.ObjectManagers;
using ArbolesMAUI.Model;
using ArbolesMAUI.ViewModels;

namespace ArbolesMAUI.Views;

/**
 * Authored by Jared Chan
**/

public partial class CulturePage : ContentPage
{
    public CulturePage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); //remove navbar from top of screen
        FadePaletteIn();

        //hides appshell tab bar
        if (ViewMediator.UploadPageOpen)
        {
            Shell.SetNavBarIsVisible(this, false); 
            Shell.SetTabBarIsVisible(this, false);
        }

        //Dynamically resize palette height when view layout phase has finished
        this.SizeChanged += CalculatePaletteHeight;
    }

    private async void OnSearchButtonClicked(object sender, EventArgs e)
    {
        if(ViewMediator.MethodOfIdentification != "By color")
        {
            titleLabel.IsVisible = false;
            colorPicker.IsVisible = false;
            searchBar.IsVisible = true;
            cancelButton.IsVisible = true;
            // await titleLabel.FadeTo(0, 1000, Easing.Linear);
            // await colorPicker.FadeTo(0, 1000, Easing.Linear);
            // searchBar.Opacity = 0; 
            // await searchBar.FadeTo(1, 1000, Easing.Linear);
            // cancelButton.Opacity = 0; 
            // await cancelButton.FadeTo(1, 1000, Easing.Linear);

            // this is broken for Lex

            searchBar.Focus();
        }
    }

    private async void OnCancelButtonClicked(object sender, EventArgs e)
    {
        colorPicker.IsVisible = true;
        titleLabel.IsVisible = true;
        searchBar.IsVisible = false; 
        cancelButton.IsVisible = false;
        // await searchBar.FadeTo(0, 1000, Easing.Linear);
        // await cancelButton.FadeTo(0, 1000, Easing.Linear);
        // await titleLabel.FadeTo(1, 1000, Easing.Linear);
        // await colorPicker.FadeTo(1, 1000, Easing.Linear);

        searchBar.Unfocus();
    }

    private async void OnColorPickerClicked(object sender, EventArgs e)
    {
        FadePaletteIn();
    }

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

    private async void OnSelectSpeciesClicked(object sender, SelectionChangedEventArgs e)
    {

        TreeManager tree = (TreeManager)e.CurrentSelection.First();
        if (ViewMediator.UploadPageOpen) ViewMediator.SpeciesColorBlock.BackgroundColor = Color.FromArgb(tree.PaletteGroupHex);
        
        ViewMediator.CulturePage = this;
        await Navigation.PushAsync(new TreeInfoPage(tree));
    }

    private async void FadePaletteIn()
    {
        palette.Opacity = 0;
        await palette.FadeTo(1, 1000, Easing.CubicIn);
    }

    private void CalculatePaletteHeight(object sender, EventArgs e)
    {
        double colorViewHeight = root.Height - topBar.Height;
        palette.HeightRequest = colorViewHeight;
    } 
  
}