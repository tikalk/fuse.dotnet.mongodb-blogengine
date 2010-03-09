using System;
using System.Net;
using System.IO;
using BlogEngine.Core;

/// <summary>
/// StopForumSpam.com custom comment filter
/// </summary>
public class StopForumSpam : ICustomFilter
{
    private static bool _passThrough = true;

    /// <summary>
    /// Enables or disables filter
    /// </summary>
    /// <returns>True of false</returns>
    public bool Initialize()
    {
        // do not need any initialization
        // simply return true to enable filter
        return true;
    }

    /// <summary>
    /// Check if comment is spam
    /// </summary>
    /// <param name="comment">Comment</param>
    /// <returns>True if comment is spam</returns>
    public bool Check(Comment comment)
    {
        try
        {
            string url = string.Format("http://www.stopforumspam.com/api?ip={0}", comment.IP);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            string value = reader.ReadToEnd();
            reader.Close();

            bool spam = value.ToLowerInvariant().Contains("<appears>yes</appears>") ? true : false;

            // if comment IP appears in the stopforumspam list
            // it is for sure spam; no need to pass to others.
            _passThrough = (spam) ? false : true;

            return spam;
        }
        catch (Exception e)
        {
            Utils.Log(string.Format("Error checking stopforumspam.com: {0}", e.Message));
            return false;
        }
    }

    /// <summary>
    /// Report mistakes to service
    /// </summary>
    /// <param name="comment">Comment</param>
    public void Report(Comment comment)
    {
        // if we needed report mistakes back to
        // service, we would put code here
    }

    /// <summary>
    /// If true comment will be passed to other
    /// custom filters for validation
    /// </summary>
    public bool FallThrough { get { return _passThrough; } }
}