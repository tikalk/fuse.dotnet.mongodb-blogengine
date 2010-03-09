﻿using System;
using System.Net;
using System.Web;
using System.IO;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using Joel.Net;

/// <summary>
/// Summary description for AkismetFilter
/// </summary>
[Extension("Akismet anti-spam comment filter", "1.0", "<a href=\"http://dotnetblogengine.net\">BlogEngine.NET</a>")]
public class AkismetFilter : ICustomFilter
{
    #region Private members

    private static ExtensionSettings _settings;
    private static Akismet _api;
    private static string _site;
    private static string _key;

    #endregion

    public AkismetFilter()
    {
        InitSettings();
    }

    #region ICustomFilter implementation

    public bool Initialize()
    {
        if (!ExtensionManager.ExtensionEnabled("AkismetFilter"))
            return false;

        if (_settings == null) InitSettings();

        _site = _settings.GetSingleValue("SiteURL");
        _key = _settings.GetSingleValue("ApiKey");
        _api = new Akismet(_key, _site, "BlogEngine.net 1.6");

        return _api.VerifyKey();
    }

    public bool Check(Comment comment)
    {
        if (_api == null) Initialize();

        if (_settings == null) InitSettings();

        AkismetComment akismetComment = GetAkismetComment(comment);
        return _api.CommentCheck(akismetComment);
    }

    public void Report(Comment comment)
    {
        if (_api == null) Initialize();

        if (_settings == null) InitSettings();

        AkismetComment akismetComment = GetAkismetComment(comment);

        if (comment.IsApproved)
        {
            Utils.Log(string.Format("Akismet: Reporting NOT spam from \"{0}\" at \"{1}\"", comment.Author, comment.IP));
            _api.SubmitHam(akismetComment);
        }
        else
        {
            Utils.Log(string.Format("Akismet: Reporting SPAM from \"{0}\" at \"{1}\"", comment.Author, comment.IP));
            _api.SubmitSpam(akismetComment);
        }
    }

    public bool FallThrough { get { return true; } }

    #endregion
    
    #region Private methods

    private AkismetComment GetAkismetComment(Comment comment)
    {
        AkismetComment akismetComment = new AkismetComment();
        akismetComment.Blog = _settings.GetSingleValue("SiteURL");
        akismetComment.UserIp = comment.IP;
        akismetComment.CommentContent = comment.Content;
        akismetComment.CommentType = "comment";
        akismetComment.CommentAuthor = comment.Author;
        akismetComment.CommentAuthorEmail = comment.Email;
        if (comment.Website != null)
        {
            akismetComment.CommentAuthorUrl = comment.Website.OriginalString;
        }
        return akismetComment;
    }

    private void InitSettings()
    {
        ExtensionSettings settings = new ExtensionSettings(this);
        settings.IsScalar = true;

        settings.AddParameter("SiteURL", "Site URL");
        settings.AddParameter("ApiKey", "API Key");

        settings.AddValue("SiteURL", "http://example.com/blog");
        settings.AddValue("ApiKey", "123456789");

        
        _settings = ExtensionManager.InitSettings("AkismetFilter", settings);
        ExtensionManager.SetStatus("AkismetFilter", false);
    }

    #endregion
}

/* Author:      Joel Thoms (http://joel.net)
 * Copyright:   2006 Joel Thoms (http://joel.net)
 * About:       Akismet (http://akismet.com) .Net 2.0 API allow you to check
 *              Akismet's spam database to verify your comments and prevent spam.
 * 
 * Note:        Do not launch 'DEBUG' code on your site.  Only build 'RELEASE' for your site.  Debug code contains
 *              Console.WriteLine's, which are not desireable on a website.
*/

namespace Joel.Net {

    #region - public class AkismetComment -
    public class AkismetComment {
        public string Blog = null;
        public string UserIp = null;
        public string UserAgent = null;
        public string Referrer = null;
        public string Permalink = null;
        public string CommentType = null;
        public string CommentAuthor = null;
        public string CommentAuthorEmail = null;
        public string CommentAuthorUrl = null;
        public string CommentContent = null;
    }
    #endregion

    public class Akismet {
        const string verifyUrl = "http://rest.akismet.com/1.1/verify-key";
        const string commentCheckUrl = "http://{0}.rest.akismet.com/1.1/comment-check";
        const string submitSpamUrl = "http://{0}.rest.akismet.com/1.1/submit-spam";
        const string submitHamUrl = "http://{0}.rest.akismet.com/1.1/submit-ham";

        string apiKey = null;
        string userAgent = "Joel.Net's Akismet API/1.0";
        string blog = null;

        public string CharSet = "UTF-8";

        /// <summary>Creates an Akismet API object.</summary>
        /// <param name="apiKey">Your wordpress.com API key.</param>
        /// <param name="blog">URL to your blog</param>
        /// <param name="userAgent">Name of application using API.  example: "Joel.Net's Akismet API/1.0"</param>
        /// <remarks>Accepts required fields 'apiKey', 'blog', 'userAgent'.</remarks>
        public Akismet(string apiKey, string blog, string userAgent) {
            this.apiKey = apiKey;
            if (userAgent != null) this.userAgent = userAgent + " | Akismet/1.11";
            this.blog = blog;
        }

        /// <summary>Verifies your wordpress.com key.</summary>
        /// <returns>'True' is key is valid.</returns>
        public bool VerifyKey() {
            bool value = false;

            string response = HttpPost(verifyUrl, String.Format("key={0}&blog={1}", apiKey, HttpUtility.UrlEncode(blog)), CharSet);
            value = (response == "valid") ? true : false;
            return value;
        }

        /// <summary>Checks AkismetComment object against Akismet database.</summary>
        /// <param name="comment">AkismetComment object to check.</param>
        /// <returns>'True' if spam, 'False' if not spam.</returns>
        public bool CommentCheck(AkismetComment comment) {
            bool value = false;

            value = Convert.ToBoolean(HttpPost(String.Format(commentCheckUrl, apiKey), CreateData(comment), CharSet));
            return value;
        }

        /// <summary>Submits AkismetComment object into Akismet database.</summary>
        /// <param name="comment">AkismetComment object to submit.</param>
        public void SubmitSpam(AkismetComment comment) {
            string value = HttpPost(String.Format(submitSpamUrl, apiKey), CreateData(comment), CharSet);
        }

        /// <summary>Retracts false positive from Akismet database.</summary>
        /// <param name="comment">AkismetComment object to retract.</param>
        public void SubmitHam(AkismetComment comment) {
            string value = HttpPost(String.Format(submitHamUrl, apiKey), CreateData(comment), CharSet);
        }



        #region - Private Methods (CreateData, HttpPost) -

        /// <summary>Takes an AkismetComment object and returns an (escaped) string of data to POST.</summary>
        /// <param name="comment">AkismetComment object to translate.</param>
        /// <returns>A System.String containing the data to POST to Akismet API.</returns>
        private string CreateData(AkismetComment comment) {
            string value = String.Format("blog={0}&user_ip={1}&user_agent={2}&referrer={3}&permalink={4}&comment_type={5}" +
                "&comment_author={6}&comment_author_email={7}&comment_author_url={8}&comment_content={9}",
                HttpUtility.UrlEncode(comment.Blog),
                HttpUtility.UrlEncode(comment.UserIp),
                HttpUtility.UrlEncode(comment.UserAgent),
                HttpUtility.UrlEncode(comment.Referrer),
                HttpUtility.UrlEncode(comment.Permalink),
                HttpUtility.UrlEncode(comment.CommentType),
                HttpUtility.UrlEncode(comment.CommentAuthor),
                HttpUtility.UrlEncode(comment.CommentAuthorEmail),
                HttpUtility.UrlEncode(comment.CommentAuthorUrl),
                HttpUtility.UrlEncode(comment.CommentContent)
            );

            return value;
        }

        /// <summary>Sends HttpPost</summary>
        /// <param name="url">URL to Post data to.</param>
        /// <param name="data">Data to post. example: key=&ltwordpress-key&gt;&blog=http://joel.net</param>
        /// <param name="charSet">Character set of your blog. example: UTF-8</param>
        /// <returns>A System.String containing the Http Response.</returns>
        private string HttpPost(string url, string data, string charSet) {
            string value = "";

            // Initialize Connection
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=" + charSet;
            request.UserAgent = userAgent;
            request.ContentLength = data.Length;

            // Write Data
            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            writer.Write(data);
            writer.Close();

            // Read Response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            value = reader.ReadToEnd();
            reader.Close();

            return value;
        }
        #endregion
    }
}