using System;

namespace AguaInteligente
{
    public class ControllerService
    {
        public Uri ServiceUri { get; }

        public ControllerService(string serviceUri)
        {
            ServiceUri = new Uri(serviceUri.TrimEnd('/'));
        }

        /// <summary>
        /// connect to the service and get the status to see if 
        /// the irrigation system is currently on (true) or 
        /// off (false)
        /// </summary>
        public bool GetIrrigationStatus()
        {
            throw new NotImplementedException();
        }
    }
}

