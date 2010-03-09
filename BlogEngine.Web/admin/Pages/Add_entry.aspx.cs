#region Using

using System;
using System.Web;
using System.Text;
using System.IO;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Threading;
using BlogEngine.Core;

#endregion

public partial class admin_entry : System.Web.UI.Page, System.Web.UI.ICallbackEventHandler
{
	private const string RAW_EDITOR_COOKIE = "useraweditor";

	protected void Page_Load(object sender, EventArgs e)
	{
		this.MaintainScrollPositionOnPostBack = true;
		txtTitle.Focus();
		BindTags();

		if (!Page.IsPostBack && !Page.IsCallback)
		{
			BindCategories();
			BindUsers();
			BindDrafts();

			Page.Title = Resources.labels.add_Entry;
			Page.ClientScript.GetCallbackEventReference(this, "title", "ApplyCallback", "slug");
			cbUseRaw.Text = Resources.labels.useRawHtmlEditor;
			if (!String.IsNullOrEmpty(Request.QueryString["id"]) && Request.QueryString["id"].Length == 36)
			{
				Guid id = new Guid(Request.QueryString["id"]);
				Page.Title = "Edit post";
				BindPost(id);
			}
			else
			{
				PreSelectAuthor(Page.User.Identity.Name);
				txtDate.Text = DateTime.Now.AddHours(BlogSettings.Instance.Timezone).ToString("yyyy-MM-dd HH\\:mm");
				cbEnableComments.Checked = BlogSettings.Instance.IsCommentsEnabled;
				if (Session["content"] != null)
				{
					txtContent.Text = Session["content"].ToString();
					txtRawContent.Text = txtContent.Text;
					txtTitle.Text = Session["title"].ToString();
					txtDescription.Text = Session["description"].ToString();
					txtSlug.Text = Session["slug"].ToString();
					txtTags.Text = Session["tags"].ToString();
				}
				BindBookmarklet();
			}

			if (!Page.User.IsInRole(BlogSettings.Instance.AdministratorRole))
				ddlAuthor.Enabled = false;

			cbEnableComments.Enabled = BlogSettings.Instance.IsCommentsEnabled;
			
			if (Request.Cookies[RAW_EDITOR_COOKIE] != null)
			{
				txtRawContent.Visible = true;
				txtContent.Visible = false;
				cbUseRaw.Checked = true;
			}

			if (!Utils.IsMono && !cbUseRaw.Checked)
				Page.Form.DefaultButton = btnSave.UniqueID;
		}

		btnSave.Text = Resources.labels.savePost; // mono does not interpret the inline code correctly
		btnSave.Click += new EventHandler(btnSave_Click);
		btnCategory.Click += new EventHandler(btnCategory_Click);
		btnUploadFile.Click += new EventHandler(btnUploadFile_Click);
		btnUploadImage.Click += new EventHandler(btnUploadImage_Click);
		valExist.ServerValidate += new ServerValidateEventHandler(valExist_ServerValidate);
		cbUseRaw.CheckedChanged += new EventHandler(cbUseRaw_CheckedChanged);
	}

	void cbUseRaw_CheckedChanged(object sender, EventArgs e)
	{
		if (cbUseRaw.Checked)
		{
			txtRawContent.Text = txtContent.Text;
			HttpCookie cookie = new HttpCookie(RAW_EDITOR_COOKIE, "1");
			cookie.Expires = DateTime.Now.AddYears(3);
			Response.Cookies.Add(cookie);
		}
		else
		{
			txtContent.Text = txtRawContent.Text;
			if (Request.Cookies[RAW_EDITOR_COOKIE] != null)
			{
				HttpCookie cookie = new HttpCookie(RAW_EDITOR_COOKIE);
				cookie.Expires = DateTime.Now.AddYears(-3);
				Response.Cookies.Add(cookie);
			}			
		}

		txtRawContent.Visible = cbUseRaw.Checked;
		txtContent.Visible = !cbUseRaw.Checked;

		//Response.Redirect(Request.RawUrl);
	}

	private void valExist_ServerValidate(object source, ServerValidateEventArgs args)
	{
		args.IsValid = true;

		foreach (Category cat in Category.Categories)
		{
			if (cat.Title.Equals(txtCategory.Text.Trim(), StringComparison.OrdinalIgnoreCase))
				args.IsValid = false;
		}
	}

	private void btnUploadImage_Click(object sender, EventArgs e)
	{
		string relativeFolder = DateTime.Now.Year.ToString() + Path.DirectorySeparatorChar + DateTime.Now.Month.ToString() + Path.DirectorySeparatorChar;
		string folder = BlogSettings.Instance.StorageLocation + "files" + Path.DirectorySeparatorChar;
		string fileName = txtUploadImage.FileName;
		Upload(folder + relativeFolder, txtUploadImage, fileName);

		string path = Utils.RelativeWebRoot.ToString();
		string img = string.Format("<img src=\"{0}image.axd?picture={1}\" alt=\"\" />", path, Server.UrlEncode(relativeFolder.Replace("\\", "/") + fileName));
		txtContent.Text += img;
		txtRawContent.Text += img;
	}

	private void btnUploadFile_Click(object sender, EventArgs e)
	{
		string relativeFolder = DateTime.Now.Year.ToString() + Path.DirectorySeparatorChar + DateTime.Now.Month.ToString() + Path.DirectorySeparatorChar;
		string folder = BlogSettings.Instance.StorageLocation + "files" + Path.DirectorySeparatorChar;
		string fileName = txtUploadFile.FileName;
		Upload(folder + relativeFolder, txtUploadFile, fileName);

		string a = "<p><a href=\"{0}file.axd?file={1}\">{2}</a></p>";
		string text = txtUploadFile.FileName + " (" + SizeFormat(txtUploadFile.FileBytes.Length, "N") + ")";
		txtContent.Text += string.Format(a, Utils.RelativeWebRoot, Server.UrlEncode(relativeFolder.Replace("\\", "/") + fileName), text);
		txtRawContent.Text += string.Format(a, Utils.RelativeWebRoot, Server.UrlEncode(relativeFolder.Replace("\\", "/") + fileName), text);
	}

	private void Upload(string virtualFolder, FileUpload control, string fileName)
	{
		string folder = Server.MapPath(virtualFolder);
		if (!Directory.Exists(folder))
			Directory.CreateDirectory(folder);

		control.PostedFile.SaveAs(folder + fileName);
	}

	private string SizeFormat(float size, string formatString)
	{
		if (size < 1024)
			return size.ToString(formatString) + " bytes";

		if (size < Math.Pow(1024, 2))
			return (size / 1024).ToString(formatString) + " kb";

		if (size < Math.Pow(1024, 3))
			return (size / Math.Pow(1024, 2)).ToString(formatString) + " mb";

		if (size < Math.Pow(1024, 4))
			return (size / Math.Pow(1024, 3)).ToString(formatString) + " gb";

		return size.ToString(formatString);
	}

	#region Event handlers

	/// <summary>
	/// Creates and saves a new category
	/// </summary>
	private void btnCategory_Click(object sender, EventArgs e)
	{
		if (Page.IsValid)
		{
			Category cat = new Category(txtCategory.Text, string.Empty);
			cat.Save();
			ListItem item = new ListItem(Server.HtmlEncode(txtCategory.Text), cat.Id.ToString());
			item.Selected = true;
			cblCategories.Items.Add(item);
		}
	}

	/// <summary>
	/// Saves the post
	/// </summary>
	private void btnSave_Click(object sender, EventArgs e)
	{
		if (!Page.IsValid)
			throw new InvalidOperationException("One or more validators are invalid.");

		Post post;
		if (Request.QueryString["id"] != null)
			post = Post.GetPost(new Guid(Request.QueryString["id"]));
		else
			post = new Post();

		if (cbUseRaw.Checked)
			txtContent.Text = txtRawContent.Text;

		if (string.IsNullOrEmpty(txtContent.Text))
			txtContent.Text = "[No text]";

		post.DateCreated = DateTime.ParseExact(txtDate.Text, "yyyy-MM-dd HH\\:mm", null).AddHours(-BlogSettings.Instance.Timezone);
		post.Author = ddlAuthor.SelectedValue;
		post.Title = txtTitle.Text.Trim();
		post.Content = txtContent.Text;
		post.Description = txtDescription.Text.Trim();
		post.IsPublished = cbPublish.Checked;
		post.IsCommentsEnabled = cbEnableComments.Checked;

		if (!string.IsNullOrEmpty(txtSlug.Text))
			post.Slug = Utils.RemoveIllegalCharacters(txtSlug.Text.Trim());

		post.Categories.Clear();

		foreach (ListItem item in cblCategories.Items)
		{
			if (item.Selected)
				post.Categories.Add(Category.GetCategory(new Guid(item.Value)));
		}

		post.Tags.Clear();
		if (txtTags.Text.Trim().Length > 0)
		{
			string[] tags = txtTags.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string tag in tags)
			{
                if (string.IsNullOrEmpty(post.Tags.Find(delegate (string t) { return t.Equals(tag.Trim(), StringComparison.OrdinalIgnoreCase); })))
                {
                    post.Tags.Add(tag.Trim());
                }				
			}
		}

		post.Save();

		Session.Remove("content");
		Session.Remove("title");
		Session.Remove("description");
		Session.Remove("slug");
		Session.Remove("tags");
		Response.Redirect(post.RelativeLink.ToString());
	}

	#endregion

	#region Data binding

	private void BindTags()
	{
		System.Collections.Generic.List<string> col = new System.Collections.Generic.List<string>();
		foreach (Post post in Post.Posts)
		{
			foreach (string tag in post.Tags)
			{
				if (!col.Contains(tag))
					col.Add(tag);
			}
		}

		col.Sort(delegate(string s1, string s2) { return String.Compare(s1, s2); });

		foreach (string tag in col)
		{
			HtmlAnchor a = new HtmlAnchor();
			a.HRef = "javascript:void(0)";
			a.Attributes.Add("onclick", "AddTag(this)");
			a.InnerText = tag;
			phTags.Controls.Add(a);
		}
	}

	private void BindCategories()
	{
		foreach (Category cat in Category.Categories)
		{
			cblCategories.Items.Add(new ListItem(Server.HtmlEncode(cat.Title), cat.Id.ToString()));
		}
	}

	private void BindPost(Guid postId)
	{
		Post post = Post.GetPost(postId);
		txtTitle.Text = post.Title;
		txtContent.Text = post.Content;
		txtRawContent.Text = post.Content;
		txtDescription.Text = post.Description;
		txtDate.Text = post.DateCreated.ToString("yyyy-MM-dd HH\\:mm");
		cbEnableComments.Checked = post.IsCommentsEnabled;
		cbPublish.Checked = post.IsPublished;
		txtSlug.Text = Utils.RemoveIllegalCharacters(post.Slug);

		PreSelectAuthor(post.Author);

		foreach (Category cat in post.Categories)
		{
			ListItem item = cblCategories.Items.FindByValue(cat.Id.ToString());
			if (item != null)
				item.Selected = true;
		}

		string[] tags = new string[post.Tags.Count];
		for (int i = 0; i < post.Tags.Count; i++)
		{
			tags[i] = post.Tags[i];
		}
		txtTags.Text = string.Join(",", tags);
	}

	private void PreSelectAuthor(string author)
	{
		ddlAuthor.ClearSelection();
		foreach (ListItem item in ddlAuthor.Items)
		{
			if (item.Text.Equals(author, StringComparison.OrdinalIgnoreCase))
			{
				item.Selected = true;
				break;
			}
		}
	}

	private void BindBookmarklet()
	{
		if (Request.QueryString["title"] != null && Request.QueryString["url"] != null)
		{
			string title = Request.QueryString["title"];
			string url = Request.QueryString["url"];

			txtTitle.Text = title;
			txtContent.Text = string.Format("<p><a href=\"{0}\" title=\"{1}\">{1}</a></p>", url, title);
		}
	}

	private void BindUsers()
	{
		foreach (MembershipUser user in Membership.GetAllUsers())
		{
			ddlAuthor.Items.Add(user.UserName);
		}
	}

	private void BindDrafts()
	{
		Guid id = Guid.Empty;
		if (!String.IsNullOrEmpty(Request.QueryString["id"]) && Request.QueryString["id"].Length == 36)
		{
			id = new Guid(Request.QueryString["id"]);
		}

		int counter = 0;

		foreach (Post post in Post.Posts)
		{
			if (!post.IsPublished && post.Id != id)
			{
				HtmlGenericControl li = new HtmlGenericControl("li");
				HtmlAnchor a = new HtmlAnchor();
				a.HRef = "?id=" + post.Id.ToString();
				a.InnerHtml = post.Title;

				System.Web.UI.LiteralControl text = new System.Web.UI.LiteralControl(" by " + post.Author + " (" + post.DateCreated.ToString("yyyy-dd-MM HH\\:mm") + ")");

				li.Controls.Add(a);
				li.Controls.Add(text);
				ulDrafts.Controls.Add(li);
				counter++;
			}
		}

		if (counter > 0)
		{
			divDrafts.Visible = true;
			aDrafts.InnerHtml = string.Format(Resources.labels.thereAreXDrafts, counter);
		}
	}

	#endregion


	#region ICallbackEventHandler Members

	private string _Callback;

	public string GetCallbackResult()
	{
		return _Callback;
	}

	public void RaiseCallbackEvent(string eventArgument)
	{
		if (eventArgument.StartsWith("_autosave"))
		{
			string[] fields = eventArgument.Replace("_autosave", string.Empty).Split(new string[] { ";|;" }, StringSplitOptions.None);
			Session["content"] = fields[0];
			Session["title"] = fields[1];
			Session["description"] = fields[2];
			Session["slug"] = fields[3];
			Session["tags"] = fields[4];
		}
		else
		{
			_Callback = Utils.RemoveIllegalCharacters(eventArgument.Trim());
		}
	}

	#endregion
}
