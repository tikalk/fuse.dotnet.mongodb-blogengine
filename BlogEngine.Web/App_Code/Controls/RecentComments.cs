#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BlogEngine.Core;

#endregion

namespace Controls
{
	/// <summary>
	/// Builds a category list.
	/// </summary>
	public class RecentComments : Control
	{

		static RecentComments()
		{
			BindComments();
			Post.CommentAdded += delegate { BindComments(); };
			Post.CommentRemoved += delegate { BindComments(); };
			Post.Saved += new EventHandler<SavedEventArgs>(Post_Saved);
			Comment.Approved += delegate { BindComments(); };
			BlogSettings.Changed += delegate { BindComments(); };
		}

		static void Post_Saved(object sender, SavedEventArgs e)
		{
			if (e.Action != SaveAction.Update)
				BindComments();
		}

		#region Private fields

		private static object _SyncRoot = new object();
		private static List<Comment> _Comments = new List<Comment>();

		#endregion

		private static void BindComments()
		{
			lock (_SyncRoot)
			{
				_Comments.Clear();
				List<Comment> comments = new List<Comment>();

				foreach (Post post in Post.Posts)
				{
					if (post.IsVisible)
					{
						foreach (Comment comment in post.Comments)
						{
							if (comment.IsApproved)
								comments.Add(comment);
						}
					}
				}

				comments.Sort();
				comments.Reverse();
				int counter = 0;

				foreach (Comment comment in comments)
				{
					if (counter == BlogSettings.Instance.NumberOfRecentComments)
						break;

					if (comment.Email == "pingback" || comment.Email == "trackback")
						continue;

					_Comments.Add(comment);
					counter++;
				}

				comments.Clear();
			}
		}

		private string RenderComments()
		{
			if (_Comments.Count == 0)
			{
				return "<p>" + Resources.labels.none + "</p>";
			}

			HtmlGenericControl ul = new HtmlGenericControl("ul");
			ul.Attributes.Add("class", "recentComments");
			ul.ID = "recentComments";

			foreach (Comment comment in _Comments)
			{
				if (comment.IsApproved)
				{
					HtmlGenericControl li = new HtmlGenericControl("li");

					// The post title
					HtmlAnchor title = new HtmlAnchor();
					title.HRef = comment.Parent.RelativeLink.ToString();
					title.InnerText = comment.Parent.Title;
					title.Attributes.Add("class", "postTitle");
					li.Controls.Add(title);

					// The comment count on the post
					LiteralControl count = new LiteralControl(" (" + ((Post)comment.Parent).ApprovedComments.Count + ")<br />");
					li.Controls.Add(count);

					// The author
					if (comment.Website != null)
					{
						HtmlAnchor author = new HtmlAnchor();
                        author.Attributes.Add("rel", "nofollow");
						author.HRef = comment.Website.ToString();
						author.InnerHtml = comment.Author;
						li.Controls.Add(author);

						LiteralControl wrote = new LiteralControl(" " + Resources.labels.wrote + ": ");
						li.Controls.Add(wrote);
					}
					else
					{
						LiteralControl author = new LiteralControl(comment.Author + " " + Resources.labels.wrote + ": ");
						li.Controls.Add(author);
					}

					// The comment body
          string commentBody = Regex.Replace(comment.Content, @"\[(.*?)\]", "");
          int bodyLength = Math.Min(commentBody.Length, 50);

          commentBody = commentBody.Substring(0, bodyLength);
          if (commentBody.Length > 0)
          {
            if (commentBody[commentBody.Length - 1] == '&')
            {
              commentBody = commentBody.Substring(0, commentBody.Length - 1);
            }
          }
					commentBody += comment.Content.Length <= 50 ? " " : "� ";
					LiteralControl body = new LiteralControl(commentBody);
					li.Controls.Add(body);

					// The comment link
					HtmlAnchor link = new HtmlAnchor();
					link.HRef = comment.Parent.RelativeLink + "#id_" + comment.Id;
					link.InnerHtml = "[" + Resources.labels.more + "]";
					link.Attributes.Add("class", "moreLink");
					li.Controls.Add(link);

					ul.Controls.Add(li);
				}
			}

			StringWriter sw = new StringWriter();
			ul.RenderControl(new HtmlTextWriter(sw));
			return sw.ToString();
		}

		/// <summary>
		/// Renders the control.
		/// </summary>
		public override void RenderControl(HtmlTextWriter writer)
		{
			if (Post.Posts.Count > 0)
			{
				string html = RenderComments();
				writer.Write(html);
				writer.Write(Environment.NewLine);
			}
		}
	}
}