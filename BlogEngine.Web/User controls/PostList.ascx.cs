#region Using

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;

#endregion

public partial class User_controls_PostList : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsCallback)
		{
			BindPosts();
			InitPaging();
		}
	}

	/// <summary>
	/// Binds the list of posts to individual postview.ascx controls
	/// from the current theme. 
	/// </summary>
	private void BindPosts()
	{
		if (Posts == null || Posts.Count == 0)
		{
			hlPrev.Visible = false;
			return;
		}

		List<IPublishable> visiblePosts = Posts.FindAll(delegate(IPublishable p) { return p.IsVisible; });

		if (Posts == null || visiblePosts.Count == 0)
		{
			hlPrev.Visible = false;
			hlNext.Visible = false;
			return;
		}

		int count = Math.Min(BlogSettings.Instance.PostsPerPage, visiblePosts.Count);
		int page = GetPageIndex();
		int index = page * count;
		int stop = count;
		if (index + count > visiblePosts.Count)
			stop = visiblePosts.Count - index;

		if (stop < 0 || stop + index > visiblePosts.Count)
		{
			hlPrev.Visible = false;
			hlNext.Visible = false;
			return;
		}

        string path = Utils.RelativeWebRoot + "themes/" + BlogSettings.Instance.Theme + "/PostView.ascx";
		int counter = 0;

        bool showExcerpt = false;
        int descriptionCharacters = 0;

        // To allow WLW to download the theme when setting up account, avoid excerpt.
        if (string.IsNullOrEmpty(Request.UserAgent) || Request.UserAgent.IndexOf("Windows Live Writer", StringComparison.OrdinalIgnoreCase) == -1)
        {
            if (this.ContentBy == ServingContentBy.Tag || this.ContentBy == ServingContentBy.Category)
            {
                showExcerpt = BlogSettings.Instance.ShowDescriptionInPostListForPostsByTagOrCategory;
                descriptionCharacters = BlogSettings.Instance.DescriptionCharactersForPostsByTagOrCategory;
            }
            else
            {
                showExcerpt = BlogSettings.Instance.ShowDescriptionInPostList;
                descriptionCharacters = BlogSettings.Instance.DescriptionCharacters;
            }
        }

		foreach (Post post in visiblePosts.GetRange(index, stop))
		{
			if (counter == stop)
				break;

			PostViewBase postView = (PostViewBase)LoadControl(path);
            postView.ShowExcerpt = showExcerpt;
            postView.DescriptionCharacters = descriptionCharacters;
			postView.Post = post;
			postView.Index = counter;
			postView.ID = post.Id.ToString().Replace("-", string.Empty);
			postView.Location = ServingLocation.PostList;
            postView.ContentBy = this.ContentBy;
			posts.Controls.Add(postView);
			counter++;
		}

		if (index + stop == visiblePosts.Count)
			hlPrev.Visible = false;
	}

	/// <summary>
	/// Retrieves the current page index based on the QueryString.
	/// </summary>
	private int GetPageIndex()
	{
		int index = 0;
		string page = Request.QueryString["page"];
		if (page != null && int.TryParse(page, out index) && index > 0)
			index--;

		return index;
	}

    private static readonly Regex REMOVE_DEFAULT_ASPX = new Regex("default\\.aspx", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    /// <summary>
    /// Initializes the Next and Previous links
    /// </summary>
    private void InitPaging()
    {
        string path = Request.RawUrl;

        // Leave "default.aspx" when posts for a specific year/month or specific date are displayed.
        if (!(Request.QueryString["year"] != null || Request.QueryString["date"] != null))
            path = REMOVE_DEFAULT_ASPX.Replace(path, string.Empty);

		if (path.Contains("?"))
		{
			if (path.Contains("page="))
			{
				int index = path.IndexOf("page=");
				path = path.Substring(0, index);
			}
			else
			{
				path += "&";
			}
		}
		else
		{
			path += "?";
		}

		int page = GetPageIndex();
		string url = path + "page={0}";

		//if (page != 1)
		hlNext.HRef = string.Format(url, page);
		//else
		//hlNext.HRef = path.Replace("?", string.Empty);
		
		hlPrev.HRef = string.Format(url, page + 2);

		if (page == 0)
		{
			hlNext.Visible = false;
		}
		else
		{
			(Page as BlogBasePage).AddGenericLink("next", "Next page", hlNext.HRef);
			Page.Title += " - Page " + (page + 1);
		}

		if (hlPrev.Visible)
			(Page as BlogBasePage).AddGenericLink("prev", "Previous page", string.Format(url, page + 2));
	}

	#region Properties

	private List<IPublishable> _Posts;
	/// <summary>
	/// The list of posts to display.
	/// </summary>
	public List<IPublishable> Posts
	{
		get { return _Posts; }
		set { _Posts = value; }
	}

    private ServingContentBy _ContentBy = ServingContentBy.AllContent;
    /// <summary>
    /// The criteria by which the content is being served (by tag, category, author, etc).
    /// </summary>
    public ServingContentBy ContentBy
    {
        get { return _ContentBy; }
        set { _ContentBy = value; }
    }

	#endregion

}
