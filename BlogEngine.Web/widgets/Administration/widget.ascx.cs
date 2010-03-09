using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class widgets_Administration_widget : WidgetBase
{

	public override string Name
	{
		get { return "Administration"; }
	}

	public override bool IsEditable
	{
		get { return false; }
	}

	public override void LoadWidget()
	{
		Visible = Page.User.Identity.IsAuthenticated;
	}
}
