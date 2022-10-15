using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Emaratech.Services.Channels.App_Start
{
    [Serializable]
    public class AppConfigSettings
    {
        private static string USER_TYPE = null;
        private static string SYSTEM_ID = null;
        
        public static string UserTypeLookup
        {
            get
            {
                if (string.IsNullOrEmpty(USER_TYPE))
                {
                    USER_TYPE = ConfigurationManager.AppSettings["UserTypeLookupId"];
                }

                return USER_TYPE;
            }
        }

        public static string SystemId
        {
            get
            {
                if (string.IsNullOrEmpty(SYSTEM_ID))
                {
                    SYSTEM_ID = ConfigurationManager.AppSettings["SystemId"];
                }

                return SYSTEM_ID;
            }
        }
    }
}