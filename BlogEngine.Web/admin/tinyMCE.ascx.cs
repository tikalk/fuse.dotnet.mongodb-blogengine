using System;
using System.Web;
using System.Web.UI.WebControls;

public partial class admin_tinyMCE : System.Web.UI.UserControl
{

  public string Text
  {
    get { return txtContent.Text; }
    set { txtContent.Text = value; }
  }

  public short TabIndex
  {
    get { return txtContent.TabIndex; }
    set { txtContent.TabIndex = value; }
  }

	public Unit Width
	{
		get { return txtContent.Width; }
		set { txtContent.Width = value; }
	}

	public Unit Height
	{
		get { return txtContent.Height; }
		set { txtContent.Height = value; }
	}
}
