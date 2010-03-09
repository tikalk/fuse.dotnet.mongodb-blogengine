using System;
using System.Collections.Generic;
using BlogEngine.Core;
using MongoDB.Driver;

namespace Tikal
{
    public partial class MongoDBBlogProvider
    {
        /// <summary>
        /// Retrieves a post based on the specified Id.
        /// </summary>
        public override Post SelectPost(Guid id)
        {
            Post post;

            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection("settings");

                Document spec = new Document();
                spec.Add("id", id);

                Document docPost = coll.FindOne(spec);
                post = docPost.ToPost();
            }

            return post;
        }


        /// <summary>
        /// Inserts a new Post to the data store.
        /// </summary>
        /// <param name="post"></param>
        public override void InsertPost(Post post)
        {
            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection("posts");
                coll.Insert(post.ToDocument());
            }
        }


        /// <summary>
        /// Updates a Post.
        /// </summary>
        public override void UpdatePost(Post post)
        {
            DeletePost(post);
            InsertPost(post);
        }

        /// <summary>
        /// Deletes a post from the data store.
        /// </summary>
        public override void DeletePost(Post post)
        {
            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection("posts");

                Document spec = new Document();
                spec.Add("id", post.Id);
                coll.Delete(spec);
            }
        }

        /// <summary>
        /// Retrieves all posts from the data store
        /// </summary>
        /// <returns>List of Posts</returns>
        public override List<Post> FillPosts()
        {
            List<Post> posts = new List<Post>();

            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection("posts");

                foreach (Document doc in coll.FindAll().Documents)
                {
                    posts.Add(doc.ToPost());
                }
            }
            return posts;
        }

    }
}