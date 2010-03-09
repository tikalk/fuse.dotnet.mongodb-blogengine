#region Using

using System;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Collections.Generic;
using BlogEngine.Core;

#endregion

namespace Controls
{
  public class TagCloud : Control
  {

    static TagCloud()
    {
      Post.Saved += delegate { _WeightedList = null; };
    }

    #region Private fields

    private const string LINK = "<a href=\"{0}\" class=\"{1}\" title=\"{2}\">{3}</a> ";
    private static Dictionary<string, string> _WeightedList;
    private static object _SyncRoot = new object();

    #endregion

		private int _MinimumPosts = 1;

		public int MinimumPosts
		{
			get { return _MinimumPosts; }
			set { _MinimumPosts = value; }
		}

    private Dictionary<string, string> WeightedList
    {
      get
      {
        if (_WeightedList == null)
        {
          lock (_SyncRoot)
          {
            if (_WeightedList == null)
            {
              _WeightedList = new Dictionary<string, string>();
              SortList();
            }
          }
        }

        return _WeightedList;
      }
    }

    /// <summary>
    /// Builds a raw list of all tags and the number of times
    /// they have been added to a post.
    /// </summary>
    private static SortedDictionary<string, int> CreateRawList()
    {
      SortedDictionary<string, int> dic = new SortedDictionary<string, int>();
      foreach (Post post in Post.Posts)
      {
        if (post.IsVisibleToPublic)
        {
          foreach (string tag in post.Tags)
          {
            if (dic.ContainsKey(tag))
              dic[tag]++;
            else
              dic[tag] = 1;
          }
        }
      }
      return dic;
    }

    /// <summary>
    /// Sorts the list of tags based on how much they are used.
    /// </summary>
    private void SortList()
    {
      SortedDictionary<string, int> dic = CreateRawList();
      int max = 0;
      foreach (int value in dic.Values)
      {
        if (value > max)
          max = value;
      }

      foreach (string key in dic.Keys)
      {
				if (dic[key] < MinimumPosts)
					continue;

        double weight = ((double)dic[key] / max) * 100;
        if (weight >= 99)
          _WeightedList.Add(key, "biggest");
        else if (weight >= 70)
          _WeightedList.Add(key, "big");
        else if (weight >= 40)
          _WeightedList.Add(key, "medium");
        else if (weight >= 20)
          _WeightedList.Add(key, "small");
        else if (weight >= 3)
          _WeightedList.Add(key, "smallest");
      }
    }

    /// <summary>
    /// Renders the control.
    /// </summary>
    public override void RenderControl(HtmlTextWriter writer)
    {
			if (WeightedList.Keys.Count == 0)
			{
				writer.Write("<p>" + Resources.labels.none + "</p>");
			}

      writer.Write("<ul id=\"tagcloud\" class=\"tagcloud\">");
      foreach (string key in WeightedList.Keys)
      {
        writer.Write("<li>");
        writer.Write(string.Format(LINK, Utils.RelativeWebRoot + "?tag=/" + HttpUtility.UrlEncode(key), WeightedList[key], "Tag: " + key, key));
        writer.Write("</li>");
      }

      writer.Write("</ul>");
      writer.Write(Environment.NewLine);
    }
  }
}