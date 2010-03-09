#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Hosting;
using System.Xml;
using System.IO;
using System.Text;
using BlogEngine.Core;
using BlogEngine.Core.DataStore;

#endregion

public partial class User_controls_WidgetEditor : System.Web.UI.Page
{
	#region Event handlers

	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Init(object sender, EventArgs e)
	{
		if (!User.IsInRole(BlogSettings.Instance.AdministratorRole))
		{
			Response.StatusCode = 403;
			Response.Clear();
			Response.End();
		}

		string widget = Request.QueryString["widget"];
		string id = Request.QueryString["id"];
		string move = Request.QueryString["move"];
		string add = Request.QueryString["add"];
		string remove = Request.QueryString["remove"];
        string zone = Request.QueryString["zone"];
        string getMoveItems = Request.QueryString["getmoveitems"];
        
		if (!string.IsNullOrEmpty(widget) && !string.IsNullOrEmpty(id))
			InitEditor(widget, id, zone);

		if (!string.IsNullOrEmpty(move))
			MoveWidgets(move);

		if (!string.IsNullOrEmpty(add))
			AddWidget(add, zone);

		if (!string.IsNullOrEmpty(remove) && remove.Length == 36)
			RemoveWidget(remove, zone);

        if (!string.IsNullOrEmpty(getMoveItems))
            GetMoveItems(getMoveItems);
            
	}    

	/// <summary>
	/// Handles the Click event of the btnSave control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	private void btnSave_Click(object sender, EventArgs e)
	{
		WidgetEditBase widget = (WidgetEditBase)FindControl("widget");
        string zone = Request.QueryString["zone"];

		if (widget != null)
			widget.Save();

		XmlDocument doc = GetXmlDocument(zone);
		XmlNode node = doc.SelectSingleNode("//widget[@id=\"" + Request.QueryString["id"] + "\"]");
		bool isChanged = false;

		if (node.Attributes["title"].InnerText != txtTitle.Text.Trim())
		{
			node.Attributes["title"].InnerText = txtTitle.Text.Trim();
			isChanged = true;
		}

		if (node.Attributes["showTitle"].InnerText != cbShowTitle.Checked.ToString())
		{
			node.Attributes["showTitle"].InnerText = cbShowTitle.Checked.ToString();
			isChanged = true;
		}

		if (isChanged)
			SaveXmlDocument(doc, zone);

		WidgetEditBase.OnSaved();
		Cache.Remove("widget_" + Request.QueryString["id"]);

        // To avoid JS errors with TextBox widget loading tinyMce scripts while
        // the edit window is closing, don't output phEdit.
        phEdit.Visible = false;

        string script = "PostEdit();";
		Page.ClientScript.RegisterStartupScript(this.GetType(), "closeWindow", script, true);
	}

	#endregion

    /// <summary>
    /// Sends a list of the widget zones and containing widgets in the Response.
    /// </summary>
    /// <param name="zoneNames">The list of zones to retrieve a list of widgets for.</param>
    private void GetMoveItems(string zoneNames)
    {
        string moveWidgetTo = (string)GetGlobalResourceObject("labels", "moveWidgetTo") ?? string.Empty;

        StringBuilder sb = new StringBuilder();
        sb.Append("{ moveWidgetTo: '" + moveWidgetTo.Replace("'", "\\'") + "', zones: [");
        if (!string.IsNullOrEmpty(zoneNames))
        {
            bool isFirstZone = true;
            string[] zones = zoneNames.Split(',');
            foreach (string zone in zones)
            {
                if (!isFirstZone)
                    sb.Append(",");

                // Zone names won't have single quotation marks in their names, because they were ran
                // through Utils.RemoveIllegalCharacters().  Therefore, escaping is unnecessary.
                sb.Append(" { zoneName: '" + zone + "', widgets: [ ");

                XmlDocument doc = GetXmlDocument(zone);

                bool isFirstWidget = true;
                foreach (XmlNode node in doc.SelectSingleNode("widgets").ChildNodes)
                {
                    if (!isFirstWidget)
                        sb.Append(",");

                    // Shorten excessively long titles.  Since the title/description is only
                    // used for display purposes in the "move to" dropdown list, this is okay.
                    // Also strip HTML.  The Visitor Widget (for example) has an <img> tag in
                    // its Title by default which causes issues when added an an option to the
                    // dropdown list.
                    string title = Utils.StripHtml(node.Attributes["title"].InnerText ?? string.Empty);
                    if (title.Length > 20)
                        title = title.Substring(0, 20) + "...";

                    // Need to escape single quotation marks in widget type and widget title.
                    string description =
                        Utils.StripHtml(node.InnerText).Replace("'", "\\'") +
                        (!string.IsNullOrEmpty(title) &&
                         bool.Parse(node.Attributes["showTitle"].InnerText) ?
                         " (" + title.Replace("'", "\\'") + ")" : string.Empty);
                    
                    sb.Append(
                        " { id: '" + node.Attributes["id"].InnerText + "', " +
                        " desc: '" + description + "' } ");

                    isFirstWidget = false;
                }

                sb.Append(" ] } ");
                isFirstZone = false;
            }
        }
        sb.Append(" ] } ");
        Response.Clear();
        Response.Write(sb.ToString());
        Response.End();
    }

	/// <summary>
	/// Removes the widget from the XML file.
	/// </summary>
	/// <param name="id">The id of the widget to remove.</param>
    /// <param name="zone">The zone a widget is being removed from.</param>
	private void RemoveWidget(string id, string zone)
	{
		XmlDocument doc = GetXmlDocument(zone);
		XmlNode node = doc.SelectSingleNode("//widget[@id=\"" + id + "\"]");
		if (node != null)
		{
			// remove widget reference in the widget zone
			node.ParentNode.RemoveChild(node);
			SaveXmlDocument(doc, zone);

			// remove widget itself
			BlogEngine.Core.Providers.BlogService.RemoveFromDataStore(ExtensionType.Widget, id);
			Cache.Remove("be_widget_" + id);

			WidgetEditBase.OnSaved();

            Response.Clear();
            Response.Write(id + zone);
            Response.End();
		}
	}

	/// <summary>
	/// Adds a widget of the specified type.
	/// </summary>
	/// <param name="type">The type of widget.</param>
    /// <param name="zone">The zone a widget is being added to.</param>
	private void AddWidget(string type, string zone)
	{
		WidgetBase widget = (WidgetBase)LoadControl(Utils.RelativeWebRoot + "widgets/" + type + "/widget.ascx");
		widget.WidgetID = Guid.NewGuid();
		widget.ID = widget.WidgetID.ToString().Replace("-", string.Empty);
		widget.Title = type;
        widget.Zone = zone;
		widget.ShowTitle = widget.DisplayHeader;
		widget.LoadWidget();

		Response.Clear();
		try
		{
			using (StringWriter sw = new StringWriter())
			{
				widget.RenderControl(new HtmlTextWriter(sw));

                // Using ? as a delimiter. ? is a safe delimiter because it cannot appear in a
                // zonename because ? is one of the characters removed by Utils.RemoveIllegalCharacters().
				Response.Write(zone + "?" + sw);
			}
		}
		catch (System.Web.HttpException)
		{
			Response.Write("reload");
		}

		SaveNewWidget(widget, zone);
		WidgetEditBase.OnSaved();
		Response.End();
	}

	/// <summary>
	/// Saves the new widget to the XML file.
	/// </summary>
	/// <param name="widget">The widget to add.</param>
    /// <param name="zone">The zone a widget is being added to.</param>
	private void SaveNewWidget(WidgetBase widget, string zone)
	{
		XmlDocument doc = GetXmlDocument(zone);
		XmlNode node = doc.CreateElement("widget");
		node.InnerText = widget.Name;

		XmlAttribute id = doc.CreateAttribute("id");
		id.InnerText = widget.WidgetID.ToString();
		node.Attributes.Append(id);

		XmlAttribute title = doc.CreateAttribute("title");
		title.InnerText = widget.Title;
		node.Attributes.Append(title);

		XmlAttribute show = doc.CreateAttribute("showTitle");
		show.InnerText = "True";
		node.Attributes.Append(show);

		doc.SelectSingleNode("widgets").AppendChild(node);
		SaveXmlDocument(doc, zone);
	}

	/// <summary>
	/// Moves the widgets as specified while dragging and dropping.
	/// </summary>
    /// <param name="moveData">Data containing which widget is moving, where it's moving from and where it's moving to.</param>
	private void MoveWidgets(string moveData)
	{
        string responseData = string.Empty;
        string[] data = moveData.Split(',');

        if (data.Length == 4)
        {
            string oldZone = data[0];
            string widgetToMoveId = data[1];
            string newZone = data[2];
            string moveBeforeWidgetId = data[3];

            // Ensure widgetToMoveId and moveBeforeWidgetId are not the same.
            if (!widgetToMoveId.Equals(moveBeforeWidgetId))
            {
                XmlDocument oldZoneDoc = GetXmlDocument(oldZone);
                XmlDocument newZoneDoc = GetXmlDocument(newZone);

                // If a widget is moving within its own widget, oldZoneDoc and newZoneDoc will
                // be referencing the same XmlDocument.  This is okay.
                if (oldZoneDoc != null && newZoneDoc != null)
                {
                    // Make sure we can find all required elements before moving anything.
                    XmlNode widgetToMove = oldZoneDoc.SelectSingleNode("//widget[@id=\"" + widgetToMoveId + "\"]");

                    // If a Zone was selected from the dropdown box (rather than a Widget), moveBeforeWidgetId
                    // will be null.  In this case, the widget is moved to the bottom of the new zone.

                    XmlNode moveBeforeWidget = null;
                    if (!string.IsNullOrEmpty(moveBeforeWidgetId))
                        moveBeforeWidget = newZoneDoc.SelectSingleNode("//widget[@id=\"" + moveBeforeWidgetId + "\"]");

                    if (widgetToMove != null)
                    {
                        // If the XmlNode is moving into a different XmlDocument, need to ImportNode() to
                        // create a copy of the XmlNode that is compatible with the new XmlDocument.

                        XmlNode widgetToMoveIntoNewDoc = newZoneDoc.ImportNode(widgetToMove, true);

                        widgetToMove.ParentNode.RemoveChild(widgetToMove);

                        if (moveBeforeWidget == null)
                            newZoneDoc.SelectSingleNode("widgets").AppendChild(widgetToMoveIntoNewDoc);
                        else
                            moveBeforeWidget.ParentNode.InsertBefore(widgetToMoveIntoNewDoc, moveBeforeWidget);

                        SaveXmlDocument(oldZoneDoc, oldZone);

                        if (!oldZone.Equals(newZone))
                            SaveXmlDocument(newZoneDoc, newZone);

                        WidgetEditBase.OnSaved();

                        // Pass back the same data that was sent in to indicate success.
                        responseData = moveData;
                    }
                }
            }
        }

        Response.Clear();
        Response.Write(responseData);
        Response.End();
	}

	#region Helper methods

	private static readonly string FILE_NAME = HostingEnvironment.MapPath(BlogSettings.Instance.StorageLocation + "widgetzone.xml");

	/// <summary>
	/// Gets the XML document.
	/// </summary>
    /// <param name="zone">The zone Xml Document to get.</param>
	/// <returns></returns>
	private XmlDocument GetXmlDocument(string zone)
	{
		XmlDocument doc;
        if (Cache[zone] == null)
		{
			WidgetSettings ws = new WidgetSettings(zone);
			ws.SettingsBehavior = new XMLDocumentBehavior();
			doc = (XmlDocument)ws.GetSettings();
			if (doc.SelectSingleNode("widgets") == null)
			{
				XmlNode widgets = doc.CreateElement("widgets");
				doc.AppendChild(widgets);
			}
            Cache[zone] = doc;
		}
		return (XmlDocument)Cache[zone];
	}

	/// <summary>
	/// Saves the XML document.
	/// </summary>
	/// <param name="doc">The doc.</param>
    /// <param name="zone">The zone to save the Xml Document for.</param>
	private void SaveXmlDocument(XmlDocument doc, string zone)
	{
		WidgetSettings ws = new WidgetSettings(zone);
		ws.SettingsBehavior = new XMLDocumentBehavior();
		ws.SaveSettings(doc);
        Cache[zone] = doc;
	}

	/// <summary>
	/// Inititiates the editor for widget editing.
	/// </summary>
	/// <param name="type">The type of widget to edit.</param>
	/// <param name="id">The id of the particular widget to edit.</param>
    /// <param name="zone">The zone the widget to be edited is in.</param>
	private void InitEditor(string type, string id, string zone)
	{
		XmlDocument doc = GetXmlDocument(zone);
		XmlNode node = doc.SelectSingleNode("//widget[@id=\"" + id + "\"]");
		string fileName = Utils.RelativeWebRoot + "widgets/" + type + "/edit.ascx";

		if (File.Exists(Server.MapPath(fileName)))
		{
			WidgetEditBase edit = (WidgetEditBase)LoadControl(fileName);
			edit.WidgetID = new Guid(node.Attributes["id"].InnerText);
			edit.Title = node.Attributes["title"].InnerText;
			edit.ID = "widget";
			edit.ShowTitle = bool.Parse(node.Attributes["showTitle"].InnerText);
			phEdit.Controls.Add(edit);
		}

		if (!Page.IsPostBack)
		{
			cbShowTitle.Checked = bool.Parse(node.Attributes["showTitle"].InnerText);
			txtTitle.Text = node.Attributes["title"].InnerText;
			txtTitle.Focus();
			btnSave.Text = Resources.labels.save;
		}

		btnSave.Click += new EventHandler(btnSave_Click);
	}

	#endregion

}
