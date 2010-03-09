using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class widgets_BlogRoll_widget : WidgetBase
{

	public override string Name
	{
		get { return "Blogroll"; }
	}

	public override bool IsEditable
	{
		get { return false; }
	}

	public override void LoadWidget()
	{
		// Nothing to load
	}
}
