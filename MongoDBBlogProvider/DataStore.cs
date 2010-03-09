using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Tikal
{
    partial class MongoDBBlogProvider
    {

        public override object LoadFromDataStore(BlogEngine.Core.DataStore.ExtensionType exType, string exId)
        {
            var db = ConnectDB();

            var coll = db.GetCollection("DataStoreSettings");
            
            Document spec = new Document();
            spec["ExtensionType"] = exType;
            spec["ExtensionId"] = exId;

            var res = coll.FindOne(spec);
            if(res == null)
                return null;

            return res["Settings"];
        }

        public override void RemoveFromDataStore(BlogEngine.Core.DataStore.ExtensionType exType, string exId)
        {
            throw new NotImplementedException();
        }

        public override void SaveToDataStore(BlogEngine.Core.DataStore.ExtensionType exType, string exId, object settings)
        {
            throw new NotImplementedException();
        }

    }
}
