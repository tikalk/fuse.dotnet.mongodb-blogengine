#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;

#endregion

public partial class widgets_Tag_cloud_edit : WidgetEditBase
{

	protected override void OnLoad(EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			StringDictionary settings = GetSettings();
			string minimumPosts = "1";
			if (settings.ContainsKey("minimumposts"))
				minimumPosts = settings["minimumposts"];

			ddlNumber.SelectedValue = minimumPosts;
		}
	}

	public override void Save()
	{
		StringDictionary settings = GetSettings();
		settings["minimumposts"] = ddlNumber.SelectedValue;
		SaveSettings(settings);
		widgets_Tag_cloud_widget.Reload();
	}
}
