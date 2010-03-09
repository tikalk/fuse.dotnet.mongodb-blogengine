#region Using

using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;

#endregion

public partial class login : BlogEngine.Core.Web.Controls.BlogBasePage
{
	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (Request.QueryString.ToString() == "logoff")
		{
			FormsAuthentication.SignOut();
			if (Request.UrlReferrer != null && Request.UrlReferrer != Request.Url)
			{
				Response.Redirect(Request.UrlReferrer.ToString(), true);
			}
			else
			{
				Response.Redirect("login.aspx");
			}
		}

		if (Page.User.Identity.IsAuthenticated)
		{
			changepassword1.Visible = true;
			changepassword1.ContinueButtonClick += new EventHandler(changepassword1_ContinueButtonClick);
			lsLogout.Visible = true;
			Login1.Visible = false;
			Page.Title = Resources.labels.changePassword;
		}
		else
		{
            Login1.LoggingIn += new LoginCancelEventHandler(Login1_LoggingIn);
			Login1.LoggedIn += new EventHandler(Login1_LoggedIn);
			Login1.FindControl("username").Focus();
		}
	}

	/// <summary>
	/// Handles the ContinueButtonClick event of the changepassword1 control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void changepassword1_ContinueButtonClick(object sender, EventArgs e)
	{
		Response.Redirect(BlogEngine.Core.Utils.RelativeWebRoot, true);
	}

	/// <summary>
	/// Handles the LoggedIn event of the Login1 control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void Login1_LoggedIn(object sender, EventArgs e)
	{
		if (!Roles.IsUserInRole(Login1.UserName, BlogEngine.Core.BlogSettings.Instance.AdministratorRole))
			Response.Redirect(BlogEngine.Core.Utils.RelativeWebRoot, true);
	}
    /// <summary>
    /// Handles the LoggingIn event of the Login1 control.  Adjusts the casing (upper/lower) of
    /// the username logged in with to the same case the user is registered as.  This prevents
    /// case sensitivity issues through the application.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Login1_LoggingIn(object sender, LoginCancelEventArgs e)
    {
        string username = Login1.UserName.Trim();
        if (!string.IsNullOrEmpty(username))
        { 
            MembershipUser user = Membership.GetUser(username);
            if (user != null)
            {
                // Only adjust the UserName if the password is correct.  This is more secure
                // so a hacker can't find valid usernames if we adjust the case of mis-cased
                // usernames with incorrect passwords.
                if (Membership.ValidateUser(user.UserName, Login1.Password))
                { 
                    Login1.UserName = user.UserName;
                }
            }
        }
    }
}
