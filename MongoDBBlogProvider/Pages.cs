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
                spec["Id"] = id.ToString();

                var doc = coll.FindOne(spec);

                return DocumentToPage(doc);
            }
        }

        public override void InsertPage(BlogEngine.Core.Page page)
        {
            throw new NotImplementedException();
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

        private BlogEngine.Core.Page DocumentToPage(Document doc)
        {
            return (Page)doc["Page"];
            
            //return page = new Page
            //{
            //    Title = doc[""],
            //    Description = doc[""],
            //    Content = doc[""],
            //    Keywords = doc[""],
            //    Slug = doc[""],
            //    Parent = doc[""],
            //    AbsoluteLink = doc[""],
            //};

            //return null;
        }

    }
}
