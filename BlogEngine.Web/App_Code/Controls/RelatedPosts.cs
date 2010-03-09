#region Using

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.IO;
using BlogEngine.Core;

#endregion

namespace Controls
{

	public class RelatedPosts : Control
	{

		static RelatedPosts()
		{
			Post.Saved += new EventHandler<SavedEventArgs>(Post_Saved);
		}

		static void Post_Saved(object sender, SavedEventArgs e)
		{
			if (e.Action == SaveAction.Update)
			{
				Post post = (Post)sender;
				if (_Cache.ContainsKey(post.Id))
					_Cache.Remove(post.Id);
			}
		}

		#region Properties

		private IPublishable _Item;

		public IPublishable Item
		{
			get { return _Item; }
			set { _Item = value; }
		}

		private int _MaxResults = 3;

		public int MaxResults
		{
			get { return _MaxResults; }
			set { _MaxResults = value; }
		}

		private bool _ShowDescription;

		public bool ShowDescription
		{
			get { return _ShowDescription; }
			set { _ShowDescription = value; }
		}

		private int _DescriptionMaxLength = 100;

		public int DescriptionMaxLength
		{
			get { return _DescriptionMaxLength; }
			set { _DescriptionMaxLength = value; }
		}

		private string _Headline = Resources.labels.relatedPosts;

		public string Headline
		{
			get { return _Headline; }
			set { _Headline = value; }
		}

		#endregion

		#region Private fiels

		private static Dictionary<Guid, string> _Cache = new Dictionary<Guid, string>();
		private static object _SyncRoot = new object();

		#endregion

		/// <summary>
		/// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object and stores tracing information about the control if tracing is enabled.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HTmlTextWriter"></see> object that receives the control content.</param>
		public override void RenderControl(HtmlTextWriter writer)
		{
			if (!BlogSettings.Instance.EnableRelatedPosts || Item == null)
				return;

			if (!_Cache.ContainsKey(Item.Id))
			{
				lock (_SyncRoot)
				{
					if (!_Cache.ContainsKey(Item.Id))
					{
						List<IPublishable> relatedPosts = SearchForPosts();
						if (relatedPosts.Count <= 1)
							return;

						CreateList(relatedPosts);
					}
				}
			}

			writer.Write(_Cache[Item.Id].Replace("+++", this.Headline));
		}

		/// <summary>
		/// Creates the list of related posts in HTML.
		/// </summary>
		/// <param name="relatedPosts">The related posts.</param>
		private void CreateList(List<IPublishable> relatedPosts)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			string link = "<a href=\"{0}\">{1}</a>";
			string desc = "<span>{0}</span>";
			sb.Append("<div id=\"relatedPosts\">");
			sb.Append("<p>+++</p>");
			sb.Append("<div>");
			
			int count = 0;
			foreach (IPublishable post in relatedPosts)
			{
				if (post != this.Item)
				{
					sb.Append(string.Format(link, post.RelativeLink, HttpUtility.HtmlEncode(post.Title)));
					if (ShowDescription)
					{
						string description = post.Description;
						if (description != null && description.Length > DescriptionMaxLength)
							description = description.Substring(0, DescriptionMaxLength) + "...";

						if (String.IsNullOrEmpty(description))
						{
							string content = Utils.StripHtml(post.Content);
							description = content.Length > DescriptionMaxLength ? content.Substring(0, DescriptionMaxLength) + "..." : content;
						}

						sb.Append(string.Format(desc, description));
					}
					count++;
				}

				if (count == MaxResults)
					break;
			}

			sb.Append("</div>");
			sb.Append("</div>");
			_Cache.Add(Item.Id, sb.ToString());
		}

		private List<IPublishable> SearchForPosts()
		{
			return Search.FindRelatedItems(this.Item);
		}
	}
}