using System;
using System.Collections.Generic;
using System.Linq;
using BlogEngine.Core;
using MongoDB.Driver;

namespace Tikal
{
    public static class Converter
    {
        public static Post ToPost(this Document doc)
        {
            Post post = new Post();

            post.Title = (string)doc["title"];
            post.Description = (string)doc["description"];
            post.Content = (string)doc["content"];

            if (doc["pubDate"] != null)
                post.DateCreated = (DateTime)doc["pubDate"];

            if (doc["lastModified"] != null)
                post.DateModified = (DateTime)doc["lastModified"];

            if (doc["author"] != null)
                post.Author = (string)doc["author"];

            if (doc["ispublished"] != null)
                post.IsPublished = (bool)doc["ispublished"];

            if (doc["iscommentsenabled"] != null)
                post.IsCommentsEnabled = (bool)doc["iscommentsenabled"];

            if (doc["raters"] != null)
                post.Raters = (int)doc["raters"];

            if (doc["rating"] != null)
                post.Rating = (float)doc["rating"];

            if (doc["slug"] != null)
                post.Slug = (string)doc["slug"];


            // Tags
            string[] tags = (string[])doc["tags"];
            foreach (string tag in tags)
            {
                if (!string.IsNullOrEmpty(tag))
                    post.Tags.Add(tag);
            }

            // categories
            string[] categories = (string[])doc["categories"];
            foreach (string category in categories)
            {
                Guid key = new Guid(category);
                Category cat = Category.GetCategory(key);
                if (cat != null)
                    post.Categories.Add(cat);
            }

            // Notification e-mails
            string[] notifications = (string[])doc["notifications"];
            foreach (string notification in notifications)
            {
                post.NotificationEmails.Add(notification);
            }

            // comments			
            Document[] docComments = (Document[])doc["comments"];
            foreach (Document docComment in docComments)
            {
                Comment comment = docComment.ToComment();
                comment.Parent = post;
                post.Comments.Add(comment);
            }

            return post;
        }

        public static Document ToDocument(this Post post)
        {
            Document doc = new Document();

            if (post.Title != null)
                doc["title"] = post.Title;

            if (post.Description != null)
                doc["description"] = post.Description;

            if (post.Content != null)
                doc["content"] = post.Content;

            if (post.DateCreated != null)
                doc["pubDate"] = post.DateCreated;

            if (post.DateModified != null)
                doc["lastModified"] = post.DateModified;

            if (post.Author != null)
                doc["author"] = post.Author;

            if (post.IsPublished != null)
                doc["ispublished"] = post.IsPublished;

            if (post.IsCommentsEnabled != null)
                doc["iscommentsenabled"] = post.IsCommentsEnabled;

            if (post.Raters != null)
                doc["raters"] = post.Raters;

            if (post.Rating != null)
                doc["rating"] = post.Rating;

            if (post.Slug != null)
                doc["slug"] = post.Slug;

            if (post.Tags != null)
                doc["tags"] = post.Tags.ToArray();

            if (post.Categories != null)
                doc["categories"] = post.Categories.Select(p => p.Id).ToArray();

            if (post.NotificationEmails != null)
                doc["notifications"] = post.NotificationEmails.ToArray();

            if (post.Comments != null)
            {
                List<Document> docComment = new List<Document>();
                foreach (Comment comment in post.Comments)
                {
                    Document docCom = comment.ToDocument();
                    doc.Add("comments", docCom);
                }
                doc["comments"] = docComment.ToArray();
            }
            return doc;
        }

        public static Comment ToComment(this Document doc)
        {
            Comment comment = new Comment();
            comment.Id = (Guid)doc["id"];
            comment.ParentId = ((Guid)doc["parentid"] != null) ? (Guid)doc["parentid"] : Guid.Empty;
            comment.Author = (string)doc["author"];
            comment.Email = (string)doc["email"];

            if (doc["country"] != null)
                comment.Country = (string)doc["country"];

            if (doc["ip"] != null)
                comment.IP = (string)doc["ip"];

            if (doc["website"] != null)
            {
                Uri website;
                if (Uri.TryCreate((string)doc["website"], UriKind.Absolute, out website))
                    comment.Website = website;
            }

            if (doc["moderatedby"] != null)
                comment.ModeratedBy = (string)doc["moderatedby"];

            if (doc["approved"] != null)
                comment.IsApproved = (bool)doc["approved"];
            else
                comment.IsApproved = true;

            if (doc["avatar"] != null)
            {
                comment.Avatar = (string)doc["avatar"];
            }

            comment.Content = (string)doc["content"];
            comment.DateCreated = (DateTime)doc["date"];

            return comment;
        }

        public static Document ToDocument(this Comment comment)
        {
            Document doc = new Document();

            doc["id"] = comment.Id;
            doc["parentid"] = comment.ParentId;
            doc["author"] = comment.Author;
            doc["email"] = comment.Email;
            doc["country"] = comment.Country;
            doc["ip"] = comment.IP;
            doc["website"] = comment.Website;
            doc["moderatedby"] = comment.ModeratedBy;
            doc["approved"] = comment.IsApproved;
            doc["avatar"] = comment.Avatar;
            doc["content"] = comment.Content;
            doc["date"] = comment.DateCreated;

            return doc;
        }
    }
}
