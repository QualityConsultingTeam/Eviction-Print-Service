using environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintingWebAPI.Util
{
    public class WebAPIEnvironmentConfig : RuntimeEnvironmentConfig
    {
        override
        protected void initialize()
        {
            settings.Add(DataEnvironmentConstants.COLLECTION, "nwe.dev");
            settings.Add(DataEnvironmentConstants.KEY, "1TC7QPDWWH37EAEGH2R2");
            settings.Add(DataEnvironmentConstants.SECURE_KEY, "OEnyWyNgx058Q4s+pt7IyCMt3IRpec1H9rHWEK+a");
        }

        override
        public bool requiresSecureConnection()
        {
            return false;
        }

        override
        public bool supportsAds()
        {
            return false;
        }
    }
}
