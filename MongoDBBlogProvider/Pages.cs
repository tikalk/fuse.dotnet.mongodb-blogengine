using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using BlogEngine.Core;

namespace Tikal
{
    partial class MongoDBBlogProvider
    {
        private const string COLLECTION_PAGES = "Pages";

        public override BlogEngine.Core.Page SelectPage(Guid id)
        {
            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection(COLLECTION_PAGES);

                var spec = new Document();
                spec["Id"] = id;

                var doc = coll.FindOne(spec);

                return DocumentToPage(doc);
            }
        }

        public override void InsertPage(BlogEngine.Core.Page page)
        {
            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection(COLLECTION_PAGES);

                var record = PageToDocument(page);

                coll.Insert(record);
            }
        }

        public override void DeletePage(BlogEngine.Core.Page page)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePage(BlogEngine.Core.Page page)
        {
            throw new NotImplementedException();
        }

        public override List<BlogEngine.Core.Page> FillPages()
        {
            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection(COLLECTION_PAGES);

                return coll.FindAll().Documents.Select(doc => DocumentToPage(doc)).ToList();
            }
        }

        private Document PageToDocument(Page page)
        {
            var doc = new Document();
            doc["Id"] = page.Id;
            doc["Title"] = page.Title;
            doc["Description"] = page.Description;
            doc["Content"] = page.Content;
            doc["Keywords"] = page.Keywords;
            doc["Slug"] = page.Slug;
            doc["Parent"] = page.Parent;
            doc["IsFrontPage"] = page.IsFrontPage;
            doc["ShowInList"] = page.ShowInList;
            doc["IsPublished"] = page.IsPublished;
            doc["DateCreated"] = page.DateCreated;
            doc["DateModified"] = page.DateModified;

            return doc;
        }

        private BlogEngine.Core.Page DocumentToPage(Document doc)
        {
            return  new Page
            {
                Id = (Guid)doc["Id"],
                Title = (string)doc["Title"],
                Description = (string)doc["Description"],
                Content = (string)doc["Content"],
                Keywords = (string)doc["Keywords"],
                Slug = (string)doc["Slug"],
                Parent = (Guid)(doc["Parent"] ?? Guid.Empty),
                IsFrontPage = (bool)(doc["IsFrontPage"] ?? false),
                ShowInList = (bool)(doc["ShowInList"] ?? false),
                IsPublished = (bool)(doc["IsPublished"] ?? false),
                DateCreated = (DateTime)doc["DateCreated"],
                DateModified = (DateTime)doc["DateModified"],
            };
        }

    }
}
