#region Using

using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;

#endregion

public partial class widgets_TextBox_widget : WidgetBase
{

	/// <summary>
	/// This method works as a substitute for Page_Load. You should use this method for
	/// data binding etc. instead of Page_Load.
	/// </summary>
	public override void LoadWidget()
	{
		StringDictionary settings = GetSettings();
		if (settings.ContainsKey("content"))
		{
			LiteralControl text = new LiteralControl(settings["content"]);
			this.Controls.Add(text);
		}
	}

	/// <summary>
	/// Gets the name. It must be exactly the same as the folder that contains the widget.
	/// </summary>
	/// <value></value>
	public override string Name
	{
		get { return "TextBox"; }
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
		get { return true; }
	}

}
