using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using BlogEngine.Core.Web.Controls;

public partial class UserControlsXmanagerExtensionsList : System.Web.UI.UserControl
{
  #region Private members
    
  const string confirm = "The website will be unavailable for a few seconds. Are you sure you wish to continue?";
  const string jsOnClick = "onclick=\"if (confirm('" + confirm + "')) { window.location.href = this.href } return false;\"";
  const string clickToEnable = "Click to enable ";
  const string clickToDisable = "Click to disable ";
  const string enabled = "Enabled";
  const string disabled = "Disabled";
    
  #endregion

  /// <summary>
  /// handles page load event
  /// </summary>
  /// <param name="sender">Page</param>
  /// <param name="e">Arguments</param>
  protected void Page_Load(object sender, EventArgs e)
  {
    lblErrorMsg.InnerHtml = string.Empty;
    lblErrorMsg.Visible = false;
    btnRestart.Visible = false;

    object act = Request.QueryString["act"];
    object ext = Request.QueryString["ext"];

    if (act != null && ext != null)
    {
      ChangeStatus(act.ToString(), ext.ToString());
    }

    if(!Page.IsPostBack)
    {
        List<ManagedExtension> extensions = new List<ManagedExtension>();

        foreach (ManagedExtension x in ExtensionManager.Extensions)
        {
            if(x.Name != "MetaExtension") extensions.Add(x);
        }
        
        // remove system meta extension from the list
        //extensions.Remove(extensions.Find(delegate(ManagedExtension x) { return x.Name == "MetaExtension"; }));

        extensions.Sort(delegate(ManagedExtension e1, ManagedExtension e2)
        {
            if (e1.Priority == e2.Priority)
                return string.CompareOrdinal(e1.Name, e2.Name);
            return e1.Priority.CompareTo(e2.Priority);
        });

        gridExtensionsList.DataSource = extensions;
        gridExtensionsList.DataBind();
    }

    btnRestart.Click += BtnRestartClick;
  }

  /// <summary>
  /// Test stuff - ignore for now
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  void BtnRestartClick(object sender, EventArgs e)
  {
    // This short cercuits the IIS process. Need to find a better way to restart the app.
    //ThreadPool.QueueUserWorkItem(delegate { ForceRestart(); });
    //ThreadStart threadStart = delegate { ForceRestart(); };
    //Thread thread = new Thread(threadStart);
    //thread.IsBackground = true;
    //thread.Start();
    Response.Redirect(Request.RawUrl, true);

  }
  public void ForceRestart()
  {
    throw new ApplicationException();
  }

  public static string StatusLink(string extensionName)
  {
      ManagedExtension x = ExtensionManager.GetExtension(extensionName);
      StringBuilder sb = new StringBuilder();
      if (x.Enabled)
          sb.Append("<span style='background:#ccffcc'><a href='?act=dis&ext=" + x.Name + "' title='" + clickToDisable + x.Name + "' " + jsOnClick + ">" + enabled + "</a></span>");
      else
          sb.Append("<span style='background:#ffcc66'><a href='?act=enb&ext=" + x.Name + "' title='" + clickToEnable + x.Name + "' " + jsOnClick + ">" + disabled + "</a></span>");

      return sb.ToString();
  }

  public static string SettingsLink(string extensionName)
  {
      ManagedExtension x = ExtensionManager.GetExtension(extensionName);
      StringBuilder sb = new StringBuilder();

      if (!string.IsNullOrEmpty(x.AdminPage))
      {
          string url = BlogEngine.Core.Utils.AbsoluteWebRoot.AbsoluteUri;
          if (!url.EndsWith("/"))
              url += "/";

          if (x.AdminPage.StartsWith("~/"))
              url += x.AdminPage.Substring(2);
          else if (x.AdminPage.StartsWith("/"))
              url += x.AdminPage.Substring(1);
          else
              url += x.AdminPage;

          sb.Append("<a href='" + url + "'>" + Resources.labels.edit + "</a>");
      }
      else
      {
          if (x.Settings == null)
          {
              sb.Append("&nbsp;");
          }
          else
          {
              if (x.Settings.Count == 0 || (x.Settings.Count == 1 && x.Settings[0] == null) || x.ShowSettings == false)
                  sb.Append("&nbsp;");
              else
                  sb.Append("<a href='?ctrl=params&ext=" + x.Name + "'>" + Resources.labels.edit + "</a>");
          }
      }
      return sb.ToString();
  }

  /// <summary>
  /// Method to change extension status
  /// to enable or disable extension and
  /// then will restart applicaton by
  /// touching web.config file
  /// </summary>
  /// <param name="act">Enable or Disable</param>
  /// <param name="ext">Extension Name</param>
  void ChangeStatus(string act, string ext)
  {
    // UnloadAppDomain() requires full trust - touch web.config to reload app
    try
    {
      if (act == "dis")
      {
        ExtensionManager.ChangeStatus(ext, false);
      }
      else
      {
        ExtensionManager.ChangeStatus(ext, true);
      }

      if (ExtensionManager.FileAccessException == null)
      {
        //string ConfigPath = HttpContext.Current.Request.PhysicalApplicationPath + "\\web.config";
        //System.IO.File.SetLastWriteTimeUtc(ConfigPath, DateTime.UtcNow);
        Response.Redirect("default.aspx");
      }
      else
      {
        ShowError(ExtensionManager.FileAccessException);
      }
    }
    catch (Exception e)
    {
      ShowError(e);
    }
  }

  /// <summary>
  /// Show error message if something
  /// goes wrong
  /// </summary>
  /// <param name="e">Exception</param>
  void ShowError(Exception e)
  {
    lblErrorMsg.Visible = true;
    lblErrorMsg.InnerHtml = "Changes will not be applied: " + e.Message;
  }

  protected void btnPriorityUp_click(object sender, EventArgs e)
  {
      ImageButton btn = (ImageButton)sender;
      GridViewRow grdRow = (GridViewRow)btn.Parent.Parent;

      string s = gridExtensionsList.DataKeys[grdRow.RowIndex].Value.ToString();
      ChangePriority(s, true);
  }

  protected void btnPriorityDwn_click(object sender, EventArgs e)
  {
      ImageButton btn = (ImageButton)sender;
      GridViewRow grdRow = (GridViewRow)btn.Parent.Parent;

      string s = gridExtensionsList.DataKeys[grdRow.RowIndex].Value.ToString();
      ChangePriority(s, false);
  }

  protected void ChangePriority(string filterName, bool up)
  {
      ManagedExtension x = ExtensionManager.GetExtension(filterName);

      if(x != null)
      {
          if (up && x.Priority > 1)
              x.Priority--;
          else
              x.Priority++;

          ExtensionManager.SaveToStorage(x);
      }

      Response.Redirect(Request.RawUrl);
  }
}