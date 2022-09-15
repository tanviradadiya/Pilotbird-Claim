using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Configuration
{
    public class ApplicationSettings
    {
        public string ConnectionStringKeyNameForIdentityDataStore { get; set; }
        public string ConnectionStringKeyNameForQueryDataStore { get; set; }

        public string BaseB2BWebsiteUrl { get; set; }
    }
}
