using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Xml.Serialization;
using System.IO;

namespace Tikal
{
    partial class MongoDBBlogProvider
    {

        public override object LoadFromDataStore(BlogEngine.Core.DataStore.ExtensionType exType, string exId)
        {
            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection("DataStoreSettings");

                Document spec = new Document();
                spec["ExtensionType"] = exType;
                spec["ExtensionId"] = exId;

                var res = coll.FindOne(spec);
                if (res == null)
                    return null;

                return res["Settings"];
            }
        }

        public override void RemoveFromDataStore(BlogEngine.Core.DataStore.ExtensionType exType, string exId)
        {
            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection("DataStoreSettings");
                
                Document spec = new Document();
                spec["ExtensionType"] = exType;
                spec["ExtensionId"] = exId;
                
                coll.Delete(spec);
            }
        }

        public override void SaveToDataStore(BlogEngine.Core.DataStore.ExtensionType exType, string exId, object settings)
        {
            XmlSerializer xs = new XmlSerializer(settings.GetType());
            string objectXML = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                xs.Serialize(sw, settings);
                objectXML = sw.ToString();
            }

            using (var mongo = new MongoDbWr())
            {
                var coll = mongo.BlogDB.GetCollection("DataStoreSettings");

                Document spec = new Document();
                spec["ExtensionType"] = exType;
                spec["ExtensionId"] = exId;

                var res = new Document();
                res["Settings"] = objectXML;
                res["ExtensionType"] = exType;
                res["ExtensionId"] = exId;

                coll.Update(res, spec, UpdateFlags.Upsert);
            }
        }

    }
}
