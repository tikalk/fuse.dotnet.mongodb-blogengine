#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Collections.Specialized;

#endregion

public partial class widgets_LinkList_edit : WidgetEditBase
{

	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			BindGrid();
		}
		
		grid.RowEditing += new GridViewEditEventHandler(grid_RowEditing);
		grid.RowUpdating += new GridViewUpdateEventHandler(grid_RowUpdating);
		grid.RowCancelingEdit += delegate { grid.EditIndex = -1; };
		grid.RowDeleting += new GridViewDeleteEventHandler(grid_RowDeleting);
		btnAdd.Click += new EventHandler(btnAdd_Click);
	}

	void btnAdd_Click(object sender, EventArgs e)
	{
    XmlDocument doc = Doc();
		XmlNode links = doc.SelectSingleNode("links");
		if (links == null)
		{
			links = doc.CreateElement("links");
			doc.AppendChild(links);
		}

		XmlNode link = doc.CreateElement("link");
	
		XmlAttribute id = doc.CreateAttribute("id");
		id.InnerText = Guid.NewGuid().ToString();
		link.Attributes.Append(id);

		XmlAttribute title = doc.CreateAttribute("title");
		title.InnerText = txtTitle.Text.Trim(); ;
		link.Attributes.Append(title);

		XmlAttribute url = doc.CreateAttribute("url");
		url.InnerText = txtUrl.Text.Trim();
		link.Attributes.Append(url);

		XmlAttribute newwindow = doc.CreateAttribute("newwindow");
		newwindow.InnerText = cbNewWindow.Checked.ToString();
		link.Attributes.Append(newwindow);

		links.AppendChild(link);
    Save(doc);
		BindGrid();
	}

	private void BindGrid()
	{
    XmlDocument doc = Doc();
		XmlNodeList list = doc.SelectNodes("//link");
		if (list.Count > 0)
		{			
			using (XmlTextReader reader = new XmlTextReader(doc.OuterXml, XmlNodeType.Document, null))
			{
				System.Data.DataSet ds = new System.Data.DataSet();
				ds.ReadXml(reader);
				grid.DataSource = ds;
				grid.DataKeyNames = new string[] { "id" };
				grid.DataBind();
				ds.Dispose();
			}			
		}
	}

	/// <summary>
	/// Handles the RowDeleting event of the grid control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewDeleteEventArgs"/> instance containing the event data.</param>
	void grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
	{
    XmlDocument doc = Doc();
		string id = (string)grid.DataKeys[e.RowIndex].Value;
		XmlNode node = doc.SelectSingleNode("//link[@id=\"" + id + "\"]");
		if (node != null)
		{
			node.ParentNode.RemoveChild(node);
      Save(doc);
			BindGrid();
		}
	}

	/// <summary>
	/// Handles the RowUpdating event of the grid control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewUpdateEventArgs"/> instance containing the event data.</param>
	void grid_RowUpdating(object sender, GridViewUpdateEventArgs e)
	{
    XmlDocument doc = Doc();
		string id = (string)grid.DataKeys[e.RowIndex].Value;
		TextBox textboxTitle = (TextBox)grid.Rows[e.RowIndex].FindControl("txtTitle");
		TextBox textboxUrl = (TextBox)grid.Rows[e.RowIndex].FindControl("txtUrl");
		CheckBox checkboxNewWindow = (CheckBox)grid.Rows[e.RowIndex].FindControl("cbNewWindow");
		XmlNode node = doc.SelectSingleNode("//link[@id=\"" + id + "\"]");
		
		if (node != null)
		{
			node.Attributes["title"].InnerText = textboxTitle.Text;
			node.Attributes["url"].InnerText = textboxUrl.Text;
			node.Attributes["newwindow"].InnerText = checkboxNewWindow.Checked.ToString();
			grid.EditIndex = -1;
      Save(doc);
			BindGrid();
		}
	}

	/// <summary>
	/// Handles the RowEditing event of the grid control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewEditEventArgs"/> instance containing the event data.</param>
	void grid_RowEditing(object sender, GridViewEditEventArgs e)
	{
		grid.EditIndex = e.NewEditIndex;
		BindGrid();
	}

	/// <summary>
	/// Saves this the basic widget settings such as the Title.
	/// </summary>
	public override void Save()
	{
    XmlDocument doc = Doc();
	}

  void Save(XmlDocument doc)
  {
    StringDictionary settings = GetSettings();
    settings["content"] = doc.InnerXml;
    SaveSettings(settings);
  }

  XmlDocument Doc()
  {
    StringDictionary settings = GetSettings();
    XmlDocument doc = new XmlDocument();
    if (settings["content"] != null)
      doc.InnerXml = settings["content"];

    return doc;
  }
}
