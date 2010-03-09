#region Using

using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;
using System.Collections.Generic;
using System.Web.Security;

#endregion

public partial class widgets_Visitor_info_widget : WidgetBase
{

	public override string Name
	{
		get { return "Visitor info"; }
	}

	public override bool IsEditable
	{
		get { return false; }
	}

	public override void LoadWidget()
	{
		this.Visible = false;
		HttpCookie cookie = Request.Cookies["comment"];

		if (cookie != null)
		{
			string name = cookie.Values["name"];
			string email = cookie.Values["email"];
			string website = cookie.Values["url"];

			if (name != null)
			{
				name = name.Replace("+", " ");
				WriteHtml(name, email, website);

				Uri url;
				if (Request.QueryString["apml"] == null && Uri.TryCreate(website, UriKind.Absolute, out url))
				{
					phScript.Visible = true;
					ltWebsite.Text = url.ToString();
				}

				this.Visible = true;
			}
		}
	}

	private void WriteHtml(string name, string email, string website)
	{
		if (name.Contains(" "))
			name = name.Substring(0, name.IndexOf(" "));

		Title = string.Format("<img src=\"{0}\" alt=\"{1}\" align=\"top\" /> Hi {1}", Gravatar(16, email), Server.HtmlEncode(name));
		pName.InnerHtml = "<strong>Welcome back!</strong>";
		List<Post> list = GetCommentedPosts(email, website);

		if (list.Count > 0)
		{
			string link = string.Format("<a href=\"{0}\">{1}</a>", list[0].RelativeLink, Server.HtmlEncode(list[0].Title));
			pComment.InnerHtml = "New comments have been added to " + link + " since your last comment. ";			
		}

		if (_NumberOfComments > 0)
		{
			pComment.InnerHtml += "You have written " + _NumberOfComments + " comments in total."; 
		}
	}

	private string Gravatar(int size, string email)
	{
		string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(email.ToLowerInvariant().Trim(), "MD5").ToLowerInvariant();
		string gravatar = "http://www.gravatar.com/avatar/" + hash + ".jpg?s=" + size + "&d=";

		string link = string.Empty;
		switch (BlogSettings.Instance.Avatar)
		{
			case "identicon":
				link = gravatar + "identicon";
				break;

			case "wavatar":
				link = gravatar + "wavatar";
				break;

			default:
				link = gravatar + "monsterid";
				break;
		}

		return link;
	}

	private int _NumberOfComments = 0;

	private List<Post> GetCommentedPosts(string email, string website)
	{
		List<Post> list = new List<Post>();
		foreach (Post post in Post.Posts)
		{
			List<Comment> comments = post.Comments.FindAll(delegate(Comment c)
			{
				if (email.Equals(c.Email, StringComparison.OrdinalIgnoreCase))
					return true;

				if (c.Website != null && c.Website.ToString().Equals(website, StringComparison.OrdinalIgnoreCase))
					return true;

				return false;
			});

			if (comments.Count > 0)
			{
				_NumberOfComments += comments.Count;
				int index = post.Comments.IndexOf(comments[comments.Count - 1]);
				if (index < post.Comments.Count - 1 && post.Comments[post.Comments.Count - 1].DateCreated > DateTime.Now.AddDays(-7))
					list.Add(post);
			}
		}

		return list;
	}
}
