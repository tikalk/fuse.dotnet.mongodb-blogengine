#region Using

using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using System.Net.Mail;
using System.Text.RegularExpressions;

#endregion

public partial class contact : BlogBasePage, ICallbackEventHandler
{

	private static readonly Regex _Regex = new Regex("<[^>]*>", RegexOptions.Compiled);

	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
		ClientScript.GetCallbackEventReference(this, "arg", "callback", "context");
		btnSend.Click += new EventHandler(btnSend_Click);
		if (!Page.IsPostBack)
		{
			txtSubject.Text = Request.QueryString["subject"];
			txtName.Text = Request.QueryString["name"];
			txtEmail.Text = Request.QueryString["email"];

			GetCookie();
			phAttachment.Visible = BlogSettings.Instance.EnableContactAttachments;
			InititializeCaptcha();
			SetFocus();
		}

		Page.Title = Server.HtmlEncode(Resources.labels.contact);
		base.AddMetaTag("description", _Regex.Replace(BlogSettings.Instance.ContactFormMessage, string.Empty));
	}

	/// <summary>
	/// Sets the focus on the first empty textbox.
	/// </summary>
	private void SetFocus()
	{
		if (string.IsNullOrEmpty(Request.QueryString["name"]) && txtName.Text == string.Empty)
		{
			txtName.Focus();
		}
		else if (string.IsNullOrEmpty(Request.QueryString["email"]) && txtEmail.Text == string.Empty)
		{
			txtEmail.Focus();
		}
		else if (string.IsNullOrEmpty(Request.QueryString["subject"]))
		{
			txtSubject.Focus();
		}
		else
		{
			txtMessage.Focus();
		}
	}

	/// <summary>
	/// Handles the Click event of the btnSend control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	private void btnSend_Click(object sender, EventArgs e)
	{
		if (IsCaptchaValid && Page.IsValid && txtAttachment.HasFile)
		{
			bool success = SendEmail(txtEmail.Text, txtName.Text, txtSubject.Text, txtMessage.Text);
			divForm.Visible = !success;
			lblStatus.Visible = !success;
			divThank.Visible = success;
			SetCookie();
		}
	}

	private bool SendEmail(string email, string name, string subject, string message)
	{
		try
		{
			using (MailMessage mail = new MailMessage())
			{
				mail.From = new MailAddress(BlogSettings.Instance.Email, name);
				mail.ReplyTo = new MailAddress(email, name);

				mail.To.Add(BlogSettings.Instance.Email);
				mail.Subject = BlogSettings.Instance.EmailSubjectPrefix + " e-mail - " + subject;

				mail.Body = "<div style=\"font: 11px verdana, arial\">";
				mail.Body += Server.HtmlEncode(message).Replace("\n", "<br />") + "<br /><br />";
				mail.Body += "<hr /><br />";
				mail.Body += "<h3>Author information</h3>";
				mail.Body += "<div style=\"font-size:10px;line-height:16px\">";
				mail.Body += "<strong>Name:</strong> " + Server.HtmlEncode(name) + "<br />";
				mail.Body += "<strong>E-mail:</strong> " + Server.HtmlEncode(email) + "<br />";

				if (ViewState["url"] != null)
					mail.Body += string.Format("<strong>Website:</strong> <a href=\"{0}\">{0}</a><br />", ViewState["url"]);

				if (ViewState["country"] != null)
					mail.Body += "<strong>Country code:</strong> " + ((string)ViewState["country"]).ToUpperInvariant() + "<br />";

				if (HttpContext.Current != null)
				{
					mail.Body += "<strong>IP address:</strong> " + HttpContext.Current.Request.UserHostAddress + "<br />";
					mail.Body += "<strong>User-agent:</strong> " + HttpContext.Current.Request.UserAgent;
				}

				if (txtAttachment.HasFile)
				{
					Attachment attachment = new Attachment(txtAttachment.PostedFile.InputStream, txtAttachment.FileName);
					mail.Attachments.Add(attachment);
				}

				Utils.SendMailMessage(mail);
			}

			return true;
		}
		catch (Exception ex)
		{
			if (User.Identity.IsAuthenticated)
			{
				if (ex.InnerException != null)
					lblStatus.Text = ex.InnerException.Message;
				else
					lblStatus.Text = ex.Message;
			}

			return false;
		}
	}

	#region Cookies

	/// <summary>
	/// Gets the cookie with visitor information if any is set.
	/// Then fills the contact information fields in the form.
	/// </summary>
	private void GetCookie()
	{
		HttpCookie cookie = Request.Cookies["comment"];
		if (cookie != null)
		{
			txtName.Text = Server.UrlDecode(cookie.Values["name"]);
			txtEmail.Text = cookie.Values["email"];
			ViewState["url"] = cookie.Values["url"];
			ViewState["country"] = cookie.Values["country"];
		}
	}

	/// <summary>
	/// Sets a cookie with the entered visitor information
	/// so it can be prefilled on next visit.
	/// </summary>
	private void SetCookie()
	{
		HttpCookie cookie = new HttpCookie("comment");
		cookie.Expires = DateTime.Now.AddMonths(24);
		cookie.Values.Add("name", Server.UrlEncode(txtName.Text));
		cookie.Values.Add("email", txtEmail.Text);
		cookie.Values.Add("url", string.Empty);
		cookie.Values.Add("country", string.Empty);
		Response.Cookies.Add(cookie);
	}

	#endregion

	#region CAPTCHA

	/// <summary> 
	/// Initializes the captcha and registers the JavaScript 
	/// </summary> 
	private void InititializeCaptcha()
	{
		if (ViewState[DateTime.Today.Ticks.ToString()] == null)
		{
			ViewState[DateTime.Today.Ticks.ToString()] = Guid.NewGuid().ToString();
		}

		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.AppendLine("function SetCaptcha(){");
		sb.AppendLine("var form = document.getElementById('" + Page.Form.ClientID + "');");
		sb.AppendLine("var el = document.createElement('input');");
		sb.AppendLine("el.type = 'hidden';");
		sb.AppendLine("el.name = '" + DateTime.Today.Ticks + "';");
		sb.AppendLine("el.value = '" + ViewState[DateTime.Today.Ticks.ToString()] + "';");
		sb.AppendLine("form.appendChild(el);}");

		Page.ClientScript.RegisterClientScriptBlock(GetType(), "captchascript", sb.ToString(), true);
		Page.ClientScript.RegisterOnSubmitStatement(GetType(), "captchayo", "SetCaptcha()");
	}

	/// <summary> 
	/// Gets whether or not the user is human 
	/// </summary> 
	private bool IsCaptchaValid
	{
		get
		{
			if (ViewState[DateTime.Today.Ticks.ToString()] != null)
			{
				return Request.Form[DateTime.Today.Ticks.ToString()] == ViewState[DateTime.Today.Ticks.ToString()].ToString();
			}

			return false;
		}
	}

	#endregion


	#region ICallbackEventHandler Members

	private string _Callback;

	public string GetCallbackResult()
	{
		return _Callback;
	}

	public void RaiseCallbackEvent(string eventArgument)
	{
		string[] arg = eventArgument.Split(new string[] { "-||-" }, StringSplitOptions.RemoveEmptyEntries);
		if (arg.Length == 4)
		{
			string name = arg[0];
			string email = arg[1];
			string subject = arg[2];
			string message = arg[3];
			
			if (SendEmail(email, name, subject, message))
			{
				_Callback = BlogSettings.Instance.ContactThankMessage;
			}
			else
			{
				_Callback = "This form does not work at the moment. Sorry for the inconvenience.";
			}
		}
		else
		{
			_Callback = "This form does not work at the moment. Sorry for the inconvenience.";
		}
	}

	#endregion

}