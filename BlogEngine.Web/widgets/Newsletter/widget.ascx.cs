#region using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using BlogEngine.Core;
using System.Net.Mail;
using System.Collections.Generic;

#endregion

public partial class widgets_Newsletter_widget : WidgetBase, ICallbackEventHandler
{

    public override string Name
    {
        get { return "Newsletter"; }
    }

    public override bool IsEditable
    {
        get { return true; }
    }

    public override void LoadWidget()
    {

    }

    #region Event handling

    static widgets_Newsletter_widget()
    {
        Post.Saved += new EventHandler<SavedEventArgs>(Post_Saved);
        Post.Saving += new EventHandler<SavedEventArgs>(Post_Saving);
    }

    private static Dictionary<Guid, bool> GetSendNewslettersContextData()
    {
        string sendNewsletterEmailsContextItemKey = "SendNewsletterEmails";
        Dictionary<Guid, bool> data = null;

        if (HttpContext.Current.Items.Contains(sendNewsletterEmailsContextItemKey))
            data = HttpContext.Current.Items[sendNewsletterEmailsContextItemKey] as Dictionary<Guid, bool>;
        else
        {
            data = new Dictionary<Guid, bool>();
            HttpContext.Current.Items[sendNewsletterEmailsContextItemKey] = data;
        }

        return data;
    }

    private static bool GetSendSendNewsletterEmails(Guid postId)
    {
        Dictionary<Guid, bool> data = GetSendNewslettersContextData();

        if (data.ContainsKey(postId))
            return data[postId];

        return false;
    }

    private static void SetSendNewsletterEmails(Guid postId, bool send)
    {
        Dictionary<Guid, bool> data = GetSendNewslettersContextData();
        data[postId] = send;
    }

    static void Post_Saving(object sender, SavedEventArgs e)
    {
        // Set SendNewsletterEmails to true whenever a post is changing from an unpublished
        // state to a published state.  To check the published state of this Post before
        // it was changed, it's necessary to retrieve the post from the datastore since the
        // post in memory (via Post.GetPost()) will already have the updated values about
        // to be saved.

        Post post = (Post)sender;

        SetSendNewsletterEmails(post.Id, false);  // default to not sending

        if (e.Action == SaveAction.Insert && post.IsVisibleToPublic)
            SetSendNewsletterEmails(post.Id, true);
        else if (e.Action == SaveAction.Update && post.IsVisibleToPublic)
        {
            Post preUpdatePost = BlogEngine.Core.Providers.BlogService.SelectPost(post.Id);
            if (preUpdatePost != null && !preUpdatePost.IsVisibleToPublic)
                SetSendNewsletterEmails(post.Id, true);
        }
    }

    static void Post_Saved(object sender, SavedEventArgs e)
    {
        Post post = (Post)sender;

        if (GetSendSendNewsletterEmails(post.Id))
        {
            LoadEmails();
            foreach (XmlNode node in _Doc.SelectNodes("emails/email"))
            {
                MailMessage mail = CreateEmail(post);
                mail.To.Add(node.InnerText);
                Utils.SendMailMessageAsync(mail);
            }
        }
    }

    private static MailMessage CreateEmail(Post post)
    {
        MailMessage mail = new MailMessage();
        mail.Subject = post.Title;
        mail.Body = "There'a new post available.<br /> <a href=\"" + post.AbsoluteLink + "\">" + post.Title + "</a><br />";
        mail.Body += post.Description;
        mail.Body += "<hr />";
        mail.Body += "You receive this e-mail because you have signed up for e-mail notifications at ";
        mail.Body += "<a href=\"" + Utils.AbsoluteWebRoot + "\">" + Utils.AbsoluteWebRoot + "</a>";
        mail.From = new MailAddress(BlogSettings.Instance.Email, BlogSettings.Instance.Name);
        return mail;
    }

    #endregion

    #region Save and retrieve e-mails

    private static XmlDocument _Doc;
    private static string _FileName;

    private void AddEmail(string email)
    {
        try
        {
            LoadEmails();

            if (!DoesEmailExist(email))
            {
                XmlNode node = _Doc.CreateElement("email");
                node.InnerText = email;
                _Doc.FirstChild.AppendChild(node);

                _Callback = "true";
                SaveEmails();
            }
            else
            {
                _Doc.FirstChild.RemoveChild(_Doc.SelectSingleNode("emails/email[text()='" + email + "']"));
                _Callback = "false";
                SaveEmails();
            }
        }
        catch
        {
            _Callback = "false";
        }
    }

    private bool DoesEmailExist(string email)
    {
        return _Doc.SelectSingleNode("emails/email[text()='" + email + "']") != null;
    }

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

    #endregion

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
        _Callback = eventArgument;
        AddEmail(eventArgument);
    }

    #endregion
}
