using System;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;
using Resources;

public partial class admin_Comments_Menu : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        BuildMenuList();
    }

    protected void BuildMenuList()
    {
        string cssClass = "";
        string tmpl = "<a href=\"{0}.aspx\" class=\"{1}\"><span>{2}</span></a>";

        HtmlGenericControl inbx = new HtmlGenericControl("li");
        cssClass = Request.Path.ToLower().Contains("default.aspx") ? "current" : "";
        inbx.InnerHtml = string.Format(tmpl, "Default", cssClass, labels.inbox);

        HtmlGenericControl appr = new HtmlGenericControl("li");
        cssClass = Request.Path.ToLower().Contains("approved.aspx") ? "current" : "";
        appr.InnerHtml = string.Format(tmpl, "Approved", cssClass, labels.approved);

        HtmlGenericControl spm = new HtmlGenericControl("li");
        cssClass = Request.Path.ToLower().Contains("spam.aspx") ? "current" : "";
        spm.InnerHtml = string.Format(tmpl, "Spam", cssClass, labels.spam);

        HtmlGenericControl stn = new HtmlGenericControl("li");
        cssClass = Request.Path.ToLower().Contains("settings.aspx") ? "current" : "";
        stn.InnerHtml = string.Format(tmpl, "Settings", cssClass, labels.configuration);

        UlMenu.Controls.Add(inbx);

        if(BlogSettings.Instance.EnableCommentsModeration && BlogSettings.Instance.IsCommentsEnabled)
        {
            if(BlogSettings.Instance.ModerationType == 1)
            {                  
                hdr.InnerHtml = labels.comments + ": " + labels.automoderation;
                UlMenu.Controls.Add(spm);
            }
            else
            {
                hdr.InnerHtml = string.Format("{0}: {1} {2}", labels.comments, labels.manual, labels.moderation);
                UlMenu.Controls.Add(appr);
            }
        }
        else
        {
            hdr.InnerHtml = labels.comments + ": " + labels.unmoderated;
        }

        UlMenu.Controls.Add(stn);

        if(Request.Path.ToLower().Contains("settings.aspx"))
        {
            hdr.InnerHtml = labels.comments + ": " + labels.settings;
        }
    }
}