using ArbolesMAUI.DB.ObjectManagers;
using ArbolesMAUI.ViewModels;
using Timer = System.Timers.Timer;

namespace ArbolesMAUI.Views;

/**
 * Authored by Jared Chan
**/

public partial class TreeInfoPage : ContentPage
{
    private static Timer aTimer;
    private int numImages;
    public TreeInfoPage(TreeManager tree)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = tree;

        //get the number of images available for this tree species
        //number used to cycle images in slideshow via modulo
        numImages = ((TreeManager)BindingContext).Images.Count;

        //if accessing this page within upload or filter page, set the button to select this species as visible
        if (ViewMediator.UploadPageOpen || ViewMediator.IsFilterPageOpen) selectButton.IsVisible = true;
        List<Label> labels = MakeWeeksLayout();
        foreach (Label l in labels)
        {
            FloweringWeeksList.Children.Add(l);
        }
        //start the timer for switching slideshow images
        StartTimer();

	}

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
		await Navigation.PopAsync();
    }

	private async void OnSelectButtonClicked(object sender, EventArgs e)
	{
        //set select species button visible if within upload or filter page
        if (ViewMediator.UploadPageOpen || ViewMediator.IsFilterPageOpen)
        {
            if(ViewMediator.UploadPageOpen)
                ViewMediator.SelectSpeciesButton.Text = ((TreeManager)BindingContext).NameAutoTranslation;
            else if(ViewMediator.IsFilterPageOpen)
                ViewMediator.FilterSpeciesButton.Text = ((TreeManager)BindingContext).NameAutoTranslation;

            //remove the culture/palette page and directly pop back to the filter/upload page
            Navigation.RemovePage(ViewMediator.CulturePage);
            await Navigation.PopAsync();
        }
    }

    /// <summary>
    /// Initialize the timer for switching slideshow images
    /// </summary>
    private void StartTimer()
    {
        // Create a timer and set a two second interval.
        aTimer = new System.Timers.Timer
        {
            Interval = 5000
        };

        // Hook up the Elapsed event for the timer. 
        aTimer.Elapsed += OnTimedEvent;

        // Have the timer fire repeated events (true is the default)
        aTimer.AutoReset = true;

        // Start the timer
        aTimer.Enabled = true;
    }

    /// <summary>
    /// Changes current item of carouselview after set time has elapsed
    /// </summary>
    private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
    {
        int index = carouselView.Position + 1;
        carouselView.CurrentItem = ((TreeManager)BindingContext).Images[(index % numImages)];
    }

    void SeedDispersion_Clicked(System.Object sender, System.EventArgs e)
    {
        

    }

    void FlowerDimesions_Clicked(System.Object sender, System.EventArgs e)
    {
        
    }

    void TreeHeight_Clicked(System.Object sender, System.EventArgs e)
    {
        

    }

    List<Label> MakeWeeksLayout()
    {
        TreeManager tree = ((TreeManager)BindingContext);
        List<Label> List = new();

        for (int i = 0; i < 47; i++)
        {
            Label week = new Label();
            week.StyleId = i.ToString();
            week.WidthRequest = 8;
            
            if (tree.FloweringWeeks.Contains(i))
            {
                week.BackgroundColor = Color.FromArgb("c1ca6e");
            }
            else

            {
                week.BackgroundColor = Color.FromArgb("00000000");
            }
            List.Add(week);
        }
        return List;
    }
}