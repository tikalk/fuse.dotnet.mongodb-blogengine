using System;
using System.Web;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Security;
using BlogEngine.Core;
using System.Web.UI.WebControls;
using Resources;

public partial class admin_Comments_DataGrid : System.Web.UI.UserControl
{
    public string AreYouSureDelete()
    {
        return string.Format(labels.areYouSure, labels.delete.ToLower(), labels.selectedComments);
    }

    #region Private members

    static protected List<Comment> Comments;
    private const string GravatarImage = "<img class=\"photo\" src=\"{0}\" alt=\"{1}\" />";
    private const string REMOVE_AUTHOR_FILTER = "<a href='?author=' alt='{0} {1}'>{0} {1}</a>";
    private const string REMOVE_IP_FILTER = "<a href='?ip=' alt='{0} {1}'>{0} {1}</a>";
    private bool _autoModerated;
    protected enum ActionType
    {
        Approve,Reject,Delete
    }
    static protected string _authorFilter = "";
    static protected string _ipFilter = "";

    #endregion

    #region Form events

    protected void Page_Load(object sender, EventArgs e)
    {
        gridComments.RowDataBound += gridComments_RowDataBound;
        gridComments.PageSize = (BlogSettings.Instance.CommentsPerPage > 0) ? BlogSettings.Instance.CommentsPerPage : 15;
        _autoModerated = BlogSettings.Instance.ModerationType == 1 ? true : false;

        string confirm = "return confirm('{0}');";
        string msg = "";

        if (!BlogSettings.Instance.EnableCommentsModeration || !BlogSettings.Instance.IsCommentsEnabled)
            btnAction.Visible = false;

        if (Request.Path.ToLower().Contains("approved.aspx"))
        {
            btnAction.Text = labels.reject;
            msg = string.Format(labels.areYouSure, labels.reject.ToLower(), labels.selectedComments);
            btnAction.OnClientClick = string.Format(confirm, msg);
        }
        else if (Request.Path.ToLower().Contains("spam.aspx"))
        {
            btnAction.Text = labels.restore;
            msg = string.Format(labels.areYouSure, labels.restore.ToLower(), labels.selectedComments);
            btnAction.OnClientClick = string.Format(confirm, msg);
        }
        else
        {
            if (_autoModerated)
            {
                btnAction.Text = labels.spam;
                msg = string.Format(labels.areYouSure, labels.spam.ToLower(), labels.selectedComments);
                btnAction.OnClientClick = string.Format(confirm, msg);
            }
            else
            {
                btnAction.Text = labels.approve;
                msg = string.Format(labels.areYouSure, labels.approve.ToLower(), labels.selectedComments);
                btnAction.OnClientClick = string.Format(confirm, msg);
            }
        }

        SetFilters();

        if (!Page.IsPostBack)
        {
            BindComments();
        }
    }

    protected void ddCommentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindComments();
    }

    protected void gridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        BindComments();
        gridComments.PageIndex = e.NewPageIndex;
        gridComments.DataBind();
    }

    protected void gridComments_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[8].Text = string.Format("{0} : {1} {2}", labels.total, Comments.Count, labels.comments);

            if(!string.IsNullOrEmpty(_authorFilter))
                e.Row.Cells[3].Text = string.Format(REMOVE_AUTHOR_FILTER, labels.remove, labels.filter);

            if(!string.IsNullOrEmpty(_ipFilter))
                e.Row.Cells[4].Text = string.Format(REMOVE_IP_FILTER, labels.remove, labels.filter);
        }
    }

    #endregion

    #region Binding

    protected void BindComments()
    {
        LoadData();
        gridComments.DataSource = Comments;
        gridComments.DataBind();
    }

    #endregion

    #region Data handling

    protected void LoadData()
    {
        List<Comment> comments = new List<Comment>();

        foreach (Post p in Post.Posts)
        {
            foreach (Comment c in p.Comments)
            {
                // do not include trackbacks and pingbacks to comment list
                if(c.Email == "trackback" || c.Email == "pingback") continue;

                if (!BlogSettings.Instance.EnableCommentsModeration || !BlogSettings.Instance.IsCommentsEnabled)
                {
                    if(Filtered(c)) comments.Add(c);
                }
                else
                {
                    if (Request.Path.ToLower().Contains("approved.aspx"))
                    {
                        if (c.IsApproved && Filtered(c)) comments.Add(c);
                    }
                    else if (Request.Path.ToLower().Contains("spam.aspx"))
                    {
                        if (!c.IsApproved && Filtered(c)) comments.Add(c);
                    }
                    else
                    {
                        // if auto-moderated, inbox has unapproved comments
                        if (_autoModerated && c.IsApproved && Filtered(c)) comments.Add(c);

                        // if manual moderation inbox has unapproved comments
                        if (!_autoModerated && !c.IsApproved && Filtered(c)) comments.Add(c);
                    }
                }
            }
        }
        // sort in descending order
        comments.Sort(delegate(Comment c1, Comment c2)
        { return DateTime.Compare(c2.DateCreated, c1.DateCreated); });
        Comments = comments;
    }

    protected void ApproveComment(Comment comment)
    {
        comment.IsApproved = true;
        ReportAndUpdate(comment);
    }

    protected void RejectComment(Comment comment)
    {
        comment.IsApproved = false;
        ReportAndUpdate(comment);
    }

    protected void RemoveComment(Comment comment)
    {
        bool found = false;
        for (int i = 0; i < Post.Posts.Count; i++)
        {
            for (int j = 0; j < Post.Posts[i].Comments.Count; j++)
            {
                if (Post.Posts[i].Comments[j].Id == comment.Id)
                {
                    Post.Posts[i].RemoveComment(Post.Posts[i].Comments[j]);
                    found = true;
                    break;
                }
            }
            if (found) { break; }
        }
    }

    protected void UpdateComment(Comment comment)
    {
        bool found = false;
        // Cast ToArray so the original collection isn't modified. 
        foreach (Post p in Post.Posts.ToArray())
        {
            // Cast ToArray so the original collection isn't modified. 
            foreach (Comment c in p.Comments.ToArray())
            {
                if (c.Id == comment.Id)
                {
                    c.Content = comment.Content;
                    c.IsApproved = comment.IsApproved;
                    c.ModeratedBy = comment.ModeratedBy;

                    // Need to mark post as "changed" for it to get saved into the data store. 
                    string desc = p.Description;
                    p.Description = (desc ?? string.Empty) + " ";
                    p.Description = desc;

                    p.Save();
                    found = true;
                    break;
                }
            }
            if (found) break;
        }
        BindComments();
    }

    #endregion

    #region Footer buttons

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        BindComments();
        foreach (GridViewRow row in gridComments.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkSelect");
            if (cb != null && cb.Enabled)
            {
                cb.Checked = true;
            }
        }
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        BindComments();
        foreach (GridViewRow row in gridComments.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkSelect");
            if (cb != null && cb.Enabled)
            {
                cb.Checked = false;
            }
        }
    }

    protected void btnAction_Click(object sender, EventArgs e)
    {
        if (Request.Path.ToLower().Contains("approved.aspx"))
        {
            ProcessSelected(ActionType.Reject);
        }
        else if (Request.Path.ToLower().Contains("spam.aspx"))
        {
            ProcessSelected(ActionType.Approve);
        }
        else // default.aspx
        {  
            if (_autoModerated)
                ProcessSelected(ActionType.Reject);
            else
                ProcessSelected(ActionType.Approve);
        }           
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        ProcessSelected(ActionType.Delete);
    }

    #endregion

    #region Private Methods

    protected void SetFilters()
    {
        string auth = Request.QueryString["author"];
        string ip = Request.QueryString["ip"];

        if (auth != null) _authorFilter = auth;
        if (ip != null) _ipFilter = ip;
    }

    protected bool Filtered(Comment c)
    {

        if (!string.IsNullOrEmpty(_authorFilter))
        {
            if (c.Author.ToLowerInvariant() != _authorFilter.ToLowerInvariant())
                return false;
        }

        if (!string.IsNullOrEmpty(_ipFilter))
        {
            if (c.IP.ToLowerInvariant() != _ipFilter.ToLowerInvariant())
                return false;
        }

        return true;
    }

    protected void ProcessSelected(ActionType action)
    {
        List<Comment> tmp = new List<Comment>();

        foreach (GridViewRow row in gridComments.Rows)
        {
            try
            {
                CheckBox cbx = (CheckBox)row.FindControl("chkSelect");
                if (cbx != null && cbx.Checked)
                {
                    Comment comment = Comments.Find(
                    delegate(Comment c)
                    {
                        return c.Id == (Guid)gridComments.DataKeys[row.RowIndex].Value;
                    });

                    if (comment != null) tmp.Add(comment);
                }
            }
            catch (Exception e)
            {
                Utils.Log(string.Format("Error processing selected row in comments data grid: {0}", e.Message));
            }
        }

        foreach (Comment cm in tmp)
        {
            if (action == ActionType.Approve)
            {
                if (!cm.IsApproved)
                    ApproveComment(cm);
            }
            else if (action == ActionType.Reject)
            {
                if (cm.IsApproved)
                    RejectComment(cm);
            }
            if (action == ActionType.Delete)
            {
                RemoveComment(cm);
            }
        }

        BindComments();
    }

    protected void ReportAndUpdate(Comment comment)
    {
        // moderator should match anti-spam service
        if (BlogSettings.Instance.ModerationType == 1)
            CommentHandlers.ReportMistake(comment);

        // now moderator can be set to logged in user
        comment.ModeratedBy = HttpContext.Current.User.Identity.Name;
        UpdateComment(comment);
    }

    public static bool HasNoChildren(Guid comId)
    {
        foreach (Post p in Post.Posts)
        {
            // Cast ToArray so the original collection isn't modified. 
            foreach (Comment c in p.Comments)
            {
                if (c.ParentId == comId)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static string GetEditHtml(string id)
    {
        return string.Format("editComment('{0}');return false;", id);
    }

    protected string Gravatar(string email, string author)
    {
        if (BlogSettings.Instance.Avatar == "none")
            return null;

        if (String.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            return "<img src=\"" + Utils.AbsoluteWebRoot + "themes/" + BlogSettings.Instance.Theme + "/noavatar.jpg\" alt=\"" + author + "\" width=\"28\" height=\"28\" />";
        }

        string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(email.ToLowerInvariant().Trim(), "MD5").ToLowerInvariant();
        string gravatar = "http://www.gravatar.com/avatar/" + hash + ".jpg?s=28&amp;d=";

        string link = string.Empty;
        switch (BlogSettings.Instance.Avatar)
        {
            case "identicon":
                link = gravatar + "identicon";
                break;

            case "wavatar":
                link = gravatar + "wavatar";
                break;

            default:
                link = gravatar + "monsterid";
                break;
        }

        return string.Format(CultureInfo.InvariantCulture, GravatarImage, link, author);
    }

    public static string GetWebsite(object website)
    {
        if (website == null) return "";

        const string templ = "<a href='{0}' target='_new' rel='{0}'>{1}</a>";

        string site = website.ToString();
        site = site.Replace("http://www.", "");
        site = site.Replace("http://", "");
        site = site.Length < 20 ? site : site.Substring(0, 17) + "...";

        return string.Format(templ, website, site);
    }
    
    #endregion
}