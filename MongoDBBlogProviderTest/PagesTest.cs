using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using BlogEngine.Core;

namespace Tikal
{
    [TestFixture]
    public class PagesTest
    {
        [Test]
        public void InsertPage()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Page page = new Page
            {
                Title = "test title",
                Description = "test desc",
                Content = "test content",
                DateCreated = DateTime.MinValue,
                DateModified = DateTime.MinValue,
            };
            target.InsertPage(page);

        }
    }
}
