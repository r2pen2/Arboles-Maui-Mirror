namespace MauiLocalization.Resources.Localization;

using ArbolesMAUI.ViewModels;
using System.ComponentModel;
using System.Globalization;

/**
 * Authored by Jared Chan
 **/

/// <summary>
/// Class stores a singleton instance for retrieving and setting the current culture (language: spanish/english)
/// </summary>
public class LocalizationResourceManager : INotifyPropertyChanged
{
	private LocalizationResourceManager()
	{
		AppResources.Culture = CultureInfo.CurrentCulture;
	}

    /// <summary>
    /// Singleton class
    /// </summary>
	public static LocalizationResourceManager Instance { get; } = new();

	public object this[string resourceKey] =>
		AppResources.ResourceManager.GetObject(resourceKey, AppResources.Culture) ?? Array.Empty<byte>();

	public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Getter/setter for currently set culture
    /// </summary>
    public CultureInfo Culture
    {
        get
        {
			return AppResources.Culture;
        }
        set 
		{
            AppResources.Culture = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}