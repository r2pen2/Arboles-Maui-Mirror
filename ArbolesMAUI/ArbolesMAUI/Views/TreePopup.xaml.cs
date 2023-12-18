using Android.Content.Res;
using ArbolesMAUI.DB;
using ArbolesMAUI.DB.ObjectManagers;
using ArbolesMAUI.ViewModels;
using CommunityToolkit.Maui.Views;
using MauiLocalization.Resources.Localization;
using System.ComponentModel;

namespace ArbolesMAUI.Views;

/**
 * Authored by Jared Chan, Joe Dobbelaar, and Cole Parks
**/

public partial class TreePopup : Popup
{
	ReportManager report;
	int contributionCursor;
    public TreePopup(ReportManager r)
    {
        InitializeComponent();
        if (ViewMediator.UploadPageOpen) {
            deleteButton.IsVisible = false;
            UseThisTreeButton.IsVisible = true;
            ViewMediator.UsePinAsLocation = false;
        }
        else
        {
            UseThisTreeButton.IsVisible = false;
            deleteButton.IsVisible = DBManager.currentUserManager.getIsAdmin();
        }
        
        this.report = r;
        contributionCursor = 0;
        updateContribution();
        LeftButton.IsEnabled = false;
        RightButton.IsEnabled = (r.Contributions.Count > 1);

        if (!string.IsNullOrWhiteSpace(report.TreeId))
        {
            foreach(TreeManager tree in CultureUtil.Instance.TreeManagers)
            {
                if (report.TreeId == tree.documentId) treeNameLabel.Text = tree.NameAutoTranslation;
            }
        }
    }

    


    void OnCloseButtonClicked(object sender, EventArgs e) => Close();

    //public event PropertyChangedEventHandler? UseThisTreeChosen;

    void OnUseThisTreeClicked(object sender, EventArgs e)
    {
        
        //the actual report object should be set in ViewMediator
        ViewMediator.UsePinAsLocation = true;
        //** Fires event to use location tapped
        ViewMediator.SetLocationAsPin();
        //ViewMediator.SetSpeciesColorBlockToPin();

        //UseThisTreeChosen?.Invoke(this, new PropertyChangedEventArgs(null));



        Close();
    }

    async void OnDeleteClick(object sender, EventArgs e) {
        await Task.Run(() => {
            this.report.removeContribution(this.report.Contributions[contributionCursor]);
        });
		Close();
	}

	void OnLeftClick(object sender, EventArgs e) {
		if (this.contributionCursor > 0) {
            RightButton.IsEnabled = true;


            this.contributionCursor -= 1;
            LeftButton.IsEnabled = (this.contributionCursor > 0);
            updateContribution();
        }
    }

	void OnRightClick(object sender, EventArgs e) {
		if (this.contributionCursor + 1 < this.report.Contributions.Count) {
            this.contributionCursor += 1;
            LeftButton.IsEnabled = true;

            RightButton.IsEnabled = (this.contributionCursor + 1 < this.report.Contributions.Count);

            updateContribution();
        }
    }

    void updateContribution() {
        if (ViewMediator.UploadPageOpen)
        {
            deleteButton.IsVisible = DBManager.currentUserManager.getIsAdmin() || this.report.Contributions[contributionCursor].ContributorId == DBManager.currentUserManager.documentId;
        }

        if (!this.report.Contributions[contributionCursor].Verified) {
            if (this.report.Contributions[contributionCursor].Flagged) {
                flagImage.Source = ImageSource.FromFile("flag_red.png");
            } else {
                flagImage.Source = ImageSource.FromFile("flag.svg");
            }
        } else {
            flagImage.Source = ImageSource.FromFile("tab_cultre.svg");
        }

        CurrentImage.Source = ImageSource.FromUri(new Uri(this.report.Contributions[contributionCursor].PhotoURL));

        string uploadedByText = "Uploaded by: ";
        if (LocalizationResourceManager.Instance.Culture.Name.ToLower() == "es-cr") uploadedByText = "Subido por: ";
        CurrentContributorName.Text = uploadedByText + this.report.Contributions[contributionCursor].ContributorName;
    }

    void handleFlagClick(object sender, EventArgs e) {
        if (!report.Contributions[contributionCursor].Verified) {
            this.report.Contributions[contributionCursor].Flagged = true;
            flagImage.Source = ImageSource.FromFile("flag_red.png");
            this.report.push();
        }
    }

}