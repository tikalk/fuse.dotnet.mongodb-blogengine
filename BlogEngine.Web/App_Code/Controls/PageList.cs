#region Using

using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.IO;
using BlogEngine.Core;

#endregion

namespace Controls
{
  /// <summary>
  /// Builds a page list.
  /// </summary>
  public class PageList : Control
  {

    /// <summary>
    /// Initializes the <see cref="PageList"/> class.
    /// </summary>
    static PageList()
    {
      BlogEngine.Core.Page.Saved += delegate { _Html = null; };
    }

    #region Properties
      

    private static object _SyncRoot = new object(); 
    private static string _Html;
    /// <summary>
    /// Caches the rendered HTML in the private field and first
    /// updates it when a post has been saved (new or updated).
    /// </summary>
    private string Html
    {
      get
      {
        if (_Html == null)
        {
          lock (_SyncRoot)
          {
              if (_Html == null || BlogEngine.Core.Page.Pages == null)
            {
              HtmlGenericControl ul = BindPages();
              System.IO.StringWriter sw = new System.IO.StringWriter();
              ul.RenderControl(new HtmlTextWriter(sw));
              _Html = sw.ToString();
            }
          }
        }

        return _Html;
      }
    }

    #endregion

    /// <summary>
    /// Loops through all pages and builds the HTML
    /// presentation.
    /// </summary>
    private HtmlGenericControl BindPages()
    {
      HtmlGenericControl ul = new HtmlGenericControl("ul");
			ul.Attributes.Add("class", "pagelist");
			ul.ID = "pagelist";

      foreach (BlogEngine.Core.Page page in BlogEngine.Core.Page.Pages)
      {
        if (page.ShowInList && page.IsVisibleToPublic)
        {
          HtmlGenericControl li = new HtmlGenericControl("li");

          HtmlAnchor anc = new HtmlAnchor();
          anc.HRef = page.RelativeLink.ToString();
          anc.InnerHtml = page.Title;
          anc.Title = page.Description;

          li.Controls.Add(anc);
          ul.Controls.Add(li);
        }
      }

      return ul;
    }

    /// <summary>
    /// Renders the control.
    /// </summary>
    public override void RenderControl(HtmlTextWriter writer)
    {
      writer.Write(Html);
      writer.Write(Environment.NewLine);
    }
  }
}