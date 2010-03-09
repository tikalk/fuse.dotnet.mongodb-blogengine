#region Using

using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;

#endregion

public partial class widgets_RecentPosts_edit : WidgetEditBase
{
	protected void Page_PreRender(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			StringDictionary settings = GetSettings();
			if (settings.ContainsKey("numberofposts"))
				txtNumberOfPosts.Text = settings["numberofposts"];
			else
				txtNumberOfPosts.Text = "10";

            if (settings.ContainsKey("showcomments"))
                cbShowComments.Checked = settings["showcomments"].Equals("true", StringComparison.OrdinalIgnoreCase);
            else
                cbShowComments.Checked = true;

            if (settings.ContainsKey("showrating"))
                cbShowRating.Checked = settings["showrating"].Equals("true", StringComparison.OrdinalIgnoreCase);
            else
                cbShowRating.Checked = true;
		}
	}

	public override void Save()
	{
		StringDictionary settings = GetSettings();
		settings["numberofposts"] = txtNumberOfPosts.Text;
		settings["showcomments"] = cbShowComments.Checked.ToString();
		settings["showrating"] = cbShowRating.Checked.ToString();
		SaveSettings(settings);
		HttpRuntime.Cache.Remove("widget_recentposts");
	}
}
