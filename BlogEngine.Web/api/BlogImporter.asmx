<%@ WebService Language="C#" Class="BlogImporter" %>

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Collections.ObjectModel;
using System.Web.Services;
using System.Web.Security;
using System.Web.Services.Protocols;
using System.Text.RegularExpressions;
using BlogEngine.Core;

/// <summary>
/// Web Service API for Blog Importer
/// </summary>
[WebService(Namespace = "http://dotnetblogengine.net/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class BlogImporter : System.Web.Services.WebService {
    /// <summary>
    /// Name/Type of Blog Software
    /// </summary>
    /// <returns>Blog Software name</returns>
    [WebMethod]
    public string BlogType() {
        return "BlogEngine.NET";
    }

    /// <summary>
    /// Version Number of the Blog
    /// </summary>
    /// <returns>Version number in string</returns>
    [WebMethod]
    public string BlogVersion() {
        return BlogSettings.Instance.Version();
    }

    /// <summary>
    /// Relative File Handler path
    /// </summary>
    /// <returns>file handler path as string</returns>
    [WebMethod]
    public string BlogFileHandler() {
        return "file.axd?file=";
    }

    /// <summary>
    /// Relative Image Handler path
    /// </summary>
    /// <returns>image handler path as string</returns>
    [WebMethod]
    public string BlogImageHandler() {
        return "image.axd?picture=";
    }

    /// <summary>
    /// Add new blog post to system
    /// </summary>
    /// <param name="import">ImportPost object</param>
    /// <param name="previousUrl">Old Post Url (for Url re-writing)</param>
    /// <param name="removeDuplicate">Search for duplicate post and remove?</param>
    /// <returns>string containing unique post identifier</returns>
    [SoapHeader("AuthenticationHeader")]
    [WebMethod]
    public string AddPost(ImportPost import, string previousUrl, bool removeDuplicate) {
        if (!IsAuthenticated())
            throw new InvalidOperationException("Wrong credentials");

        if (removeDuplicate) {
            if (!Post.IsTitleUnique(import.Title)) {
                // Search for matching post (by date and title) and delete it
                foreach (Post temp in Post.GetPostsByDate(import.PostDate.AddDays(-2), import.PostDate.AddDays(2))) {
                    if (temp.Title == import.Title) {
                        temp.Delete();
                        temp.Import();
                    }
                }
            }
        }

        Post post = new Post();
        post.Title = import.Title;
        post.Author = import.Author;
        post.DateCreated = import.PostDate;
        post.DateModified = import.PostDate; 
        post.Content = import.Content;
        post.Description = import.Description;
        post.IsPublished = import.Publish;
        //TODO: Save Previous Url?

        AddCategories(import.Categories, post);
        
        //Tag Support:
        if (import.Tags.Count == 0)
        {
            //No tags. Use categories. 
            post.Tags.AddRange(import.Categories);
        }
        else
        {
            post.Tags.AddRange(import.Tags);
        }
        
        post.Import();

        return post.Id.ToString();

    }

    /// <summary>
    /// Add Comment to specified post
    /// </summary>
    /// <param name="postID">postId as string</param>
    /// <param name="author">commenter username</param>
    /// <param name="email">commenter email</param>
    /// <param name="website">commenter url</param>
    /// <param name="description">actual comment</param>
    /// <param name="date">comment datetime</param>
    [SoapHeader("AuthenticationHeader")]
    [WebMethod]
    public void AddComment(string postID, string author, string email, string website, string description, DateTime date) {
        if (!IsAuthenticated())
            throw new InvalidOperationException("Wrong credentials");

        //Post post = Post.GetPost(new Guid(postID));
        Post post = Post.Load(new Guid(postID));
        if (post != null) {
            Comment comment = new Comment();
            comment.Id = Guid.NewGuid();
            comment.Author = author;
            comment.Email = email;
            Uri url;
            if (Uri.TryCreate(website, UriKind.Absolute, out url))
                comment.Website = url;

            comment.Content = description;
            comment.DateCreated = date;
            comment.Parent = post;
            comment.IsApproved = true;
            post.ImportComment(comment);
            post.Import();
        }
    }

    /// <summary>
    /// Force Reload of all posts
    /// </summary>
    [SoapHeader("AuthenticationHeader")]
    [WebMethod]
    public void ForceReload()
    {
        if (!IsAuthenticated())
            throw new InvalidOperationException("Wrong credentials");

        Post.Reload();
    }

    /// <summary>
    /// Downloads specified file to specified location
    /// </summary>
    /// <param name="source">source file path</param>
    /// <param name="destination">relative destination path</param>
    /// <returns></returns>
    [SoapHeader("AuthenticationHeader")]
    [WebMethod]
    public bool GetFile(string source, string destination) {
        bool response;
        try {
            string rootPath = BlogSettings.Instance.StorageLocation + "files/";
            string serverPath = Server.MapPath(rootPath);
            string saveFolder = serverPath;
            string fileName = destination;

            // Check/Create Folders & Fix fileName
            if (fileName.LastIndexOf('/') > -1) {
                saveFolder += fileName.Substring(0, fileName.LastIndexOf('/'));
                saveFolder = saveFolder.Replace('/', Path.DirectorySeparatorChar);

                fileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
            } else {
                if (saveFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    saveFolder = saveFolder.Substring(0, saveFolder.Length - 1);
            }

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);
            saveFolder += Path.DirectorySeparatorChar;

            using (WebClient client = new WebClient()) {
                client.DownloadFile(source, saveFolder + fileName);
            }
            response = true;
        } catch (Exception) {
            // The file probably didn't exist. No action needed.
            response = false;
        }

        return response;
    }

    private bool IsAuthenticated() {
        return Membership.ValidateUser(AuthenticationHeader.Username, AuthenticationHeader.Password);
    }

    public AuthHeader AuthenticationHeader;

    public class AuthHeader : SoapHeader {
        public string Username;
        public string Password;
    }

    public class ImportPost {
        public string Title;
        public string Author;
        public DateTime PostDate;
        public string Content;
        public string Description;
        public Collection<string> Categories;
        public Collection<string> Tags;
        public bool Publish;
    }

    private static void AddCategories(Collection<string> categories, Post post) {
        try {
            foreach (string category in categories) {
                bool added = false;
                foreach (Category cat in Category.Categories) {
                    if (cat.Title.Equals(category, StringComparison.OrdinalIgnoreCase)) {
                        post.Categories.Add(cat);
                        added = true;
                    }
                }
                if (!added) {
                    Category newCat = new Category(category, string.Empty);
                    newCat.Save();
                    post.Categories.Add(newCat);
                }
            }
        } catch (Exception ex) {
            string test = ex.Message;
        }
    }

}

