#region Using

using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;

#endregion

public partial class widgets_RecentComments_edit : WidgetEditBase
{
	protected void Page_PreRender(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			StringDictionary settings = GetSettings();
			if (settings.ContainsKey("numberofcomments"))
				txtNumberOfPosts.Text = settings["numberofcomments"];
			else
				txtNumberOfPosts.Text = "10";
		}
	}

	public override void Save()
	{
		StringDictionary settings = GetSettings();
		settings["numberofcomments"] = txtNumberOfPosts.Text;
		SaveSettings(settings);
		HttpRuntime.Cache.Remove("widget_recentcomments");
	}
}
