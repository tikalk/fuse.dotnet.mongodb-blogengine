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
using System.Collections.Specialized;

public partial class widgets_Most_comments_edit : WidgetEditBase
{

	protected override void OnPreRender(EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			txtNumber.Text = "3";
			txtSize.Text = "50";
			txtDays.Text = "60";
			cbShowComments.Checked = true;

			StringDictionary settings = GetSettings();
			if (settings.ContainsKey("avatarsize"))
				txtSize.Text = settings["avatarsize"];

			if (settings.ContainsKey("numberofvisitors"))
				txtNumber.Text = settings["numberofvisitors"];

			if (settings.ContainsKey("days"))
				txtDays.Text = settings["days"];

			if (settings.ContainsKey("showcomments"))
				cbShowComments.Checked = settings["showcomments"].Equals("true", StringComparison.OrdinalIgnoreCase);
		}
	}

	public override void Save()
	{
		StringDictionary settings = GetSettings();
		settings["avatarsize"] = txtSize.Text;
		settings["numberofvisitors"] = txtNumber.Text;
		settings["days"] = txtDays.Text;
		settings["showcomments"] = cbShowComments.Checked.ToString();
		SaveSettings(settings);

		Cache.Remove("most_comments");
	}
}
