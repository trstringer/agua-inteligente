using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AguaInteligente.UWP
{
    public class LocalCache
    {
        private const string _localSettingsServiceUrlKey = "serviceUrl";

        public string ServiceUrl {
            get
            {
                return GetCachedServiceUrl();
            }
            set
            {
                CacheServiceUrl(value);
            }
        }

        private void CacheServiceUrl(string serviceUrl)
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            settings.Values[_localSettingsServiceUrlKey] = serviceUrl;
        }
        private string GetCachedServiceUrl()
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            var serviceUrl = settings.Values[_localSettingsServiceUrlKey];
            return serviceUrl != null ? serviceUrl.ToString() : "";
        }
    }
}
