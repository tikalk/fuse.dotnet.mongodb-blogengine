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
	/// Builds a authro list.
	/// </summary>
	public class AuthorList : Control
	{

		/// <summary>
		/// Initializes the <see cref="AuthorList"/> class.
		/// </summary>
		static AuthorList()
		{
			Post.Saved += delegate { _Html = null; };
		}

		#region Properties

		private static bool _ShowRssIcon = true;
		/// <summary>
		/// Gets or sets whether or not to show feed icons next to the category links.
		/// </summary>
		public bool ShowRssIcon
		{
			get { return _ShowRssIcon; }
			set
			{
				if (_ShowRssIcon != value)
				{
					_ShowRssIcon = value;
					_Html = null;
				}
			}
		}

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
						if (_Html == null)
						{
							HtmlGenericControl ul = BindAuthors();
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
		/// Loops through all users and builds the HTML
		/// presentation.
		/// </summary>
		private HtmlGenericControl BindAuthors()
		{
			if (Post.Posts.Count == 0)
			{
				HtmlGenericControl p = new HtmlGenericControl("p");
				p.InnerHtml = Resources.labels.none;
				return p;
			}

			HtmlGenericControl ul = new HtmlGenericControl("ul");
			ul.ID = "authorlist";

			foreach (MembershipUser user in Membership.GetAllUsers())
			{
				int postCount = Post.GetPostsByAuthor(user.UserName).Count;
				if (postCount == 0)
					continue;

				HtmlGenericControl li = new HtmlGenericControl("li");

				if (ShowRssIcon)
				{
					HtmlImage img = new HtmlImage();
					img.Src = Utils.RelativeWebRoot + "pics/rssButton.gif";
					img.Alt = "RSS feed for " + user.UserName;
					img.Attributes["class"] = "rssButton";

					HtmlAnchor feedAnchor = new HtmlAnchor();
					feedAnchor.HRef = Utils.RelativeWebRoot + "syndication.axd?author=" + Utils.RemoveIllegalCharacters(user.UserName);
					feedAnchor.Attributes["rel"] = "nofollow";
					feedAnchor.Controls.Add(img);

					li.Controls.Add(feedAnchor);
				}
				
				HtmlAnchor anc = new HtmlAnchor();
				anc.HRef = Utils.RelativeWebRoot + "author/" + user.UserName + BlogSettings.Instance.FileExtension;
				anc.InnerHtml = user.UserName + " (" + postCount + ")";
				anc.Title = "Author: " + user.UserName;

				li.Controls.Add(anc);
				ul.Controls.Add(li);
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