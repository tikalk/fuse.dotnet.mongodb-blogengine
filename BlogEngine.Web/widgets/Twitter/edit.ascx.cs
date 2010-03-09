#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;

#endregion

public partial class widgets_Twitter_edit : WidgetEditBase
{
    private const string TWITTER_SETTINGS_CACHE_KEY = "twitter-settings";  // same key used in widget.ascx.cs.

	protected void Page_Load(object sender, EventArgs e)
	{
        if (!IsPostBack)
        { 
		    StringDictionary settings = GetSettings();
		    if (settings.ContainsKey("feedurl"))
		    {
			    txtUrl.Text = settings["feedurl"];
			    txtAccountUrl.Text = settings["accounturl"];
			    txtTwits.Text = settings["maxitems"];
                txtPolling.Text = settings["pollinginterval"];
                txtFollowMe.Text = settings["followmetext"];
		    }
        }
	}

	public override void Save()
	{
		StringDictionary settings = GetSettings();		
		settings["feedurl"] = txtUrl.Text;
		settings["accounturl"] = txtAccountUrl.Text;
		settings["maxitems"] = txtTwits.Text;
        settings["pollinginterval"] = txtPolling.Text;
        settings["followmetext"] = txtFollowMe.Text;
		SaveSettings(settings);

        // Don't need to clear Feed out of cache because when the Settings are cleared,
        // the last modified date (i.e. TwitterSettings.LastModified) will reset to
        // DateTime.MinValue and Twitter will be re-queried.
        HttpRuntime.Cache.Remove(TWITTER_SETTINGS_CACHE_KEY);
	}
}
