#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using BlogEngine.Core;

#endregion

public partial class widgets_TextBox_edit : WidgetEditBase
{

	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			StringDictionary settings = GetSettings();
			txtText.Text = settings["content"];
		}
	}

	/// <summary>
	/// Saves this the basic widget settings such as the Title.
	/// </summary>
	public override void Save()
	{
		StringDictionary settings = GetSettings();
		settings["content"] = txtText.Text;
		SaveSettings(settings);
	}
}
