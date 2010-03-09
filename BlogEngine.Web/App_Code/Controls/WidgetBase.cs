#region Using

using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.Hosting;
using System.Threading;
using System.Xml;
using System.Text;
using BlogEngine.Core;
using BlogEngine.Core.DataStore;
using System.Xml.Serialization;
using System.Collections.Specialized;

#endregion

/// <summary>
/// Summary description for WidgetBase
/// </summary>
public abstract class WidgetBase : UserControl
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

	/// <summary>
	/// Gets the name. It must be exactly the same as the folder that contains the widget.
	/// </summary>
	public abstract string Name { get; }
    
    private string _Zone;
    /// <summary>
    /// Gets the name of the containing WidgetZone
    /// </summary>
    public string Zone
    {
        get { return _Zone; }
        set { _Zone = value; }
    }

	/// <summary>
	/// Gets wether or not the widget can be edited.
	/// <remarks>
	/// The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
	/// </remarks>
	/// </summary>
	public abstract bool IsEditable { get; }

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

	/// <summary>
	/// Gets a value indicating if the header is visible. This only takes effect if the widgets isn't editable.
	/// </summary>
	/// <value><c>true</c> if the header is visible; otherwise, <c>false</c>.</value>
	public virtual bool DisplayHeader
	{
		get { return true; }
	}

	#endregion

	/// <summary>
	/// This method works as a substitute for Page_Load. You should use this method for
	/// data binding etc. instead of Page_Load.
	/// </summary>
	public abstract void LoadWidget();

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

	#endregion

	/// <summary>
	/// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> 
	/// object, which writes the content to be rendered on the client.
	/// </summary>
	/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
	protected override void Render(HtmlTextWriter writer)
	{
		if (string.IsNullOrEmpty(Name))
			throw new NullReferenceException("Name must be set on a widget");
        
		StringBuilder sb = new StringBuilder();
        
		sb.Append("<div class=\"widget " + this.Name.Replace(" ", string.Empty).ToLowerInvariant() + "\" id=\"widget" + WidgetID + "\">");

		if (Thread.CurrentPrincipal.IsInRole(BlogSettings.Instance.AdministratorRole))
		{
            
			sb.Append("<a class=\"delete\" href=\"javascript:void(0)\" onclick=\"BlogEngine.widgetAdmin.removeWidget('" + WidgetID + "');return false\" title=\"" + Resources.labels.delete + " widget\">X</a>");
			//if (IsEditable)
				sb.Append("<a class=\"edit\" href=\"javascript:void(0)\" onclick=\"BlogEngine.widgetAdmin.editWidget('" + Name + "', '" + WidgetID + "');return false\" title=\"" + Resources.labels.edit + " widget\">" + Resources.labels.edit + "</a>");
                sb.Append("<a class=\"move\" href=\"javascript:void(0)\" onclick=\"BlogEngine.widgetAdmin.initiateMoveWidget('" + WidgetID + "');return false\" title=\"" + Resources.labels.move + " widget\">" + Resources.labels.move + "</a>");
		}

		if (ShowTitle)
			sb.Append("<h4>" + Title + "</h4>");
		else
			sb.Append("<br />");

		sb.Append("<div class=\"content\">");

		writer.Write(sb.ToString());
		base.Render(writer);
		writer.Write("</div>");
		writer.Write("</div>");
	}

}
