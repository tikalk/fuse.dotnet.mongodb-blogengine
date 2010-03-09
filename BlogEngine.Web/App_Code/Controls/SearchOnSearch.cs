#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using BlogEngine.Core;

#endregion

namespace Controls
{
	/// <summary>
	/// If the visitor arrives through a search engine, this control
	/// will display an in-site search result based on the same search term.
	/// </summary>
	public class SearchOnSearch : Control
	{
		#region Class Data

		private static Regex _rxSearchTerm = null;

		#endregion


		#region Class Constructor

		static SearchOnSearch()
		{
			// Matches the query string parameter "q" and its value.  Does not match if "q" is blank.
			_rxSearchTerm = new Regex("[?&]q=([^&#]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		}

		#endregion

		#region Properties

		private int _MaxResults;
		/// <summary>
		/// Gets or sets the maximum numbers of results to display.
		/// </summary>
		public int MaxResults
		{
			get { return _MaxResults; }
			set { _MaxResults = value; }
		}

		private string _Headline;
		/// <summary>
		/// Gets or sets the text of the headline.
		/// </summary>
		public string Headline
		{
			get { return _Headline; }
			set { _Headline = value; }
		}

		private string _Text;
		/// <summary>
		/// Gets or sets the text displayed below the headline.
		/// </summary>
		public string Text
		{
			get { return _Text; }
			set { _Text = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks the referrer to see if it qualifies as a search.
		/// </summary>
		private string Html()
		{
			if (Context.Request.UrlReferrer != null && !Context.Request.UrlReferrer.ToString().Contains(Utils.AbsoluteWebRoot.ToString()) && IsSearch)
			{
				string referrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLowerInvariant();
				string searchTerm = GetSearchTerm(referrer);
				List<IPublishable> items = Search.Hits(searchTerm, false);
				if (items.Count == 0)
					return null;

				return WriteHtml(items, searchTerm);
			}

			return null;
		}

		/// <summary>
		/// Writes the search results as HTML.
		/// </summary>
		private string WriteHtml(List<IPublishable> items, string searchTerm)
		{
			int results = MaxResults < items.Count ? MaxResults : items.Count;
			StringBuilder sb = new StringBuilder();
			sb.Append("<div id=\"searchonsearch\">");
			sb.AppendFormat("<h3>{0} '{1}'</h3>", Headline, HttpUtility.HtmlEncode(HttpUtility.UrlDecode(searchTerm)));
			sb.AppendFormat("<p>{0}</p>", Text);
			sb.Append("<ol>");

			for (int i = 0; i < results; i++)
			{
				sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", items[i].RelativeLink, items[i].Title);
			}

			sb.Append("</ol>");
			sb.Append("</div>");

			return sb.ToString();
		}

		/// <summary>
		/// Retrieves the search term from the specified referrer string.
		/// </summary>
		private string GetSearchTerm(string referrer)
		{
			string term = string.Empty;
			Match match = _rxSearchTerm.Match(referrer);

			if (match.Success)
			{
				term = match.Groups[1].Value;
			}

			return term.Replace("+", " ");
		}

		/// <summary>
		/// Checks the referrer to see if it is from a search engine.
		/// </summary>
		private bool IsSearch
		{
			get
			{
				string referrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLowerInvariant();
				return _rxSearchTerm.IsMatch(referrer);
			}
		}

		#endregion

		/// <summary>
		/// Renders the control as a script tag.
		/// </summary>
		public override void RenderControl(HtmlTextWriter writer)
		{
			string html = Html();
			if (html != null)
				writer.Write(html);
		}
	}
}