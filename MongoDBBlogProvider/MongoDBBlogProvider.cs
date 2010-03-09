using System;
using System.Collections.Generic;
using System.Text;
using BlogEngine.Core.Providers;

namespace Tikal
{
    public partial class MongoDBBlogProvider : BlogProvider
    {

        internal string _Folder
        {
            get
            {
                string p = StorageLocation().Replace("~/", "");
                return System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, p);
            }
        }

        public override void DeleteBlogRollItem(BlogEngine.Core.BlogRollItem blogRollItem)
        {
            throw new NotImplementedException();
        }

        public override List<BlogEngine.Core.BlogRollItem> FillBlogRoll()
        {
            throw new NotImplementedException();
        }

        public override List<BlogEngine.Core.Referrer> FillReferrers()
        {
            throw new NotImplementedException();
        }

        public override void InsertBlogRollItem(BlogEngine.Core.BlogRollItem blogRollItem)
        {
            throw new NotImplementedException();
        }

        public override void InsertReferrer(BlogEngine.Core.Referrer referrer)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.Specialized.StringCollection LoadPingServices()
        {
            throw new NotImplementedException();
        }


        public override System.Collections.Specialized.StringCollection LoadStopWords()
        {
            return new System.Collections.Specialized.StringCollection();
        }

        public override void SavePingServices(System.Collections.Specialized.StringCollection services)
        {
            throw new NotImplementedException();
        }

        public override BlogEngine.Core.BlogRollItem SelectBlogRollItem(Guid Id)
        {
            throw new NotImplementedException();
        }

        public override void UpdateBlogRollItem(BlogEngine.Core.BlogRollItem blogRollItem)
        {
            throw new NotImplementedException();
        }

        public override void UpdateReferrer(BlogEngine.Core.Referrer referrer)
        {
            throw new NotImplementedException();
        }

        public override BlogEngine.Core.Referrer SelectReferrer(Guid Id)
        {
            throw new NotImplementedException();
        }
    }
}
