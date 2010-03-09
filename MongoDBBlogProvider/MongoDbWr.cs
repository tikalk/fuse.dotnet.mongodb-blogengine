using System;
using MongoDB.Driver;

namespace Tikal
{
    public class MongoDbWr : IDisposable
    {
        Mongo mongo;

        public Database BlogDB
        {
            get
            {
                return mongo.getDB("BlogDB");
            }
        }

        public MongoDbWr()
        {
            mongo = new Mongo();
            mongo.Connect();
        }

        #region IDisposable Members

        public void Dispose()
        {
            mongo.Disconnect();
        }

        #endregion
    }
}
