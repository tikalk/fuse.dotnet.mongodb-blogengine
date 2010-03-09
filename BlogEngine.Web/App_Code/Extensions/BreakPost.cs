#region using

using System;
using System.Web;
using System.Web.UI;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core;
using System.Text.RegularExpressions;
using System.Collections.Generic;

#endregion

/// <summary>
/// Breaks a post where [more] is found in the body and adds a link to full post.
/// </summary>
[Extension("Breaks a post where [more] is found in the body and adds a link to full post", "1.4", "BlogEngine.NET", 1010)]
public class BreakPost
{

    /// <summary>
    /// Hooks up an event handler to the Post.Serving event.
    /// </summary>
    static BreakPost()
    {
        Post.Serving += new EventHandler<ServingEventArgs>(Post_Serving);
    }

    /// <summary>
    /// Handles the Post.Serving event to take care of the [more] keyword.
    /// </summary>
    private static void Post_Serving(object sender, ServingEventArgs e)
    {
        if (!e.Body.Contains("[more]"))
            return;

        if (e.Location == ServingLocation.PostList)
        {
            AddMoreLink(sender, e);
        }
        else if (e.Location == ServingLocation.SinglePost)
        {
            PrepareFullPost(e);
        }
        else if (e.Location == ServingLocation.Feed)
        {
            e.Body = e.Body.Replace("[more]", string.Empty);
        }
    }

    private static Regex openingTagRegex = new Regex(@"<([A-Z][A-Z0-9]*?)\b[^>/]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex closedTagRegex = new Regex(@"</([A-Z][A-Z0-9]*?)\b[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Replaces the [more] string with a hyperlink to the full post.
    /// </summary>
    private static void AddMoreLink(object sender, ServingEventArgs e)
    {
        Post post = (Post)sender;
        int index = e.Body.IndexOf("[more]");
        string link = "<a class=\"more\" href=\"" + post.RelativeLink + "#continue\">" + Resources.labels.more + "...</a>";
        string NewBody = e.Body.Substring(0, index);

        // Need to close any open HTML tags in NewBody where the matching close tags have been truncated.
        string closingTagsToAppend = string.Empty;
        MatchCollection openingTagsCollection = openingTagRegex.Matches(NewBody);

        if (openingTagsCollection.Count > 0)
        {
            // Copy the opening tags in MatchCollection to a generic list.
            List<string> openingTags = new List<string>();
            foreach (Match openTag in openingTagsCollection)
            {
                if (openTag.Groups.Count == 2)
                {
                    openingTags.Add(openTag.Groups[1].Value);
                }
            }
            MatchCollection closingTagsCollection = closedTagRegex.Matches(NewBody);
            // Iterate through closed tags and remove the first matching open tag from the openingTags list.
            foreach (Match closedTag in closingTagsCollection)
            {
                if (closedTag.Groups.Count == 2)
                {
                    int indexToRemove = openingTags.FindIndex(delegate(string openTag) { return openTag.Equals(closedTag.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase); });
                    if (indexToRemove != -1)
                        openingTags.RemoveAt(indexToRemove);
                }
            }
            // A closing tag needs to be created for any remaining tags in the openingTags list.
            if (openingTags.Count > 0)
            {
                // Reverse the order of the tags so tags opened later are closed first.
                openingTags.Reverse();
                closingTagsToAppend = "</" + string.Join("></", openingTags.ToArray()) + ">";
            }
        }
        e.Body = NewBody + link + closingTagsToAppend;
    }

    /// <summary>
    /// Replaces the [more] string on the full post page.
    /// </summary>
    private static void PrepareFullPost(ServingEventArgs e)
    {
        HttpRequest request = HttpContext.Current.Request;
        if (request.UrlReferrer == null || request.UrlReferrer.Host != request.Url.Host)
        {
            e.Body = e.Body.Replace("[more]", string.Empty);
        }
        else
        {
            e.Body = e.Body.Replace("[more]", "<span id=\"continue\"></span>");
        }
    }

}