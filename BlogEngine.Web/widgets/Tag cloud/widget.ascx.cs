#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Collections.Generic;
using BlogEngine.Core;

#endregion

public partial class widgets_Tag_cloud_widget : WidgetBase
{

	public override void LoadWidget()
	{
		foreach (string key in WeightedList.Keys)
		{
			HtmlGenericControl li = new HtmlGenericControl("li");
			li.InnerHtml = string.Format(LINK, Utils.RelativeWebRoot + "?tag=/" + Utils.RemoveIllegalCharacters(key), WeightedList[key], "Tag: " + key, key);
			ulTags.Controls.Add(li);
		}
	}

	/// <summary>
	/// Initializes the <see cref="widgets_Tag_cloud_widget"/> class.
	/// </summary>
	static widgets_Tag_cloud_widget()
	{
		Post.Saved += delegate { Reload(); };
	}

	public static void Reload()
	{
		_WeightedList = null;
	}

	#region Private fields

	private const string LINK = "<a href=\"{0}\" class=\"{1}\" title=\"{2}\">{3}</a> ";
	private static Dictionary<string, string> _WeightedList;
	private static object _SyncRoot = new object();

	#endregion

	private int _MinimumPosts = 1;

	private int MinimumPosts
	{
		get 
		{
			StringDictionary settings = GetSettings();
			if (settings.ContainsKey("minimumposts"))
			{
				int.TryParse(settings["minimumposts"], out _MinimumPosts);
			}

			return _MinimumPosts; 		
		}
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
        SortedDictionary<string, int> dic = new SortedDictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
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
	/// Gets the name. It must be exactly the same as the folder that contains the widget.
	/// </summary>
	/// <value></value>
	public override string Name
	{
		get { return "Tag cloud"; }
	}

	/// <summary>
	/// Gets wether or not the widget can be edited.
	/// <remarks>
	/// The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
	/// </remarks>
	/// </summary>
	/// <value></value>
	public override bool IsEditable
	{
		get { return true; }
	}

}
