#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using BlogEngine.Core;
using System.Collections.Specialized;
using System.Web.Security;

#endregion

public partial class widgets_Most_comments_widget : WidgetBase
{

	#region Private variables

	private int AVATAR_SIZE = 50;
	private int NUMBER_OF_VISITORS = 3;
	private int DAYS = 60;
	private bool SHOW_COMMENTS = true;

	#endregion

	static widgets_Most_comments_widget()
	{
		Post.CommentAdded += delegate { HttpRuntime.Cache.Remove("most_comments"); };
	}

	/// <summary>
	/// Gets the name. It must be exactly the same as the folder that contains the widget.
	/// </summary>
	public override string Name
	{
		get { return "Most comments"; }
	}

	/// <summary>
	/// Gets wether or not the widget can be edited.
	/// <remarks>
	/// The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
	/// </remarks>
	/// </summary>
	public override bool IsEditable
	{
		get { return true; }
	}

	/// <summary>
	/// This method works as a substitute for Page_Load. You should use this method for
	/// data binding etc. instead of Page_Load.
	/// </summary>
	public override void LoadWidget()
	{
		LoadSettings();

		if (Cache["most_comments"] == null)
		{
			BuildList();
		}

		List<Visitor> visitors = (List<Visitor>)Cache["most_comments"];

		rep.ItemDataBound += new RepeaterItemEventHandler(rep_ItemDataBound);
		rep.DataSource = visitors;
		rep.DataBind();
	}

	private void LoadSettings()
	{
		StringDictionary settings = GetSettings();
		try
		{
			if (settings.ContainsKey("avatarsize"))
				AVATAR_SIZE = int.Parse(settings["avatarsize"]);

			if (settings.ContainsKey("numberofvisitors"))
				NUMBER_OF_VISITORS = int.Parse(settings["numberofvisitors"]);

			if (settings.ContainsKey("days"))
				DAYS = int.Parse(settings["days"]);

			if (settings.ContainsKey("showcomments"))
				SHOW_COMMENTS = settings["showcomments"].Equals("true", StringComparison.OrdinalIgnoreCase);
		}
		catch
		{ }
	}

	void rep_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		{
			Visitor visitor = (Visitor)e.Item.DataItem;
			Image imgAvatar = (Image)e.Item.FindControl("imgAvatar");
			Image imgCountry = (Image)e.Item.FindControl("imgCountry");
			Literal name = (Literal)e.Item.FindControl("litName");
			Literal number = (Literal)e.Item.FindControl("litNumber");
			Literal litCountry = (Literal)e.Item.FindControl("litCountry");

			imgAvatar.ImageUrl = Gravatar(AVATAR_SIZE, visitor.Email);
			imgAvatar.AlternateText = visitor.Name;
			imgAvatar.Width = Unit.Pixel(AVATAR_SIZE);

			if (SHOW_COMMENTS)
				number.Text = visitor.Comments + " " + Resources.labels.comments.ToLowerInvariant() + "<br />";

			if (visitor.Website != null)
			{
				string link = "<a rel=\"nofollow contact\" class=\"url fn\" href=\"{0}\">{1}</a>";
				name.Text = string.Format(link, visitor.Website, visitor.Name);
			}
			else
			{
				name.Text = "<span class=\"fn\">" + visitor.Name + "</span>";
			}

			if (!string.IsNullOrEmpty(visitor.Country))
			{
				imgCountry.ImageUrl = Utils.RelativeWebRoot + "pics/flags/" + visitor.Country + ".png";
				imgCountry.AlternateText = visitor.Country;

				foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
				{
					RegionInfo ri = new RegionInfo(ci.Name);
					if (ri.TwoLetterISORegionName.Equals(visitor.Country, StringComparison.OrdinalIgnoreCase))
					{
						litCountry.Text = ri.DisplayName;
						break;
					}
				}
			}
			else
			{
				imgCountry.Visible = false;
			}
		}
	}

	#region Gravatar

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

	#endregion

	#region Build the list

	private void BuildList()
	{
		Dictionary<string, int> visitors = new Dictionary<string, int>();
		foreach (Post post in Post.Posts)
		{
			foreach (Comment comment in post.ApprovedComments)
			{
				if (comment.DateCreated.AddDays(DAYS) < DateTime.Now || string.IsNullOrEmpty(comment.Email) || !comment.Email.Contains("@"))
					continue;

				if (post.Author.Equals(comment.Author, StringComparison.OrdinalIgnoreCase))
					continue;

				if (visitors.ContainsKey(comment.Email))
				{
					visitors[comment.Email] += 1;
				}
				else
				{
					visitors.Add(comment.Email, 1);
				}
			}
		}

		visitors = SortDictionary(visitors);
		int max = Math.Min(visitors.Count, NUMBER_OF_VISITORS);
		int count = 0;
		List<Visitor> list = new List<Visitor>();

		foreach (string key in visitors.Keys)
		{
			Visitor v = FindVisitor(key, visitors[key]);
			list.Add(v);

			count++;

			if (count == NUMBER_OF_VISITORS)
				break;
		}

		Cache.Insert("most_comments", list);
	}

	private Visitor FindVisitor(string email, int comments)
	{
		foreach (Post post in Post.Posts)
		{
			foreach (Comment comment in post.ApprovedComments)
			{
				if (comment.Email == email)
				{
					Visitor visitor = new Visitor();
					visitor.Name = comment.Author;
					visitor.Country = comment.Country;
					visitor.Website = comment.Website;
					visitor.Comments = comments;
					visitor.Email = email;
					return visitor;
				}
			}
		}

		return new Visitor();
	}

	private static Dictionary<string, int> SortDictionary(Dictionary<string, int> dic)
	{
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
		foreach (string key in dic.Keys)
		{
			list.Add(new KeyValuePair<string, int>(key, dic[key]));
		}

		list.Sort(delegate(KeyValuePair<string, int> obj1, KeyValuePair<string, int> obj2)
		{
			return obj2.Value.CompareTo(obj1.Value);
		});

		Dictionary<string, int> sortedDic = new Dictionary<string, int>();
		foreach (KeyValuePair<string, int> pair in list)
		{
			sortedDic.Add(pair.Key, pair.Value);
		}

		return sortedDic;
	}

	#endregion

}

public struct Visitor
{
	public string Name;
	public string Country;
	public Uri Website;
	public int Comments;
	public string Email;
}