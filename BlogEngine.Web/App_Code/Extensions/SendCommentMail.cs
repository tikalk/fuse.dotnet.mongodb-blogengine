#region using

using System;
using System.Web;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core;
using System.Net.Mail;
using System.Threading;

#endregion

/// <summary>
/// Sends an e-mail to the blog owner whenever a comment is added.
/// </summary>
[Extension("Sends an e-mail to the blog owner whenever a comment is added", "1.3", "BlogEngine.NET")]
public class SendCommentMail
{

	/// <summary>
	/// Hooks up an event handler to the Post.CommentAdded event.
	/// </summary>
	static SendCommentMail()
	{
		Post.CommentAdded += new EventHandler<EventArgs>(Post_CommentAdded);
	}

	private static void Post_CommentAdded(object sender, EventArgs e)
	{
		Post post = (Post)((Comment)sender).Parent;
		if (post != null && BlogSettings.Instance.SendMailOnComment && !Thread.CurrentPrincipal.Identity.IsAuthenticated)
		{
			Comment comment = post.Comments[post.Comments.Count - 1];
			// Trackback and pingback comments don't have a '@' symbol in the e-mail field.
			string replyTo = comment.Email.Contains("@") ? comment.Email : BlogSettings.Instance.Email;
			string subject = " comment on ";

			if (comment.Email == "trackback")
				subject = " trackback on ";
			else if (comment.Email == "pingback")
				subject = " pingback on ";

            System.Globalization.CultureInfo defaultCulture = Utils.GetDefaultCulture();

			ServingEventArgs args = new ServingEventArgs(comment.Content, ServingLocation.Email);
			Comment.OnServing(comment, args);
			string body = args.Body;

			MailMessage mail = new MailMessage();
			mail.From = new MailAddress(BlogSettings.Instance.Email);
			mail.To.Add(BlogSettings.Instance.Email);
            mail.ReplyTo = new MailAddress(replyTo, HttpUtility.HtmlDecode(comment.Author));
			mail.Subject = BlogSettings.Instance.EmailSubjectPrefix + subject + post.Title;
			mail.Body = "<div style=\"font: 11px verdana, arial\">";
			mail.Body += body.Replace(Environment.NewLine, "<br />") + "<br /><br />";
            mail.Body += string.Format("<strong>{0}</strong>: <a href=\"{1}\">{2}</a><br /><br />", Utils.Translate("post", null, defaultCulture), post.PermaLink + "#id_" + comment.Id, post.Title);

			string deleteLink = post.AbsoluteLink + "?deletecomment=" + comment.Id;
            mail.Body += string.Format("<a href=\"{0}\">{1}</a>", deleteLink, Utils.Translate("delete", null, defaultCulture));

			if (BlogSettings.Instance.EnableCommentsModeration)
			{
				string approveLink = post.AbsoluteLink + "?approvecomment=" + comment.Id;
                mail.Body += string.Format(" | <a href=\"{0}\">{1}</a>", approveLink, Utils.Translate("approve", null, defaultCulture));
			}

			mail.Body += "<br />_______________________________________________________________________________<br />";
			mail.Body += "<h3>Author information</h3>";
			mail.Body += "<div style=\"font-size:10px;line-height:16px\">";
			mail.Body += "<strong>Name:</strong> " + comment.Author + "<br />";
			mail.Body += "<strong>E-mail:</strong> " + comment.Email + "<br />";
			mail.Body += string.Format("<strong>Website:</strong> <a href=\"{0}\">{0}</a><br />", comment.Website);

			if (comment.Country != null)
				mail.Body += "<strong>Country code:</strong> " + comment.Country.ToUpperInvariant() + "<br />";

			if (HttpContext.Current != null)
			{
				mail.Body += "<strong>IP address:</strong> " + HttpContext.Current.Request.UserHostAddress + "<br />";
				mail.Body += "<strong>User-agent:</strong> " + HttpContext.Current.Request.UserAgent;
			}

			mail.Body += "</div>";
			mail.Body += "</div>";

			Utils.SendMailMessageAsync(mail);
		}
	}

}
