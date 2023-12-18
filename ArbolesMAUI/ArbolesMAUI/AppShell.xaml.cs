using ArbolesMAUI.ViewModels;

namespace ArbolesMAUI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		ViewMediator.TabBar = tabBar;
        CultureUtil.LoadColorsFromDB();
    }
}
