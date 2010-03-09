using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core;
using System.Collections.Generic;

public partial class error_occurred : BlogBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = "Error";
        OutputErrorDetails();
    }

    private void OutputErrorDetails()
    {
        string contextItemKey = "LastErrorDetails";

        // Only display error details if the person is logged in.
        if (Page.User.Identity.IsAuthenticated && HttpContext.Current.Items.Contains(contextItemKey))
        { 
            string errorDetails = (string)HttpContext.Current.Items[contextItemKey];

            if (!string.IsNullOrEmpty(errorDetails))
            {
                divErrorDetails.Visible = true;                
                pDetails.InnerHtml = Server.HtmlEncode(errorDetails);
                pDetails.InnerHtml = errorDetails.Replace(Environment.NewLine, "<br /><br />");
            }
        }        
    }
}
