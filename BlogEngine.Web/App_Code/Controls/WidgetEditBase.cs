#region Using

using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using BlogEngine.Core;
using BlogEngine.Core.DataStore;
using System.Collections.Specialized;

#endregion

/// <summary>
/// Summary description for WidgetBase
/// </summary>
public abstract class WidgetEditBase : UserControl
{
	#region Properties

  private string _Title;
	/// <summary>
	/// Gets or sets the title of the widget. It is mandatory for all widgets to set the Title.
	/// </summary>
	/// <value>The title of the widget.</value>
	public string Title
	{
		get { return _Title; }
		set { _Title = value; }
	}

	private bool _ShowTitle;
	/// <summary>
	/// Gets or sets a value indicating whether [show title].
	/// </summary>
	/// <value><c>true</c> if [show title]; otherwise, <c>false</c>.</value>
	public bool ShowTitle
	{
		get { return _ShowTitle; }
		set { _ShowTitle = value; }
	}

	private Guid _WidgetID;
	/// <summary>
	/// Gets the widget ID.
	/// </summary>
	/// <value>The widget ID.</value>
	public Guid WidgetID
	{
		get { return _WidgetID; }
		set { _WidgetID = value; }
	}

	#endregion

	/// <summary>
	/// Saves this the basic widget settings such as the Title.
	/// </summary>
	public abstract void Save();

  #region Settings

  /// <summary>
  /// Get settings from data store
  /// </summary>
  /// <returns>Settings</returns>
  public StringDictionary GetSettings()
  {
    string cacheId = "be_widget_" + WidgetID;
    if (Cache[cacheId] == null)
    {
      WidgetSettings ws = new WidgetSettings(WidgetID.ToString());
      Cache[cacheId] = (StringDictionary)ws.GetSettings();
    }
    return (StringDictionary)Cache[cacheId];
  }

  /// <summary>
  /// Saves settings to data store
  /// </summary>
  /// <param name="settings">Settings</param>
  protected virtual void SaveSettings(StringDictionary settings)
  {
    string cacheId = "be_widget_" + WidgetID;

    WidgetSettings ws = new WidgetSettings(WidgetID.ToString());
    ws.SaveSettings(settings);
    
    Cache[cacheId] = settings;
  }

  #endregion

  public static event EventHandler<EventArgs> Saved;
	/// <summary>
	/// Occurs when the class is Saved
	/// </summary>
	public static void OnSaved()
	{
		if (Saved != null)
		{
			Saved(null, new EventArgs());
		}
	}

}
