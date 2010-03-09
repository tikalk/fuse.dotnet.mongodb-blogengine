#region Using

using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using BlogEngine.Core;

#endregion

public partial class search : BlogEngine.Core.Web.Controls.BlogBasePage
{

	private const int PAGE_SIZE = 20;

	#region Event handlers

	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
		rep.ItemDataBound += new RepeaterItemEventHandler(rep_ItemDataBound);

		if (!string.IsNullOrEmpty(Request.QueryString["q"]))
		{
			bool includeComments = Request.QueryString["comment"] == "true";
			string term = Request.QueryString["q"];
			Page.Title = Server.HtmlEncode(Resources.labels.searchResultsFor) + " '" + Server.HtmlEncode(Request.QueryString["q"]) + "'";
			h1Headline.InnerHtml = Resources.labels.searchResultsFor + " '" + Server.HtmlEncode(Request.QueryString["q"]) + "'";

			Uri url;
			if (!Uri.TryCreate(term, UriKind.Absolute, out url))
			{
				List<IPublishable> list = Search.Hits(term, includeComments);
				BindSearchResult(list);
			}
			else
			{
				SearchByApml(url);
			}
		}
		else
		{
			Page.Title = Resources.labels.search;
			h1Headline.InnerHtml = Resources.labels.search;
		}
	}

	private void SearchByApml(Uri url)
	{
		List<IPublishable> list = new List<IPublishable>();
		try
		{
			Dictionary<Uri, XmlDocument> docs = Utils.FindSemanticDocuments(url, "apml");
			if (docs.Count > 0)
			{
				foreach (Uri key in docs.Keys)
				{
					list = Search.ApmlMatches(docs[key], 30);
					Page.Title = "APML matches for '" + Server.HtmlEncode(key.ToString()) + "'";
					break;
				}
			}
			else
			{
				Page.Title = "APML matches for '" + Request.QueryString["q"] + "'";
			}
			h1Headline.InnerText = Page.Title;
		}
		catch
		{

		}

		BindSearchResult(list);
	}

	/// <summary>
	/// Handles the ItemDataBound event of the rep control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
	void rep_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		HtmlGenericControl control = (HtmlGenericControl)e.Item.FindControl("type");
		string type = "<strong>Type</strong>: {0}";
		string categories = "<strong>" + Resources.labels.categories + "</strong> : {0}";
		string tags = "<strong>Tags</strong> : {0}";
		string keywords = "<strong>Keywords</strong> : {0}    ";
		if (e.Item.DataItem is Comment)
		{
			control.InnerHtml = string.Format(type, Resources.labels.comment);
		}
		else if (e.Item.DataItem is Post)
		{
			Post post = (Post)e.Item.DataItem;
			string text = string.Format(type, Resources.labels.post);
			if (post.Categories.Count > 0)
			{
				string cat = string.Empty;
				foreach (Category category in post.Categories)
				{
					cat += category.Title + ", ";
				}

				text += "<br />" + string.Format(categories, cat.Substring(0, cat.Length - 2));
			}

			if (post.Tags.Count > 0)
			{
				string t = string.Empty;
				foreach (string tag in post.Tags)
				{
					t += tag + ", ";
				}

				text += "<br />" + string.Format(tags, t.Substring(0, t.Length - 2));
			}

			control.InnerHtml = text;
		}
		else if (e.Item.DataItem is BlogEngine.Core.Page)
		{
			BlogEngine.Core.Page page = (BlogEngine.Core.Page)e.Item.DataItem;
			string text = string.Format(type, Resources.labels.page);

			if (!string.IsNullOrEmpty(page.Keywords))
			{
				text += "<br />" + string.Format(keywords, page.Keywords);
			}

			control.InnerHtml = text;
		}
	}

	#endregion

	#region Data binding

	private void BindSearchResult(List<IPublishable> list)
	{
		int page = 1;
		if (Request.QueryString["page"] != null)
		{
			page = int.Parse(Request.QueryString["page"]);
		}

		int start = PAGE_SIZE * (page - 1);

		if (start <= list.Count - 1)
		{
			int size = PAGE_SIZE;
			if (start + size > list.Count)
				size = list.Count - start;

			rep.DataSource = list.GetRange(start, size);
			rep.DataBind();

			BindPaging(list.Count, page - 1);
		}
	}

	private void BindPaging(int results, int page)
	{
		if (results <= PAGE_SIZE)
			return;

		decimal pages = Math.Ceiling((decimal)results / (decimal)PAGE_SIZE);

		HtmlGenericControl ul = new HtmlGenericControl("ul");
		ul.Attributes.Add("class", "paging");

		for (int i = 0; i < pages; i++)
		{
			HtmlGenericControl li = new HtmlGenericControl("li");
			if (i == page)
			{
				li.Attributes.Add("class", "active");
			}

			HtmlAnchor a = new HtmlAnchor();
			a.InnerHtml = (i + 1).ToString();

			string comment = string.Empty;
			if (Request.QueryString["comment"] != null)
			{
				comment = "&amp;comment=true";
			}

			a.HRef = "?q=" + Server.HtmlEncode(Request.QueryString["q"]) + comment + "&amp;page=" + (i + 1);

			li.Controls.Add(a);
			ul.Controls.Add(li);
		}
		Paging.Controls.Add(ul);
	}

	#endregion

	#region Data manipulation

	/// <summary>
	/// Removes the comment anchor from the URL
	/// </summary>
	protected string ShortenUrl(string uri)
	{
		string url = Utils.ConvertToAbsolute(uri).ToString();
		if (!url.Contains("#"))
			return url;

		int index = url.IndexOf("#");
		return url.Substring(0, index);
	}

	/// <summary>
	/// Shortens the content to fit to a search result
	/// </summary>
	protected string GetContent(string description, string content)
	{
		string text = string.Empty;
		if (!string.IsNullOrEmpty(description))
		{
			text = description;
		}
		else
		{
			text = StripHtml(content);
			if (text.Length > 200)
				text = text.Substring(0, 200) + " ...";
			text = "\"" + text.Trim() + "\"";
		}

		return text;
	}

	private static Regex _Regex = new Regex("<[^>]*>", RegexOptions.Compiled);
	/// <summary>
	/// Removes all HTML tags from a given string
	/// </summary>
	private static string StripHtml(string html)
	{
		if (string.IsNullOrEmpty(html))
			return string.Empty;

		return _Regex.Replace(html, string.Empty);
	}

	#endregion

}
