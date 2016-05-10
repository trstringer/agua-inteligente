using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AguaInteligente
{
    public class ControllerService
    {
        public string ServiceUri { get; }

        public ControllerService(string serviceUri)
        {
            ServiceUri = serviceUri.TrimEnd('/');
        }

        private async Task<HttpResponseMessage> ApiWebRequest(Uri uri, string httpVerb, HttpContent content)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response;

            switch (httpVerb.ToLower())
            {
                case "get":
                    response = await client.GetAsync(uri);
                    break;
                case "post":
                    response = await client.PostAsync(uri, null);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Unrecognized or unsupported very {0}", httpVerb));
            }

            return response;
        }
        private Task<HttpResponseMessage> ApiWebRequest(Uri uri, string httpVerb)
        {
            return ApiWebRequest(uri, httpVerb, null);
        }

        /// <summary>
        /// find out if the service is currently available
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceStatus> GetServiceStatus()
        {
            try
            {
                await GetIrrigationStatus();
                return ServiceStatus.Available;
            }
            catch
            {
                return ServiceStatus.Unavailable;
            }
        }

        /// <summary>
        /// connect to the service and get the status to see if 
        /// the irrigation system is currently on (true) or 
        /// off (false)
        /// </summary>
        public async Task<bool> GetIrrigationStatus()
        {
            string requestUri = string.Format("{0}/status", ServiceUri);
            HttpResponseMessage response = await ApiWebRequest(new Uri(requestUri), "get");

            if (response.IsSuccessStatusCode)
            {
                return Convert.ToBoolean(await response.Content.ReadAsStringAsync());
            }
            else
            {
                throw new WebException(string.Format("Web request error while retrieving status. Status code {0}", response.StatusCode), WebExceptionStatus.SendFailure);
            }
        }

        /// <summary>
        /// start the irrigation for a set number of seconds
        /// </summary>
        public async Task StartIrrigation(int seconds)
        {
            string requestUri = string.Format("{0}/on/{1}", ServiceUri, seconds);
            HttpResponseMessage response = await ApiWebRequest(new Uri(requestUri), "post");

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new WebException(string.Format("Web request error while starting irrigation. Status code {0}", response.StatusCode), WebExceptionStatus.SendFailure);
            }
        }
        /// <summary>
        /// start the irrigation for the default time (5 minutes)
        /// </summary>
        public Task StartIrrigation()
        {
            return StartIrrigation(5 * 60);
        }

        /// <summary>
        /// stop the irrigation system immediately
        /// </summary>
        /// <returns></returns>
        public async Task StopIrrigation()
        {
            string requestUri = string.Format("{0}/off", ServiceUri);
            HttpResponseMessage response = await ApiWebRequest(new Uri(requestUri), "post");

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new WebException(string.Format("Web request error while stopping irrigation. Status code {0}", response.StatusCode), WebExceptionStatus.SendFailure);
            }
        }
    }
}

