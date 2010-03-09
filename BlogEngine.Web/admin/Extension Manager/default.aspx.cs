using System;
using System.Web;
using System.Web.UI;

public partial class User_controls_xdashboard_Default : System.Web.UI.Page
{
    /// <summary>
    /// Handles page load, loading control
    /// based on query string parameter
    /// </summary>
    /// <param name="sender">Page</param>
    /// <param name="e">Event args</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        string ctrlToLoad = string.Empty;
        UserControl uc = null;

        switch (Request.QueryString["ctrl"])
        {
          case "params":
            string xName = Request.QueryString["ext"].ToString();

            foreach (ManagedExtension x in ExtensionManager.Extensions)
            {
              if (x.Name == xName)
              {
                foreach (ExtensionSettings setting in x.Settings)
                {
                  if (!string.IsNullOrEmpty(setting.Name) && !setting.Hidden)
                  {
                    uc = (UserControl)Page.LoadControl("Settings.ascx");
                    uc.ID = setting.Name;
                    ucPlaceHolder.Controls.Add(uc);
                  }
                }
              }
            }
            break;
          case "editor":
              uc = (UserControl)Page.LoadControl("Editor.ascx");
              ucPlaceHolder.Controls.Add(uc);
              break;
          default:
              uc = (UserControl)Page.LoadControl("Extensions.ascx");
              ucPlaceHolder.Controls.Add(uc);
              break;
        }
    }
}