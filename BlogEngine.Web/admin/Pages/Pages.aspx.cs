#region Using

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;
using System.Collections.Generic;

#endregion

public partial class admin_Pages_pages : System.Web.UI.Page, System.Web.UI.ICallbackEventHandler
{
	protected void Page_Load(object sender, EventArgs e)
	{
		base.MaintainScrollPositionOnPostBack = true;

		if (!Page.IsPostBack && !Page.IsCallback)
		{
			if (!String.IsNullOrEmpty(Request.QueryString["id"]) && Request.QueryString["id"].Length == 36)
			{
				Guid id = new Guid(Request.QueryString["id"]);
				BindPage(id);
				BindParents(id);
			}
			else if (!String.IsNullOrEmpty(Request.QueryString["delete"]) && Request.QueryString["delete"].Length == 36)
			{
				Guid id = new Guid(Request.QueryString["delete"]);
				DeletePage(id);
			}
			else
			{
				BindParents(Guid.Empty);
			}

			BindPageList();
			Page.ClientScript.GetCallbackEventReference(this, "title", "ApplyCallback", "slug");
		}

		btnSave.Click += new EventHandler(btnSave_Click);
		btnSave.Text = Resources.labels.savePage; // mono does not interpret the inline code correctly
		btnUploadFile.Click += new EventHandler(btnUploadFile_Click);
		btnUploadImage.Click += new EventHandler(btnUploadImage_Click);
		Page.Title = Resources.labels.pages;

		if (!Utils.IsMono)
			Page.Form.DefaultButton = btnSave.UniqueID;
	}

	private void DeletePage(Guid pageId)
	{
		Page page = BlogEngine.Core.Page.GetPage(pageId);
		if (page != null)
		{
			ResetParentPage(page);
			page.Delete();
			page.Save();
			Response.Redirect("pages.aspx");
		}
	}

	private void ResetParentPage(Page page)
	{
		foreach (BlogEngine.Core.Page child in BlogEngine.Core.Page.Pages)
		{
			if (page.Id == child.Parent)
			{
				child.Parent = Guid.Empty;
				child.Save();
				ResetParentPage(child);
			}
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

	private void btnSave_Click(object sender, EventArgs e)
	{
		if (!Page.IsValid)
			throw new InvalidOperationException("One or more validators are invalid.");

		Page page;
		if (Request.QueryString["id"] != null)
			page = BlogEngine.Core.Page.GetPage(new Guid(Request.QueryString["id"]));
		else
			page = new Page();

		if (string.IsNullOrEmpty(txtContent.Text))
			txtContent.Text = "[No text]";

		page.Title = txtTitle.Text;
		page.Content = txtContent.Text;
		page.Description = txtDescription.Text;
		page.Keywords = txtKeyword.Text;

		if (cbIsFrontPage.Checked)
		{
			foreach (Page otherPage in BlogEngine.Core.Page.Pages)
			{
				if (otherPage.IsFrontPage)
				{
					otherPage.IsFrontPage = false;
					otherPage.Save();
				}
			}
		}

		page.IsFrontPage = cbIsFrontPage.Checked;
		page.ShowInList = cbShowInList.Checked;
		page.IsPublished = cbIsPublished.Checked;

        if (!string.IsNullOrEmpty(txtSlug.Text))
            page.Slug = Utils.RemoveIllegalCharacters(txtSlug.Text.Trim());

		if (ddlParent.SelectedIndex != 0)
			page.Parent = new Guid(ddlParent.SelectedValue);
		else
			page.Parent = Guid.Empty;

		page.Save();

		Response.Redirect(page.RelativeLink.ToString());
	}

	#endregion

	#region Data binding

	private void BindPage(Guid pageId)
	{
		Page page = BlogEngine.Core.Page.GetPage(pageId);
		txtTitle.Text = page.Title;
		txtContent.Text = page.Content;
		txtDescription.Text = page.Description;
		txtKeyword.Text = page.Keywords;
		txtSlug.Text = page.Slug;
		cbIsFrontPage.Checked = page.IsFrontPage;
		cbShowInList.Checked = page.ShowInList;
		cbIsPublished.Checked = page.IsPublished;
	}

	private void BindParents(Guid pageId)
	{
		foreach (Page page in BlogEngine.Core.Page.Pages)
		{
			if (pageId != page.Id)
				ddlParent.Items.Add(new ListItem(page.Title, page.Id.ToString()));
		}

		ddlParent.Items.Insert(0, "-- " + Resources.labels.noParent + " --");
		if (pageId != Guid.Empty)
		{
			Page parent = BlogEngine.Core.Page.GetPage(pageId);
			if (parent != null)
				ddlParent.SelectedValue = parent.Parent.ToString();
		}
	}

	private void BindPageList()
	{
		foreach (Page page in BlogEngine.Core.Page.Pages)
		{
			if (!page.HasParentPage)
			{
				HtmlGenericControl li = new HtmlGenericControl("li");
				HtmlAnchor a = new HtmlAnchor();
				a.HRef = "?id=" + page.Id.ToString();
				a.InnerHtml = page.Title;

				System.Web.UI.LiteralControl text = new System.Web.UI.LiteralControl
				(" (" + page.DateCreated.ToString("yyyy-dd-MM HH:mm") + ") ");

				string deleteText = "Are you sure you want to delete the page?";
				HtmlAnchor delete = new HtmlAnchor();
				delete.InnerText = Resources.labels.delete;
				delete.Attributes["onclick"] = "if (confirm('" + deleteText + "')){location.href='?delete=" + page.Id + "'}";
				delete.HRef = "javascript:void(0);";
				delete.Style.Add(System.Web.UI.HtmlTextWriterStyle.FontWeight, "normal");

				li.Controls.Add(a);
				li.Controls.Add(text);
				li.Controls.Add(delete);

				if (page.HasChildPages)
				{
					li.Controls.Add(BuildChildPageList(page));					
				}

				li.Attributes.CssStyle.Remove("font-weight");
				li.Attributes.CssStyle.Add("font-weight", "bold");

				ulPages.Controls.Add(li);
			}
		}

		divPages.Visible = true;
		aPages.InnerHtml = BlogEngine.Core.Page.Pages.Count + " " + Resources.labels.pages;
	}

	private HtmlGenericControl BuildChildPageList(BlogEngine.Core.Page page)
	{
		HtmlGenericControl ul = new HtmlGenericControl("ul");
		foreach (Page cPage in BlogEngine.Core.Page.Pages.FindAll(delegate(BlogEngine.Core.Page p)
		{
			//p => (p.Parent == page.Id)))
			return p.Parent == page.Id;
		}))
		{
			HtmlGenericControl cLi = new HtmlGenericControl("li");
			cLi.Attributes.CssStyle.Add("font-weight", "normal");
			HtmlAnchor cA = new HtmlAnchor();
			cA.HRef = "?id=" + cPage.Id.ToString();
			cA.InnerHtml = cPage.Title;

			System.Web.UI.LiteralControl cText = new System.Web.UI.LiteralControl
			(" (" + cPage.DateCreated.ToString("yyyy-dd-MM HH:mm") + ") ");

			string deleteText = "Are you sure you want to delete the page?";
			HtmlAnchor delete = new HtmlAnchor();
			delete.InnerText = Resources.labels.delete;
			delete.Attributes["onclick"] = "if (confirm('" + deleteText + "')){location.href='?delete=" + cPage.Id + "'}";
			delete.HRef = "javascript:void(0);";
			delete.Style.Add(System.Web.UI.HtmlTextWriterStyle.FontWeight, "normal");

			cLi.Controls.Add(cA);
			cLi.Controls.Add(cText);
			cLi.Controls.Add(delete);

			if (cPage.HasChildPages)
			{
				cLi.Attributes.CssStyle.Remove("font-weight");
				cLi.Attributes.CssStyle.Add("font-weight", "bold");
				cLi.Controls.Add(BuildChildPageList(cPage));
			}

			ul.Controls.Add(cLi);

		}
		return ul;
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
		_Callback = Utils.RemoveIllegalCharacters(eventArgument.Trim());
	}

	#endregion
}
