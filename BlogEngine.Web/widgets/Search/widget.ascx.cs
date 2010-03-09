using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class widgets_Search_widget : WidgetBase
{

	/// <summary>
	/// Gets the name. It must be exactly the same as the folder that contains the widget.
	/// </summary>
	/// <value></value>
	public override string Name
	{
		get { return "Search"; }
	}

	/// <summary>
	/// Gets wether or not the widget can be edited.
	/// <remarks>
	/// The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
	/// </remarks>
	/// </summary>
	/// <value></value>
	public override bool IsEditable
	{
		get { return false; }
	}

	/// <summary>
	/// This method works as a substitute for Page_Load. You should use this method for
	/// data binding etc. instead of Page_Load.
	/// </summary>
	public override void LoadWidget()
	{
		// Nothing to load
	}

	/// <summary>
	/// Gets a value indicating if the header is visible. This only takes effect if the widgets isn't editable.
	/// </summary>
	/// <value><c>true</c> if [display header]; otherwise, <c>false</c>.</value>
	public override bool DisplayHeader
	{
		get { return false; }
	}
}
