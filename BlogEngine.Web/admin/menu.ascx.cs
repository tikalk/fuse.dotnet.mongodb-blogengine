using System;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core.Providers;
using BlogEngine.Core;

public partial class admin_menu : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsCallback)
			BindMenu();
	}

	private void BindMenu()
	{
		SiteMapNode root = SiteMap.Providers["SecuritySiteMap"].RootNode;
		if (root != null)
		{
			foreach (SiteMapNode adminNode in root.ChildNodes)
			{
				if (adminNode.IsAccessibleToUser(HttpContext.Current))
				{
					if (!Request.RawUrl.ToUpperInvariant().Contains("/ADMIN/") && (adminNode.Url.Contains("xmanager") || adminNode.Url.Contains("PingServices")))
						continue;

					HtmlAnchor a = new HtmlAnchor();
					a.HRef = adminNode.Url;

                    a.InnerHtml = "<span>" + Utils.Translate(adminNode.Title, adminNode.Title) + "</span>";//"<span>" + Utils.Translate(info.Name.Replace(".aspx", string.Empty)) + "</span>";
					if (Request.RawUrl.IndexOf(adminNode.Url, StringComparison.OrdinalIgnoreCase) != -1)
						a.Attributes["class"] = "current";

                    // if "page" has its own subfolder (comments, extensions) should 
                    // select parent tab when navigating through child tabs
                    if (adminNode.Url.IndexOf("/admin/pages/", StringComparison.OrdinalIgnoreCase) == -1 && SubUrl(Request.RawUrl) == SubUrl(adminNode.Url))
                        a.Attributes["class"] = "current";

					HtmlGenericControl li = new HtmlGenericControl("li");
					li.Controls.Add(a);
					ulMenu.Controls.Add(li);
				}
			}
		}

		if (!Request.RawUrl.ToUpperInvariant().Contains("/ADMIN/"))
			AddItem(Resources.labels.changePassword, Utils.RelativeWebRoot + "login.aspx");
	}

	public void AddItem(string text, string url)
	{
		HtmlAnchor a = new HtmlAnchor();
		a.InnerHtml = "<span>" + text + "</span>";
		a.HRef = url;

		HtmlGenericControl li = new HtmlGenericControl("li");
		li.Controls.Add(a);
		ulMenu.Controls.Add(li);
	}

    private string SubUrl(string url)
    {
        int i = url.LastIndexOf("/");

        return (i > 0) ? url.Substring(0, i) : "";
    }
}
