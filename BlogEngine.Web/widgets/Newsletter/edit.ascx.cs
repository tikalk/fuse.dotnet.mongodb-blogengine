using System;
using System.Web;
using System.IO;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;

public partial class widgets_Newsletter_edit : WidgetEditBase
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			_Doc = null;
			BindGrid();
		}

		gridEmails.RowDeleting += new GridViewDeleteEventHandler(gridEmails_RowDeleting);
		gridEmails.RowDataBound += new GridViewRowEventHandler(gridEmails_RowDataBound);
}

	private void BindGrid()
	{
		LoadEmails();
	
		gridEmails.DataKeyNames = new string[] { "innertext" };
		gridEmails.DataSource = _Doc.FirstChild;
		gridEmails.DataBind();
	}

	void gridEmails_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow && !Page.IsPostBack)
		{
			LinkButton delete = e.Row.Cells[0].Controls[0] as LinkButton;
			if (delete != null)
			{
				delete.OnClientClick = "return confirm('Are you sure you want to delete this e-mail?')";
			}
		}
	}


	void gridEmails_RowDeleting(object sender, GridViewDeleteEventArgs e)
	{
		LoadEmails();
		string email = (string)gridEmails.DataKeys[e.RowIndex].Value;
		XmlNode node = _Doc.SelectSingleNode("emails/email[text()='" + email + "']");
		if (node != null)
		{
			_Doc.FirstChild.RemoveChild(node);
			BindGrid();
		}
	}

	private static XmlDocument _Doc;
	private static string _FileName;

	private static void LoadEmails()
	{
		if (_Doc == null || _FileName == null)
		{
			_FileName = Path.Combine(BlogSettings.Instance.StorageLocation, "newsletter.xml");
			_FileName = System.Web.Hosting.HostingEnvironment.MapPath(_FileName);

			if (File.Exists(_FileName))
			{
				_Doc = new XmlDocument();
				_Doc.Load(_FileName);
			}
			else
			{
				_Doc = new XmlDocument();
				_Doc.LoadXml("<emails></emails>");
			}
		}
	}

	private void SaveEmails()
	{
		using (MemoryStream ms = new MemoryStream())
		using (FileStream fs = File.Open(_FileName, FileMode.Create, FileAccess.Write))
		{
			_Doc.Save(ms);
			ms.WriteTo(fs);
		}
	}

	public override void Save()
	{
		SaveEmails();
	}
}
