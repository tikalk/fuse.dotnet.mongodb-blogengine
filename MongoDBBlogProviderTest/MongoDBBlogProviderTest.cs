using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BlogEngine.Core;
using BlogEngine.Core.DataStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tikal;

namespace MongoDBBlogProviderTest
{


    /// <summary>
    ///This is a test class for MongoDBBlogProviderTest and is intended
    ///to contain all MongoDBBlogProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MongoDBBlogProviderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for UpdateReferrer
        ///</summary>
        [TestMethod()]
        public void UpdateReferrerTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Referrer referrer = null; // TODO: Initialize to an appropriate value
            target.UpdateReferrer(referrer);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for UpdateProfile
        ///</summary>
        [TestMethod()]
        public void UpdateProfileTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            AuthorProfile profile = null; // TODO: Initialize to an appropriate value
            target.UpdateProfile(profile);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for UpdatePost
        ///</summary>
        [TestMethod()]
        public void UpdatePostTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Post post = null; // TODO: Initialize to an appropriate value
            target.UpdatePost(post);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for UpdatePage
        ///</summary>
        [TestMethod()]
        public void UpdatePageTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Page page = null; // TODO: Initialize to an appropriate value
            target.UpdatePage(page);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for UpdateCategory
        ///</summary>
        [TestMethod()]
        public void UpdateCategoryTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Category category = null; // TODO: Initialize to an appropriate value
            target.UpdateCategory(category);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for UpdateBlogRollItem
        ///</summary>
        [TestMethod()]
        public void UpdateBlogRollItemTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            BlogRollItem blogRollItem = null; // TODO: Initialize to an appropriate value
            target.UpdateBlogRollItem(blogRollItem);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for StorageLocation
        ///</summary>
        [TestMethod()]
        public void StorageLocationTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.StorageLocation();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SelectReferrer
        ///</summary>
        [TestMethod()]
        public void SelectReferrerTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Guid Id = new Guid(); // TODO: Initialize to an appropriate value
            Referrer expected = null; // TODO: Initialize to an appropriate value
            Referrer actual;
            actual = target.SelectReferrer(Id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SelectProfile
        ///</summary>
        [TestMethod()]
        public void SelectProfileTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            string id = string.Empty; // TODO: Initialize to an appropriate value
            AuthorProfile expected = null; // TODO: Initialize to an appropriate value
            AuthorProfile actual;
            actual = target.SelectProfile(id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SelectPost
        ///</summary>
        [TestMethod()]
        public void SelectPostTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Guid id = new Guid(); // TODO: Initialize to an appropriate value
            Post expected = null; // TODO: Initialize to an appropriate value
            Post actual;
            actual = target.SelectPost(id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SelectPage
        ///</summary>
        [TestMethod()]
        public void SelectPageTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Guid id = new Guid(); // TODO: Initialize to an appropriate value
            Page expected = null; // TODO: Initialize to an appropriate value
            Page actual;
            actual = target.SelectPage(id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SelectCategory
        ///</summary>
        [TestMethod()]
        public void SelectCategoryTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Guid id = new Guid(); // TODO: Initialize to an appropriate value
            Category expected = null; // TODO: Initialize to an appropriate value
            Category actual;
            actual = target.SelectCategory(id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SelectBlogRollItem
        ///</summary>
        [TestMethod()]
        public void SelectBlogRollItemTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Guid Id = new Guid(); // TODO: Initialize to an appropriate value
            BlogRollItem expected = null; // TODO: Initialize to an appropriate value
            BlogRollItem actual;
            actual = target.SelectBlogRollItem(Id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SaveToDataStore
        ///</summary>
        [TestMethod()]
        public void SaveToDataStoreTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            ExtensionType exType = new ExtensionType(); // TODO: Initialize to an appropriate value
            string exId = string.Empty; // TODO: Initialize to an appropriate value
            object settings = null; // TODO: Initialize to an appropriate value
            target.SaveToDataStore(exType, exId, settings);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SaveSettings
        ///</summary>
        [TestMethod()]
        public void SaveSettingsTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            StringDictionary settings = new StringDictionary();
            settings.Add("key1", "value1");
            settings.Add("key2", "value2");
            target.SaveSettings(settings);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SavePingServices
        ///</summary>
        [TestMethod()]
        public void SavePingServicesTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            StringCollection services = null; // TODO: Initialize to an appropriate value
            target.SavePingServices(services);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RemoveFromDataStore
        ///</summary>
        [TestMethod()]
        public void RemoveFromDataStoreTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            ExtensionType exType = new ExtensionType(); // TODO: Initialize to an appropriate value
            string exId = string.Empty; // TODO: Initialize to an appropriate value
            target.RemoveFromDataStore(exType, exId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for LoadStopWords
        ///</summary>
        [TestMethod()]
        public void LoadStopWordsTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            StringCollection expected = null; // TODO: Initialize to an appropriate value
            StringCollection actual;
            actual = target.LoadStopWords();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LoadSettings
        ///</summary>
        [TestMethod()]
        public void LoadSettingsTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider();
            StringDictionary actual;
            actual = target.LoadSettings();

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for LoadPingServices
        ///</summary>
        [TestMethod()]
        public void LoadPingServicesTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            StringCollection expected = null; // TODO: Initialize to an appropriate value
            StringCollection actual;
            actual = target.LoadPingServices();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LoadFromDataStore
        ///</summary>
        [TestMethod()]
        public void LoadFromDataStoreTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            ExtensionType exType = new ExtensionType(); // TODO: Initialize to an appropriate value
            string exId = string.Empty; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.LoadFromDataStore(exType, exId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InsertReferrer
        ///</summary>
        [TestMethod()]
        public void InsertReferrerTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Referrer referrer = null; // TODO: Initialize to an appropriate value
            target.InsertReferrer(referrer);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for InsertProfile
        ///</summary>
        [TestMethod()]
        public void InsertProfileTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            AuthorProfile profile = null; // TODO: Initialize to an appropriate value
            target.InsertProfile(profile);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for InsertPost
        ///</summary>
        [TestMethod()]
        public void InsertPostTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Post post = null; // TODO: Initialize to an appropriate value
            target.InsertPost(post);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for InsertPage
        ///</summary>
        [TestMethod()]
        public void InsertPageTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Page page = new Page
            {
                Title="test title",
                Description="test desc",
                Content="test content",
            };
            target.InsertPage(page);
        }

        /// <summary>
        ///A test for InsertCategory
        ///</summary>
        [TestMethod()]
        public void InsertCategoryTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Category category = null; // TODO: Initialize to an appropriate value
            target.InsertCategory(category);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for InsertBlogRollItem
        ///</summary>
        [TestMethod()]
        public void InsertBlogRollItemTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            BlogRollItem blogRollItem = null; // TODO: Initialize to an appropriate value
            target.InsertBlogRollItem(blogRollItem);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for FillReferrers
        ///</summary>
        [TestMethod()]
        public void FillReferrersTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            List<Referrer> expected = null; // TODO: Initialize to an appropriate value
            List<Referrer> actual;
            actual = target.FillReferrers();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FillProfiles
        ///</summary>
        [TestMethod()]
        public void FillProfilesTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            List<AuthorProfile> expected = null; // TODO: Initialize to an appropriate value
            List<AuthorProfile> actual;
            actual = target.FillProfiles();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FillPosts
        ///</summary>
        [TestMethod()]
        public void FillPostsTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            List<Post> expected = null; // TODO: Initialize to an appropriate value
            List<Post> actual;
            actual = target.FillPosts();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FillPages
        ///</summary>
        [TestMethod()]
        public void FillPagesTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            List<Page> expected = null; // TODO: Initialize to an appropriate value
            List<Page> actual;
            actual = target.FillPages();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FillCategories
        ///</summary>
        [TestMethod()]
        public void FillCategoriesTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            List<Category> expected = null; // TODO: Initialize to an appropriate value
            List<Category> actual;
            actual = target.FillCategories();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FillBlogRoll
        ///</summary>
        [TestMethod()]
        public void FillBlogRollTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            List<BlogRollItem> expected = null; // TODO: Initialize to an appropriate value
            List<BlogRollItem> actual;
            actual = target.FillBlogRoll();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeleteProfile
        ///</summary>
        [TestMethod()]
        public void DeleteProfileTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            AuthorProfile profile = null; // TODO: Initialize to an appropriate value
            target.DeleteProfile(profile);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DeletePost
        ///</summary>
        [TestMethod()]
        public void DeletePostTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Post post = null; // TODO: Initialize to an appropriate value
            target.DeletePost(post);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DeletePage
        ///</summary>
        [TestMethod()]
        public void DeletePageTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Page page = null; // TODO: Initialize to an appropriate value
            target.DeletePage(page);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DeleteCategory
        ///</summary>
        [TestMethod()]
        public void DeleteCategoryTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            Category category = null; // TODO: Initialize to an appropriate value
            target.DeleteCategory(category);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DeleteBlogRollItem
        ///</summary>
        [TestMethod()]
        public void DeleteBlogRollItemTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider(); // TODO: Initialize to an appropriate value
            BlogRollItem blogRollItem = null; // TODO: Initialize to an appropriate value
            target.DeleteBlogRollItem(blogRollItem);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for MongoDBBlogProvider Constructor
        ///</summary>
        [TestMethod()]
        public void MongoDBBlogProviderConstructorTest()
        {
            MongoDBBlogProvider target = new MongoDBBlogProvider();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
