#region Using

using System;
using System.Xml;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Globalization;
using BlogEngine.Core;
using System.IO;
using System.Web.Hosting;
using BlogEngine.Core.DataStore;
using System.Text.RegularExpressions;

#endregion

public partial class widgets_Twitter_widget : WidgetBase
{

    public override string Name
    {
        get { return "Twitter"; }
    }

    public override bool IsEditable
    {
        get { return true; }
    }

    private const string TWITTER_SETTINGS_CACHE_KEY = "twitter-settings";  // same key used in edit.ascx.cs.
    private const string TWITTER_FEEDS_CACHE_KEY = "twits";

    public override void LoadWidget()
    {
        TwitterSettings settings = GetTwitterSettings();

        if (settings.AccountUrl != null && !string.IsNullOrEmpty(settings.FollowMeText))
        {
            hlTwitterAccount.NavigateUrl = settings.AccountUrl.ToString();
            hlTwitterAccount.Text = settings.FollowMeText;
        }

        if (settings.FeedUrl != null)
        {
            if (HttpRuntime.Cache[TWITTER_FEEDS_CACHE_KEY] == null)
            { 
                XmlDocument doc = GetLastFeed();
                if (doc != null)
                {
                    HttpRuntime.Cache[TWITTER_FEEDS_CACHE_KEY] = doc.OuterXml;
                }
            }

            if (HttpRuntime.Cache[TWITTER_FEEDS_CACHE_KEY] != null)
            {
                string xml = (string)HttpRuntime.Cache[TWITTER_FEEDS_CACHE_KEY];
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                BindFeed(doc, settings.MaxItems);
            }

            if (DateTime.Now > settings.LastModified.AddMinutes(settings.PollingInterval))
            {
                settings.LastModified = DateTime.Now;
                BeginGetFeed(settings.FeedUrl);
            }
        }
    }

    private TwitterSettings GetTwitterSettings()
    {
        TwitterSettings twitterSettings = HttpRuntime.Cache[TWITTER_SETTINGS_CACHE_KEY] as TwitterSettings;

        if (twitterSettings != null)
            return twitterSettings;

        twitterSettings = new TwitterSettings();

        // Defaults
        int maxItems = 3;
        int pollingInterval = 15;
        string followMeText = "Follow me";
        Uri accountUrl;
        Uri feedUrl;

        StringDictionary settings = GetSettings();

        if (settings.ContainsKey("accounturl") && !string.IsNullOrEmpty(settings["accounturl"]))
        {
            Uri.TryCreate(settings["accounturl"], UriKind.Absolute, out accountUrl);
            twitterSettings.AccountUrl = accountUrl;
        }

        if (settings.ContainsKey("feedurl") && !string.IsNullOrEmpty(settings["feedurl"]))
        {
            Uri.TryCreate(settings["feedurl"], UriKind.Absolute, out feedUrl);
            twitterSettings.FeedUrl = feedUrl;
        }

        if (settings.ContainsKey("pollinginterval") && !string.IsNullOrEmpty(settings["pollinginterval"]))
        {
            int tempPollingInterval;
            if (int.TryParse(settings["pollinginterval"], out tempPollingInterval))
            {
                if (tempPollingInterval > 0)
                    pollingInterval = tempPollingInterval;
            }
        }
        twitterSettings.PollingInterval = pollingInterval;

        if (settings.ContainsKey("maxitems") && !string.IsNullOrEmpty(settings["maxitems"]))
        {
            int tempMaxItems;
            if (int.TryParse(settings["maxitems"], out tempMaxItems))
            {
                if (tempMaxItems > 0)
                    maxItems = tempMaxItems;
            }
        }
        twitterSettings.MaxItems = maxItems;

        if (settings.ContainsKey("followmetext") && !string.IsNullOrEmpty(settings["followmetext"]))
            twitterSettings.FollowMeText = settings["followmetext"];
        else
            twitterSettings.FollowMeText = followMeText;

        HttpRuntime.Cache[TWITTER_SETTINGS_CACHE_KEY] = twitterSettings;

        return twitterSettings;
    }

    protected void repItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Label text = (Label)e.Item.FindControl("lblItem");
        Label date = (Label)e.Item.FindControl("lblDate");
        Twit twit = (Twit)e.Item.DataItem;
        text.Text = twit.Title;
        date.Text = twit.PubDate.ToString("MMMM d. HH:mm");
    }

    private void BeginGetFeed(Uri url)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.BeginGetResponse(EndGetResponse, request);
        }
        catch (Exception ex)
        {
            string msg = "Error requesting Twitter feed.";
            if (ex != null) msg += " " + ex.Message;
            Utils.Log(msg);
        }
    }

    private void EndGetResponse(IAsyncResult result)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(response.GetResponseStream());
                HttpRuntime.Cache[TWITTER_FEEDS_CACHE_KEY] = doc.OuterXml;
                SaveLastFeed(doc);
            }
        }
        catch (Exception ex)
        {
            string msg = "Error retrieving Twitter feed.";
            if (ex != null) msg += " " + ex.Message;
            Utils.Log(msg);
        }
    }

    private static string _lastFeedDataFileName;

    private static string GetLastFeedDataFileName()
    { 
        if (string.IsNullOrEmpty(_lastFeedDataFileName))
        {
            _lastFeedDataFileName = HostingEnvironment.MapPath(Path.Combine(BlogSettings.Instance.StorageLocation, "twitter_feeds.xml"));
        }

        return _lastFeedDataFileName;
    }

    private static void SaveLastFeed(XmlDocument doc)
    {
        try
        {
            string file = GetLastFeedDataFileName();
            doc.Save(file);
        }
        catch (Exception ex)
        {
            Utils.Log("Error saving last twitter feed load to data store.  Error: " + ex.Message);
        }
    }

    private XmlDocument GetLastFeed()
    {
        string file = GetLastFeedDataFileName();
        XmlDocument doc = null;

        try
        {
            if (File.Exists(file))
            {
                doc = new XmlDocument();
                doc.Load(file);
            }
        }
        catch (Exception ex)
        {
            Utils.Log("Error retrieving last twitter feed load from data store.  Error: " + ex.Message);
        }

        return doc;
    }

    private void BindFeed(XmlDocument doc, int maxItems)
    {
        XmlNodeList items = doc.SelectNodes("//channel/item");
        List<Twit> twits = new List<Twit>();
        int count = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (count == maxItems)
                break;

            XmlNode node = items[i];
            Twit twit = new Twit();
            string title = node.SelectSingleNode("description").InnerText;

            if (title.Contains("@"))
                continue;

            if (title.Contains(":"))
            {
                int start = title.IndexOf(":") + 1;
                title = title.Substring(start);
            }

            twit.Title = ResolveLinks(title);
            twit.PubDate = DateTime.Parse(node.SelectSingleNode("pubDate").InnerText, CultureInfo.InvariantCulture);
            twit.Url = new Uri(node.SelectSingleNode("link").InnerText, UriKind.Absolute);
            twits.Add(twit);

            count++;
        }

        twits.Sort();
        repItems.DataSource = twits;
        repItems.DataBind();
    }

    private static readonly Regex regex = new Regex("((http://|https://|www\\.)([A-Z0-9.\\-]{1,})\\.[0-9A-Z?;~&\\(\\)#,=\\-_\\./\\+]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private const string link = "<a href=\"{0}{1}\" rel=\"nofollow\">{1}</a>";

    /// <summary>
    /// The event handler that is triggered every time a comment is served to a client.
    /// </summary>
    private string ResolveLinks(string body)
    {
        return regex.Replace(body, new MatchEvaluator(Evaluator));
    }

    /// <summary>
    /// Evaluates the replacement for each link match.
    /// </summary>
    public string Evaluator(Match match)
    {
        CultureInfo info = CultureInfo.InvariantCulture;
        if (!match.Value.Contains("://"))
        {
            return string.Format(info, link, "http://", match.Value);
        }
        else
        {
            return string.Format(info, link, string.Empty, match.Value);
        }
    }

    internal class TwitterSettings
    {
        public string FollowMeText;
        public int PollingInterval;
        public Uri FeedUrl;
        public Uri AccountUrl;
        public int MaxItems;
        public DateTime LastModified;
    }

    private struct Twit : IComparable<Twit>
    {
        public string Title;
        public Uri Url;
        public DateTime PubDate;

        public int CompareTo(Twit other)
        {
            return other.PubDate.CompareTo(this.PubDate);
        }
    }
}

