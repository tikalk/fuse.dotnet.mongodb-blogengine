#region using

using System;
using System.Data;
using System.Text.RegularExpressions;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;

#endregion

/// <summary>
/// Converts BBCode to XHTML in the comments.
/// </summary>
[Extension("Converts BBCode to XHTML in the comments", "1.0", "<a href=\"http://dotnetblogengine.net\">BlogEngine.NET</a>")]
public class BBCode
{
    static protected ExtensionSettings _settings = null;

    static BBCode()
    {
        Comment.Serving += new EventHandler<ServingEventArgs>(Post_CommentServing);

        // create settings object. You need to pass exactly your
        // extension class name (case sencitive)
        ExtensionSettings settings = new ExtensionSettings("BBCode");

        //-----------------------------------------------------
        // 1. Simple
        //-----------------------------------------------------
        //settings.AddParameter("Code");
        //settings.AddParameter("OpenTag");
        //settings.AddParameter("CloseTag");
        //-----------------------------------------------------
        // 2. Some more options
        //-----------------------------------------------------
        //settings.AddParameter("Code");
        //settings.AddParameter("OpenTag", "Open Tag");
        //settings.AddParameter("CloseTag", "Close Tag");

        //// describe specific rules applied to entering parameters. overrides default wording.
        //settings.Help = "Converts BBCode to XHTML in the comments. Close tag is optional.";
        //-----------------------------------------------------
        // 3. More options including import defaults
        //-----------------------------------------------------
        settings.AddParameter("Code", "Code", 20, true);
        settings.AddParameter("OpenTag", "Open Tag", 150, true);
        settings.AddParameter("CloseTag", "Close Tag");

        // describe specific rules for entering parameters
        settings.Help = "Converts BBCode to XHTML in the comments. Close tag is optional.";

        settings.AddValues(new string[] { "b", "strong", "" });
        settings.AddValues(new string[] { "i", "em", "" });
        settings.AddValues(new string[] { "u", "span style=\"text-decoration:underline\"", "span" });
        settings.AddValues(new string[] { "quote", "cite title=\"Quote\"", "cite" });
        //------------------------------------------------------
        ExtensionManager.ImportSettings(settings);
        _settings = ExtensionManager.GetSettings("BBCode");
    }

    /// <summary>
    /// The event handler that is triggered every time a comment is served to a client.
    /// </summary>
    private static void Post_CommentServing(object sender, ServingEventArgs e)
    {
        string body = e.Body;

        // retrieve parameters back as a data table
        // column = parameter
        DataTable table = _settings.GetDataTable();
        foreach (DataRow row in table.Rows)
        {
            if (string.IsNullOrEmpty((string)row["CloseTag"]))
                Parse(ref body, (string)row["Code"], (string)row["OpenTag"]);
            else
                Parse(ref body, (string)row["Code"], (string)row["OpenTag"], (string)row["CloseTag"]);
        }

        e.Body = body;
    }

    private static void Parse(ref string body, string code, string tag)
    {
        Parse(ref body, code, tag, tag);
    }

    /// <summary>
    /// Parses the BBCode into XHTML in a safe non-breaking manor.
    /// </summary>
    private static void Parse(ref string body, string code, string startTag, string endTag)
    {
        int start = body.IndexOf("[" + code + "]");
        if (start > -1)
        {
            if (body.IndexOf("[/" + code + "]", start) > -1)
            {
                body = body.Remove(start, code.Length + 2);
                body = body.Insert(start, "<" + startTag + ">");

                int end = body.IndexOf("[/" + code + "]", start);

                body = body.Remove(end, code.Length + 3);
                body = body.Insert(end, "</" + endTag + ">");

                Parse(ref body, code, startTag);
            }
        }
    }

}