using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class widgets_Month_List_widget : WidgetBase
{

	public override string Name
	{
		get {return "Month List"; }
	}

	public override bool IsEditable
	{
		get { return false; }
	}

	public override void LoadWidget()
	{
		// No action
	}
}
