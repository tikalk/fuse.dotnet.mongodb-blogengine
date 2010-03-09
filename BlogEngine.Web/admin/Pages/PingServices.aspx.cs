#region Using

using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core.Providers;

#endregion

public partial class admin_Pages_PingServices : System.Web.UI.Page
{
	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!Page.IsPostBack)
    {
      BindGrid();
    }

    grid.RowEditing += new GridViewEditEventHandler(grid_RowEditing);
    grid.RowUpdating += new GridViewUpdateEventHandler(grid_RowUpdating);
    grid.RowCancelingEdit += delegate { Response.Redirect(Request.RawUrl); };
    grid.RowDeleting += new GridViewDeleteEventHandler(grid_RowDeleting);
    btnAdd.Click += new EventHandler(btnAdd_Click);
    btnAdd.Text = Resources.labels.add + " ping service";
  }

	/// <summary>
	/// Handles the Click event of the btnAdd control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
  void btnAdd_Click(object sender, EventArgs e)
  {
    StringCollection col = BlogService.LoadPingServices();
    string service = txtNewCategory.Text;
    if (!col.Contains(service))
    {
      col.Add(service);
      BlogService.SavePingServices(col);
    }
    Response.Redirect(Request.RawUrl);
  }

	/// <summary>
	/// Handles the RowDeleting event of the grid control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewDeleteEventArgs"/> instance containing the event data.</param>
  void grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
  {
    string service = grid.DataKeys[e.RowIndex].Value.ToString();
    StringCollection col = BlogService.LoadPingServices();
    col.Remove(service);    
    BlogService.SavePingServices(col);
    Response.Redirect(Request.RawUrl);
  }

	/// <summary>
	/// Handles the RowUpdating event of the grid control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewUpdateEventArgs"/> instance containing the event data.</param>
  void grid_RowUpdating(object sender, GridViewUpdateEventArgs e)
  {
    string service = grid.DataKeys[e.RowIndex].Value.ToString();
    TextBox textbox = (TextBox)grid.Rows[e.RowIndex].FindControl("txtName");
    
    StringCollection col = BlogService.LoadPingServices();
    col.Remove(service);
    col.Add(textbox.Text);
    BlogService.SavePingServices(col);
    
    Response.Redirect(Request.RawUrl);
  }

	/// <summary>
	/// Handles the RowEditing event of the grid control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewEditEventArgs"/> instance containing the event data.</param>
  void grid_RowEditing(object sender, GridViewEditEventArgs e)
  {
    grid.EditIndex = e.NewEditIndex;
    BindGrid();
  }

  private void BindGrid()
  {
    StringCollection col = BlogService.LoadPingServices();
		SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
    foreach (string services in col)
    {
      dic.Add(services, services);
    }

    grid.DataKeyNames = new string[] { "key" };
    grid.DataSource = dic;
    grid.DataBind();
  }

}
