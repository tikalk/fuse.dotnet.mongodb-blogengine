using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class widgets_PageList_widget : WidgetBase
{

	public override string Name
	{
		get { return "Page List"; }
	}

	public override bool IsEditable
	{
		get { return true; }
	}

	public override void LoadWidget()
	{
		// Nothing to load
	}
}
