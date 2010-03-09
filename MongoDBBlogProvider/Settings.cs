using System;
using System.Collections.Specialized;
using MongoDB.Driver;

namespace Tikal
{
    partial class MongoDBBlogProvider
    {
        public override System.Collections.Specialized.StringDictionary LoadSettings()
        {
            StringDictionary dic;

            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection("settings");

                dic = new StringDictionary();

                foreach (var el in coll.FindAll().Documents)
                {
                    dic.Add((string)el["name"], (string)el["value"]);
                }
            }

            // ensure defaults

            if (dic["Theme"] == null)
                dic["Theme"] = "Standard";

            return dic;
        }

        public override void SaveSettings(System.Collections.Specialized.StringDictionary settings)
        {
            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection("settings");

                foreach (string key in settings.Keys)
                {
                    Document doc = new Document();
                    doc.Append("name", key);
                    doc.Append("value", settings[key]);

                    coll.Insert(doc);
                }
            }
        }

        public override string StorageLocation()
        {
            if (String.IsNullOrEmpty(System.Web.Configuration.WebConfigurationManager.AppSettings["StorageLocation"]))
                return @"~/app_data/";
            return System.Web.Configuration.WebConfigurationManager.AppSettings["StorageLocation"];
        }
    }
}
