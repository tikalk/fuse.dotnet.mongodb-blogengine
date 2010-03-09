#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;

#endregion

public partial class User_controls_CommentView : UserControl, ICallbackEventHandler
{

    #region ICallbackEventHandler Members

    private string _Callback;

    /// <summary>
    /// Returns the results of a callback event that targets a control.
    /// </summary>
    /// <returns>The result of the callback.</returns>
    public string GetCallbackResult()
    {
        return _Callback;
    }

    /// <summary>
    /// Processes a callback event that targets a control.
    /// </summary>
    /// <param name="eventArgument">A string that represents an event argument to pass to the event handler.</param>
    public void RaiseCallbackEvent(string eventArgument)
    {
        if (!BlogSettings.Instance.IsCommentsEnabled)
            return;

        string[] args = eventArgument.Split(new string[] { "-|-" }, StringSplitOptions.None);
        string author = args[0];
        string email = args[1];
        string website = args[2];
        string country = args[3];
        string content = args[4];
        bool notify = bool.Parse(args[5]);
        bool isPreview = bool.Parse(args[6]);
        string sentCaptcha = args[7];
        //If there is no "reply to" comment, args[8] is empty
        Guid replyToCommentID = String.IsNullOrEmpty(args[8]) ? Guid.Empty : new Guid(args[8]);
        string avatar = args[9];

        string storedCaptcha = hfCaptcha.Value;

        if (sentCaptcha != storedCaptcha)
            return;

        Comment comment = new Comment();
        comment.Id = Guid.NewGuid();
        comment.ParentId = replyToCommentID;
        comment.Author = Server.HtmlEncode(author);
        comment.Email = email;
        comment.Content = Server.HtmlEncode(content);
        comment.IP = Request.UserHostAddress;
        comment.Country = country;
        comment.DateCreated = DateTime.Now;
        comment.Parent = Post;
        comment.IsApproved = !BlogSettings.Instance.EnableCommentsModeration;
        comment.Avatar = avatar.Trim();

        if (Page.User.Identity.IsAuthenticated)
            comment.IsApproved = true;

        if (website.Trim().Length > 0)
        {
            if (!website.ToLowerInvariant().Contains("://"))
                website = "http://" + website;

            Uri url;
            if (Uri.TryCreate(website, UriKind.Absolute, out url))
                comment.Website = url;
        }

        if (!isPreview)
        {
            if (notify && !Post.NotificationEmails.Contains(email))
                Post.NotificationEmails.Add(email);
            else if (!notify && Post.NotificationEmails.Contains(email))
                Post.NotificationEmails.Remove(email);

            Post.AddComment(comment);
            SetCookie(author, email, website, country);
        }

        string path = Utils.RelativeWebRoot + "themes/" + BlogSettings.Instance.Theme + "/CommentView.ascx";

        CommentViewBase control = (CommentViewBase)LoadControl(path);
        control.Comment = comment;
        control.Post = Post;

        using (StringWriter sw = new StringWriter())
        {
            control.RenderControl(new HtmlTextWriter(sw));
            _Callback = sw.ToString();
        }
    }

    #endregion

    protected string NameInputId = string.Empty;
    protected string DefaultName = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        NameInputId = "txtName" + DateTime.Now.Ticks.ToString();

        if (Post == null)
            Response.Redirect(Utils.RelativeWebRoot);

        if (!Page.IsPostBack && !Page.IsCallback)
        {
            if (Page.User.Identity.IsAuthenticated)
            {
                if (Request.QueryString["deletecomment"] != null)
                    DeleteComment();

                if (Request.QueryString["deletecommentandchildren"] != null)
                    DeleteCommentAndChildren();

                if (!string.IsNullOrEmpty(Request.QueryString["approvecomment"]))
                    ApproveComment();

                if (!string.IsNullOrEmpty(Request.QueryString["approveallcomments"]))
                    ApproveAllComments();
            }

            string path = Utils.RelativeWebRoot + "themes/" + BlogSettings.Instance.Theme + "/CommentView.ascx";

            if (NestingSupported)
            {
                // newer, nested comments
                AddNestedComments(path, Post.NestedComments, phComments);
            }
            else
            {
                // old, non nested code

                //Add approved Comments
                foreach (Comment comment in Post.Comments)
                {
                    if (comment.IsApproved || !BlogSettings.Instance.EnableCommentsModeration)
                    {
                        CommentViewBase control = (CommentViewBase)LoadControl(path);
                        control.Comment = comment;
                        control.Post = Post;
                        phComments.Controls.Add(control);
                    }
                }

                //Add unapproved comments
                if (Page.User.Identity.IsAuthenticated)
                {
                    foreach (Comment comment in Post.Comments)
                    {
                        if (!comment.IsApproved)
                        {
                            CommentViewBase control = (CommentViewBase)LoadControl(path);
                            control.Comment = comment;
                            control.Post = Post;
                            phComments.Controls.Add(control);
                        }
                    }
                }
            }


            if (BlogSettings.Instance.IsCommentsEnabled)
            {

                if (!Post.IsCommentsEnabled || (BlogSettings.Instance.DaysCommentsAreEnabled > 0 &&
                   Post.DateCreated.AddDays(BlogSettings.Instance.DaysCommentsAreEnabled) < DateTime.Now.Date))
                {
                    phAddComment.Visible = false;
                    lbCommentsDisabled.Visible = true;
                }

                BindCountries();
                GetCookie();
                hfCaptcha.Value = Guid.NewGuid().ToString();
            }
            else
            {
                phAddComment.Visible = false;
            }
            //InititializeCaptcha();
        }


        Page.ClientScript.GetCallbackEventReference(this, "arg", null, string.Empty);
    }

    private void AddNestedComments(string path, List<Comment> nestedComments, PlaceHolder phComments)
    {
        foreach (Comment comment in nestedComments)
        {
            CommentViewBase control = (CommentViewBase)LoadControl(path);
            if (comment.IsApproved || !BlogSettings.Instance.EnableCommentsModeration || (!comment.IsApproved && Page.User.Identity.IsAuthenticated))
            {
                control.Comment = comment;
                control.Post = Post;

                if (comment.Comments.Count > 0)
                {
                    // find the next placeholder and add the subcomments to it
                    PlaceHolder phSubComments = control.FindControl("phSubComments") as PlaceHolder;
                    if (phSubComments != null)
                    {
                        AddNestedComments(path, comment.Comments, phSubComments);
                    }
                }

                phComments.Controls.Add(control);
            }
        }
    }

    private void ApproveComment()
    {
        foreach (Comment comment in Post.NotApprovedComments)
        {
            if (comment.Id == new Guid(Request.QueryString["approvecomment"]))
            {
                Post.ApproveComment(comment);

                int index = Request.RawUrl.IndexOf("?");
                string url = Request.RawUrl.Substring(0, index);
                Response.Redirect(url, true);
            }
        }
    }

    private void ApproveAllComments()
    {

        Post.ApproveAllComments();

        int index = Request.RawUrl.IndexOf("?");
        string url = Request.RawUrl.Substring(0, index);
        Response.Redirect(url, true);
    }

    private void DeleteComment()
    {
        foreach (Comment comment in Post.Comments)
        {
            if (comment.Id == new Guid(Request.QueryString["deletecomment"]))
            {
                Post.RemoveComment(comment);

                int index = Request.RawUrl.IndexOf("?");
                string url = Request.RawUrl.Substring(0, index) + "#comment";
                Response.Redirect(url, true);
            }
        }
    }

    private void DeleteCommentAndChildren()
    {
        foreach (Comment comment in Post.Comments)
        {
            if (comment.Id == new Guid(Request.QueryString["deletecommentandchildren"]))
            {
                // collect comments to delete first so the Nesting isn't lost
                List<Comment> commentsToDelete = new List<Comment>();

                CollectCommentToDelete(comment, commentsToDelete);

                foreach (Comment commentToDelete in commentsToDelete)
                    Post.RemoveComment(commentToDelete);

                int index = Request.RawUrl.IndexOf("?");
                string url = Request.RawUrl.Substring(0, index) + "#comment";
                Response.Redirect(url, true);
            }
        }
    }


    private void CollectCommentToDelete(Comment comment, List<Comment> commentsToDelete)
    {
        commentsToDelete.Add(comment);
        // recursive collection
        foreach (Comment subComment in comment.Comments)
            CollectCommentToDelete(subComment, commentsToDelete);
    }

    /// <summary>
    /// Binds the country dropdown list with countries retrieved
    /// from the .NET Framework.
    /// </summary>
    public void BindCountries()
    {
        StringDictionary dic = new StringDictionary();
        List<string> col = new List<string>();

        foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            RegionInfo ri = new RegionInfo(ci.Name);
            if (!dic.ContainsKey(ri.EnglishName))
                dic.Add(ri.EnglishName, ri.TwoLetterISORegionName.ToLowerInvariant());

            if (!col.Contains(ri.EnglishName))
                col.Add(ri.EnglishName);
        }

        // Add custom cultures
        if (!dic.ContainsValue("bd"))
        {
            dic.Add("Bangladesh", "bd");
            col.Add("Bangladesh");
        }

        col.Sort();

        ddlCountry.Items.Add(new ListItem("[Not specified]", ""));
        foreach (string key in col)
        {
            ddlCountry.Items.Add(new ListItem(key, dic[key]));
        }

        if (ddlCountry.SelectedIndex == 0)
        {
            ddlCountry.SelectedValue = ResolveRegion().TwoLetterISORegionName.ToLowerInvariant();
            this.SetFlagImageUrl();
        }
    }

    /// <summary>
    /// Resolves the region based on the browser language.
    /// </summary>
    public static RegionInfo ResolveRegion()
    {
        string[] languages = HttpContext.Current.Request.UserLanguages;

        if (languages == null || languages.Length == 0)
            return new RegionInfo(CultureInfo.CurrentCulture.LCID);

        try
        {
            string language = languages[0].ToLowerInvariant().Trim();
            CultureInfo culture = CultureInfo.CreateSpecificCulture(language);
            return new RegionInfo(culture.LCID);
        }
        catch (ArgumentException)
        {
            try
            {
                return new RegionInfo(CultureInfo.CurrentCulture.LCID);
            }
            catch (ArgumentException)
            {
                // the googlebot sometimes gives a culture LCID of 127 which is invalid
                // so assume US english if invalid LCID
                return new RegionInfo(1033);
            }
        }
    }

    private void SetFlagImageUrl()
    {
        if (!string.IsNullOrEmpty(ddlCountry.SelectedValue))
        {
            imgFlag.ImageUrl = Utils.RelativeWebRoot + "pics/flags/" + ddlCountry.SelectedValue + ".png";
        }
        else
        {
            imgFlag.ImageUrl = Utils.RelativeWebRoot + "pics/pixel.png";
        }
    }

    #region Cookies

    /// <summary>
    /// Sets a cookie with the entered visitor information
    /// so it can be prefilled on next visit.
    /// </summary>
    private void SetCookie(string name, string email, string website, string country)
    {
        HttpCookie cookie = new HttpCookie("comment");
        cookie.Expires = DateTime.Now.AddMonths(24);
        cookie.Values.Add("name", Server.UrlEncode(name.Trim()));
        cookie.Values.Add("email", email.Trim());
        cookie.Values.Add("url", website.Trim());
        cookie.Values.Add("country", country);
        Response.Cookies.Add(cookie);
    }

    /// <summary>
    /// Gets the cookie with visitor information if any is set.
    /// Then fills the contact information fields in the form.
    /// </summary>
    private void GetCookie()
    {
        HttpCookie cookie = Request.Cookies["comment"];
        try
        {
            if (cookie != null)
            {
                DefaultName = Server.UrlDecode(cookie.Values["name"]);
                txtEmail.Text = cookie.Values["email"];
                txtWebsite.Text = cookie.Values["url"];
                ddlCountry.SelectedValue = cookie.Values["country"];
                this.SetFlagImageUrl();
            }
            else if (Page.User.Identity.IsAuthenticated)
            {
                MembershipUser user = Membership.GetUser();
                DefaultName = user.UserName;
                txtEmail.Text = user.Email;
                txtWebsite.Text = Request.Url.Host;
            }
        }
        catch (Exception)
        {
            // Couldn't retrieve info on the visitor/user
        }
    }

    #endregion

    #region Protected methods and properties

    private Post _Post;

    /// <summary>
    /// Gets or sets the post from which the comments are parsed.
    /// </summary>
    public Post Post
    {
        get { return _Post; }
        set { _Post = value; }
    }

    /// <summary>
    /// Displays a delete link to visitors that is authenticated
    /// using the default membership provider.
    /// </summary>
    /// <param name="id">The id of the comment.</param>
    protected string AdminLink(string id)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Comment comment in Post.Comments)
            {
                if (comment.Id.ToString() == id)
                    sb.AppendFormat(" | <a href=\"mailto:{0}\">{0}</a>", comment.Email);
            }

            string confirmDelete = "Are you sure you want to delete the comment?";
            sb.AppendFormat(" | <a href=\"?deletecomment={0}\" onclick=\"return confirm('{1}?')\">{2}</a>",
                            id.ToString(), confirmDelete, "Delete");
            return sb.ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// Displays the Gravatar image that matches the specified email.
    /// </summary>
    protected string Gravatar(string email, string name, int size)
    {
        if (email.Contains("://"))
            return
                string.Format(
                    "<img class=\"thumb\" src=\"http://images.websnapr.com/?url={0}&amp;size=t\" alt=\"{1}\" />", name,
                    email);
        //http://www.artviper.net/screenshots/screener.php?&url={0}&h={1}&w={1}
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(Encoding.ASCII.GetBytes(email));

        StringBuilder hash = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
            hash.Append(result[i].ToString("x2"));

        StringBuilder image = new StringBuilder();
        image.Append("<img src=\"");
        image.Append("http://www.gravatar.com/avatar.php?");
        image.Append("gravatar_id=" + hash.ToString());
        image.Append("&amp;rating=G");
        image.Append("&amp;size=" + size);
        image.Append("&amp;default=");
        image.Append(Server.UrlEncode(Utils.AbsoluteWebRoot + "themes/" + BlogSettings.Instance.Theme + "/noavatar.jpg"));
        image.Append("\" alt=\"\" />");
        return image.ToString();
    }

    /// <summary>
    /// Displays BBCodes dynamically loaded from settings.
    /// </summary>
    protected string BBCodes()
    {
        try
        {
            string retVal = string.Empty;
            string title = string.Empty;
            string code = string.Empty;

            ExtensionSettings settings = ExtensionManager.GetSettings("BBCode");
            DataTable table = settings.GetDataTable();

            foreach (DataRow row in table.Rows)
            {
                code = (string)row["Code"];
                title = "[" + code + "][/" + code + "]";
                retVal += "<a title=\"" + title + "\" href=\"javascript:void(BlogEngine.addBbCode('" + code + "'))\">" + code + "</a>";
            }
            return retVal;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private bool? _nestingSupported = null;

    public bool NestingSupported
    {
        get
        {
            if (!_nestingSupported.HasValue)
            {
                if (!BlogSettings.Instance.IsCommentNestingEnabled)
                    _nestingSupported = false;
                else
                {
                    string path = Utils.RelativeWebRoot + "themes/" + BlogSettings.Instance.Theme + "/CommentView.ascx";

                    // test comment control for nesting placeholder (for backwards compatibility with older themes)
                    CommentViewBase commentTester = (CommentViewBase)LoadControl(path);
                    PlaceHolder phSubComments = commentTester.FindControl("phSubComments") as PlaceHolder;
                    _nestingSupported = phSubComments != null;
                }                
            }

            return _nestingSupported.Value;
        }
    }

    /// <summary>
    /// If true, on comment save will show "awaiting moderation" message
    /// otherwise "comment saved, thank you.." will be displayd.
    /// </summary>
    public string ManualModeration
    {
        get
        {
            if (BlogSettings.Instance.EnableCommentsModeration && BlogSettings.Instance.ModerationType == 0)
                return "true";
            return "false";
        }
    }

    #endregion
}
