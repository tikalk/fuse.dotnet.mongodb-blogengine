#region Using

using System;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.Specialized;
using BlogEngine.Core;

#endregion

namespace Tikal
{
    partial class MongoDBBlogProvider
    {

        #region Categories

        /// <summary>
        /// Gets a Category based on a Guid
        /// </summary>
        /// <param name="id">The category's Guid.</param>
        /// <returns>A matching Category</returns>
        public override Category SelectCategory(Guid id)
        {
            List<Category> categories = Category.Categories;

            Category category = new Category();

            foreach (Category cat in categories)
            {
                if (cat.Id == id)
                    category = cat;
            }
            category.MarkOld();
            return category;
        }

        /// <summary>
        /// Inserts a Category
        /// </summary>
        /// <param name="category">Must be a valid Category object.</param>
        public override void InsertCategory(Category category)
        {
            List<Category> categories = Category.Categories;
            categories.Add(category);

            WriteToFile();
        }

        /// <summary>
        /// Updates a Category
        /// </summary>
        /// <param name="category">Must be a valid Category object.</param>
        public override void UpdateCategory(Category category)
        {
            List<Category> categories = Category.Categories;
            categories.Remove(category);
            categories.Add(category);

            WriteToFile();
        }

        /// <summary>
        /// Deletes a Category
        /// </summary>
        /// <param name="category">Must be a valid Category object.</param>
        public override void DeleteCategory(Category category)
        {
            List<Category> categories = Category.Categories;
            categories.Remove(category);

            if (Category.Categories.Contains(category))
                Category.Categories.Remove(category);

            WriteToFile();
        }

        /// <summary>
        /// Saves the Categories to disk.
        /// </summary>
        private void WriteToFile()
        {
            List<Category> categories = Category.Categories;
            string fileName = _Folder + "categories.xml";

            using (XmlTextWriter writer = new XmlTextWriter(fileName, System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("categories");

                foreach (Category cat in categories)
                {
                    writer.WriteStartElement("category");
                    writer.WriteAttributeString("id", cat.Id.ToString());
                    writer.WriteAttributeString("description", cat.Description);
                    writer.WriteAttributeString("parent", cat.Parent.ToString());
                    writer.WriteValue(cat.Title);
                    writer.WriteEndElement();
                    cat.MarkOld();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Fills an unsorted list of categories.
        /// </summary>
        /// <returns>A List&lt;Category&gt; of all Categories.</returns>
        public override List<Category> FillCategories()
        {

            string fileName = _Folder + "categories.xml";
            if (!File.Exists(fileName))
                return null;

            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            List<Category> categories = new List<Category>();

            foreach (XmlNode node in doc.SelectNodes("categories/category"))
            {
                Category category = new Category();

                category.Id = new Guid(node.Attributes["id"].InnerText);
                category.Title = node.InnerText;
                if (node.Attributes["description"] != null)
                    category.Description = node.Attributes["description"].InnerText;
                else
                    category.Description = string.Empty;
                if (node.Attributes["parent"] != null)
                {
                    if (String.IsNullOrEmpty(node.Attributes["parent"].InnerText))
                        category.Parent = null;
                    else
                        category.Parent = new Guid(node.Attributes["parent"].InnerText);
                }
                else
                    category.Parent = null;

                categories.Add(category);
                category.MarkOld();
            }

            return categories;
        }

        #endregion

    }
}
