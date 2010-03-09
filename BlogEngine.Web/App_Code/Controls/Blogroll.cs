#region Using

using System;
using System.IO;
using System.Web;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Web.UI;
using System.Web.UI.HtmlControls;

#endregion

using BlogEngine.Core;

namespace Controls
{
  /// <summary>
  /// Creates and displays a dynamic blogroll.
  /// </summary>
    public class Blogroll : Control
    {
        public Blogroll()
        {
            BlogRollItem.Saved += new EventHandler<SavedEventArgs>(BlogRollItem_Saved);
        }

        private void BlogRollItem_Saved(object sender, SavedEventArgs e)
        {
            if (e.Action == SaveAction.Insert || e.Action == SaveAction.Update || e.Action == SaveAction.Delete)
            {
                if (e.Action == SaveAction.Insert)
                {
                    AddBlog((BlogRollItem)sender);
                }
                else if (e.Action == SaveAction.Delete)
                {
                    blogRequest affected = null;
                    foreach (blogRequest req in _Items)
                    {
                        if (req.RollItem.Equals(sender))
                        {
                            affected = req;
                            break;
                        }
                    }
                    _Items.Remove(affected);
                }
                
                if (_Items.Count > 0)
                {
                    // Re-sort _Items collection in case sorting of blogroll items was changed.
                    _Items.Sort(delegate(blogRequest br1, blogRequest br2) { return br1.RollItem.CompareTo(br2.RollItem); });
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!Page.IsPostBack && !Page.IsCallback)
            {
                HtmlGenericControl ul = DisplayBlogroll();
                StringWriter sw = new StringWriter();
                ul.RenderControl(new HtmlTextWriter(sw));
                string html = sw.ToString();

                writer.WriteLine("<div id=\"blogroll\">");
                writer.WriteLine(html);
                writer.WriteLine("</div>");
            }
        }

        #region Private fields

        private static DateTime _LastUpdated = DateTime.Now;
        private static List<blogRequest> _Items;

        #endregion

        #region Methods


        private static object _SyncRoot = new object();

        /// <summary>
        /// Displays the RSS item collection.
        /// </summary>
        private HtmlGenericControl DisplayBlogroll()
        {
            if (DateTime.Now > _LastUpdated.AddMinutes(BlogSettings.Instance.BlogrollUpdateMinutes) && BlogSettings.Instance.BlogrollVisiblePosts > 0)
            {
                _Items = null;
                _LastUpdated = DateTime.Now;
            }

            if (_Items == null)
            {
                lock (_SyncRoot)
                {
                    if (_Items == null)
                    {
                        _Items = new List<blogRequest>();
                        foreach (BlogEngine.Core.BlogRollItem roll in BlogEngine.Core.BlogRollItem.BlogRolls)
                        {
                            AddBlog(roll);
                        }
                    }
                }
            }

            return BindControls();
        }

        /// <summary>
        /// Parses the processed RSS items and returns the HTML
        /// </summary>
        private HtmlGenericControl BindControls()
        {
            HtmlGenericControl ul = new HtmlGenericControl("ul");
            ul.Attributes.Add("class", "xoxo");
            foreach (blogRequest item in _Items)
            {
                HtmlAnchor feedAnchor = new HtmlAnchor();
                feedAnchor.HRef = item.RollItem.FeedUrl.AbsoluteUri;

                HtmlImage image = new HtmlImage();
                image.Src = Utils.RelativeWebRoot + "pics/rssButton.gif";
                image.Alt = "RSS feed for " + item.RollItem.Title;

                feedAnchor.Controls.Add(image);

                HtmlAnchor webAnchor = new HtmlAnchor();
                webAnchor.HRef = item.RollItem.BlogUrl.AbsoluteUri;
                webAnchor.InnerHtml = EnsureLength(item.RollItem.Title);

                if (!String.IsNullOrEmpty(item.RollItem.Xfn))
                    webAnchor.Attributes["rel"] = item.RollItem.Xfn;

                HtmlGenericControl li = new HtmlGenericControl("li");
                li.Controls.Add(feedAnchor);
                li.Controls.Add(webAnchor);

                AddRssChildItems(item, li);
                ul.Controls.Add(li);
            }

            return ul;
        }

        private void AddRssChildItems(blogRequest item, HtmlGenericControl li)
        {
            if (item.ItemTitles.Count > 0 && BlogSettings.Instance.BlogrollVisiblePosts > 0)
            {
                HtmlGenericControl div = new HtmlGenericControl("ul");
                for (int i = 0; i < item.ItemTitles.Count; i++)
                {
                    if (i >= BlogSettings.Instance.BlogrollVisiblePosts) break;

                    HtmlGenericControl subLi = new HtmlGenericControl("li");
                    HtmlAnchor a = new HtmlAnchor();
                    a.HRef = item.ItemLinks[i];
                    a.Title = HttpUtility.HtmlEncode(item.ItemTitles[i]);
                    a.InnerHtml = EnsureLength(item.ItemTitles[i]);

                    subLi.Controls.Add(a);
                    div.Controls.Add(subLi);
                }

                li.Controls.Add(div);
            }
        }

        /// <summary>
        /// Ensures that the name is no longer than the MaxLength.
        /// </summary>
        private string EnsureLength(string textToShorten)
        {
            if (textToShorten.Length > BlogSettings.Instance.BlogrollMaxLength)
                return textToShorten.Substring(0, BlogSettings.Instance.BlogrollMaxLength).Trim() + "...";

            return HttpUtility.HtmlEncode(textToShorten);
        }

        /// <summary>
        /// Adds a blog to the item collection and start retrieving the blogs.
        /// </summary>
        private static void AddBlog(BlogEngine.Core.BlogRollItem br)
        {
            blogRequest affected = null;
            foreach (blogRequest req in _Items)
            {
                if (req.RollItem.Equals(br))
                {
                    affected = req;
                    break;
                }
            }
            if (affected == null)
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(br.FeedUrl);
                req.Credentials = CredentialCache.DefaultNetworkCredentials;

                blogRequest bReq = new blogRequest(br, req);
                _Items.Add(bReq);
                req.BeginGetResponse(ProcessRespose, bReq);
            }
        }

        /// <summary>
        /// Gets the request and processes the response.
        /// </summary>
        private static void ProcessRespose(IAsyncResult async)
        {
            blogRequest blogReq = (blogRequest)async.AsyncState;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)blogReq.Request.EndGetResponse(async))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(response.GetResponseStream());

                    XmlNodeList nodes = doc.SelectNodes("rss/channel/item");
                    foreach (XmlNode node in nodes)
                    {
                        string title = node.SelectSingleNode("title").InnerText;
                        string link = node.SelectSingleNode("link").InnerText;
                        DateTime date = DateTime.Now;
                        if (node.SelectSingleNode("pubDate") != null)
                            date = DateTime.Parse(node.SelectSingleNode("pubDate").InnerText);

                        blogReq.ItemTitles.Add(title);
                        blogReq.ItemLinks.Add(link);
                    }
                }
            }
            catch
            { }
        }

        #endregion

    }
    internal class blogRequest
    {
        internal BlogEngine.Core.BlogRollItem RollItem;
        internal HttpWebRequest Request;
        internal List<string> ItemTitles = new List<string>();
        internal List<string> ItemLinks = new List<string>();
        internal blogRequest(BlogEngine.Core.BlogRollItem rollItem, HttpWebRequest request)
        {
            this.RollItem = rollItem;
            this.Request = request;
        }
    }

}