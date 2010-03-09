using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Collections.Specialized;

namespace Tikal
{
    partial class MongoDBBlogProvider
    {
        public override System.Collections.Specialized.StringDictionary LoadSettings()
        {
            var db = ConnectDB();

            var coll = db.GetCollection("settings");

            var dic = new StringDictionary();

            foreach (var el in coll.FindAll().Documents)
            {
                dic.Add((string)el["name"], (string)el["value"]);
            }

            return dic;
        }

        private Database ConnectDB()
        {
            Mongo mongo = new Mongo();
            mongo.Connect();

            var db = mongo.getDB("BlogDB");
            return db;
        }

        public override void SaveSettings(System.Collections.Specialized.StringDictionary settings)
        {
            throw new NotImplementedException();
        }

        public override string StorageLocation()
        {
            if (String.IsNullOrEmpty(System.Web.Configuration.WebConfigurationManager.AppSettings["StorageLocation"]))
                return @"~/app_data/";
            return System.Web.Configuration.WebConfigurationManager.AppSettings["StorageLocation"];
        }
    }
}
