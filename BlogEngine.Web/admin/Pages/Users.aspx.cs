#region Using

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
using BlogEngine.Core.Providers;
using BlogEngine.Core;

#endregion

public partial class admin_newuser : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		gridUsers.RowEditing += new GridViewEditEventHandler(grid_RowEditing);
		gridUsers.RowUpdating += new GridViewUpdateEventHandler(grid_RowUpdating);
		gridUsers.RowCancelingEdit += delegate { Response.Redirect(Request.RawUrl); };
		gridUsers.RowDeleting += new GridViewDeleteEventHandler(grid_RowDeleting);
		gridUsers.RowDataBound += new GridViewRowEventHandler(gridUsers_RowDataBound);

		CreateUserWizard1.CreatingUser += new LoginCancelEventHandler(CreateUserWizard1_CreatingUser);
		CreateUserWizard1.CreatedUser += new EventHandler(CreateUserWizard1_CreatedUser);

		Page.Title = Resources.labels.users;

		if (!Page.IsPostBack)
			BindGrid();
	}

	void CreateUserWizard1_CreatingUser(object sender, LoginCancelEventArgs e)
	{
		if (Membership.GetUser(CreateUserWizard1.UserName) != null)
		{
			e.Cancel = true;
			lblError.Visible = true;
		}
	}

	/// <summary>
	/// Implements a row control finder based on the type of control and the control Id.
	/// </summary>
	/// <param name="row">The row.</param>
	/// <param name="controlType">Type of the control.</param>
	/// <param name="id">Name of the contains.</param>
	/// <returns>The control if found, otherwise null</returns>
	static Control FindRowControl(GridViewRow row, Type controlType, string id)
	{
		foreach (TableCell cell in row.Cells)
		{
			foreach (Control control in cell.Controls)
			{
				if (control.GetType() == controlType && control.ID.Contains(id))
				{
					return control;
				}
			}
		}
		return null;
	}

	void gridUsers_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow && !Page.IsPostBack)
		{
			LinkButton delete = e.Row.Cells[0].Controls[2] as LinkButton;
			if (delete != null)
			{
				Label username = (Label)FindRowControl(e.Row, typeof(Label), "labelUsername");
				string text =
						string.Format(Resources.labels.areYouSure, Resources.labels.delete.ToLowerInvariant(),
													username.Text.Trim());
				if (Page.User.Identity.Name.Equals(username.Text, StringComparison.OrdinalIgnoreCase))
				{
					delete.OnClientClick = "alert('You cannot delete your own account');return false;";
				}
				else
				{
					delete.OnClientClick = "return confirm('" + text.Replace("'", "\\'") + "')";
				}
			}
		}
	}

	private void BindGrid()
	{
		int count = 0;
		gridUsers.DataSource = Membership.Provider.GetAllUsers(0, 999, out count);
		gridUsers.DataKeyNames = new string[] { "username" };
		gridUsers.DataBind();
		if (count > 0)
			gridUsers.HeaderRow.Cells[1].Text = Resources.labels.userName;
	}

	/// <summary>
	/// Handles the RowDeleting event of the grid control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewDeleteEventArgs"/> instance containing the event data.</param>
	void grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
	{
		string username = (string)gridUsers.DataKeys[e.RowIndex].Value;
		string[] roles = Roles.GetRolesForUser(username);

        if (roles.Length > 0)
            Roles.RemoveUserFromRoles(username, roles);

        Membership.DeleteUser(username);

		AuthorProfile profile = AuthorProfile.GetProfile(username);
        if (profile != null)
        { 
			profile.Delete();
            profile.Save();
        }

		if (HttpContext.Current.User.Identity.Name.Equals(username, StringComparison.OrdinalIgnoreCase))
			FormsAuthentication.SignOut();

		Response.Redirect(Request.RawUrl);
	}

	/// <summary>
	/// Handles the RowUpdating event of the grid control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewUpdateEventArgs"/> instance containing the event data.</param>
	void grid_RowUpdating(object sender, GridViewUpdateEventArgs e)
	{
		string username = (string)gridUsers.DataKeys[e.RowIndex].Value;
		//TextBox txtPassword = (TextBox)gridUsers.Rows[e.RowIndex].FindControl("txtPassword");
		TextBox txtEmail = (TextBox)gridUsers.Rows[e.RowIndex].FindControl("txtEmail");
		//TextBox txtUsername = (TextBox)gridUsers.Rows[e.RowIndex].FindControl("txtUsername");

		MembershipUser oldUser = Membership.GetUser(username);
		//string[] oldRoles = Roles.GetRolesForUser(username);
		//if (oldRoles.Length > 0)
		//    Roles.RemoveUserFromRoles(username, oldRoles);
		//Membership.DeleteUser(username);

		//MembershipUser user = Membership.CreateUser(txtUsername.Text, txtPassword.Text, txtEmail.Text);
		//if (oldRoles.Length > 0)
		//    Roles.AddUserToRoles(user.UserName, oldRoles);

		//if (username != user.UserName)
		//    UpdatePosts(username, txtUsername.Text);

		oldUser.Email = txtEmail.Text;
		Membership.UpdateUser(oldUser);

		if (HttpContext.Current.User.Identity.Name.Equals(oldUser.UserName, StringComparison.CurrentCultureIgnoreCase))
			FormsAuthentication.SignOut();

		Response.Redirect(Request.RawUrl);
	}

	private static void UpdatePosts(string oldUsername, string newUsername)
	{
		for (int i = 0; i < Post.Posts.Count; i++)
		{
			Post post = Post.Posts[i];
			if (post.Author == oldUsername)
			{
				foreach (Comment comment in post.Comments)
				{
					if (comment.Author == oldUsername)
						comment.Author = newUsername;
				}

				post.Author = newUsername;
				post.Save();
			}
		}
	}

	/// <summary>
	/// Handles the RowEditing event of the grid control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewEditEventArgs"/> instance containing the event data.</param>
	void grid_RowEditing(object sender, GridViewEditEventArgs e)
	{
		gridUsers.EditIndex = e.NewEditIndex;
		BindGrid();
	}

	/// <summary>
	/// Handles the CreatedUser event of the CreateUserWizard1 control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
	{
		Response.Redirect("Users.aspx", true);
	}


	/// <summary>
	/// Handles the CheckedChanged event of the cb control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void cb_CheckedChanged(object sender, EventArgs e)
	{
		CheckBox cb = (CheckBox)sender;
		GridViewRow drv = (GridViewRow)cb.Parent.BindingContainer;
		string _userName = gridUsers.DataKeys[drv.DataItemIndex].Value.ToString();
		string _roleToUse = cb.Text;
		if (cb.Checked == false)
		{
			if (User.Identity.Name != _userName.ToLower())
				Roles.RemoveUserFromRole(_userName, _roleToUse);
		}
		else
		{
			if (!Roles.IsUserInRole(_userName, _roleToUse))
				Roles.AddUserToRole(_userName, _roleToUse);
		}
		Response.Redirect("Users.aspx", true);
	}

	/// <summary>
	/// Handles the Load event of the gridUsers control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void gridUsers_Load(object sender, EventArgs e)
	{
		for (int i = 0; i < gridUsers.Rows.Count; i++)
		{
			string[] allRoles = Roles.GetAllRoles();
			foreach (string _role in allRoles)
			{
				CheckBox cb = new CheckBox();
				cb.Text = _role;
				cb.Checked = Roles.IsUserInRole(gridUsers.DataKeys[i].Value.ToString(), _role);
				cb.AutoPostBack = true;
				cb.TextAlign = TextAlign.Right;
				cb.Style.Add("display", "inline");
				cb.Style.Add("padding-right", "15px");
				cb.CheckedChanged += new EventHandler(cb_CheckedChanged);
				gridUsers.Rows[i].Cells[4].Controls.Add(cb);
			}
		}
	}
}
