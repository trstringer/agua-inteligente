using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AguaInteligente;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AguaInteligente.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private LocalCache _localCache = new LocalCache();
        private ControllerService _service;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            string serviceUrl = _localCache.ServiceUrl;

            // set pending initially
            SetControlsFromServiceStatus(ServiceStatus.Pending);

            if (string.IsNullOrWhiteSpace(serviceUrl))
            {
                SetControlsFromServiceStatus(ServiceStatus.NotConfigured);
            }
            else
            {
                _service = new ControllerService(serviceUrl);
                ServiceStatus status = await _service.GetServiceStatus();
                SetControlsFromServiceStatus(status);
            }
            
            base.OnNavigatedTo(e);
        }

        private void SetControlsFromServiceStatus(ServiceStatus serviceStatus)
        {
            switch (serviceStatus)
            {
                case ServiceStatus.Available:
                    ServiceStatusDisplay.Text = "Available";
                    ToggleIrrigation.IsEnabled = true;
                    SetFromSprinklerStatus();
                    break;
                case ServiceStatus.Unavailable:
                    ServiceStatusDisplay.Text = "Unavailable";
                    NoServiceConnection();
                    break;
                case ServiceStatus.NotConfigured:
                    ServiceStatusDisplay.Text = "Not configured";
                    NoServiceConnection();
                    break;
                case ServiceStatus.Pending:
                    ServiceStatusDisplay.Text = "Pending";
                    NoServiceConnection();
                    break;
                default:
                    break;
            }
        }
        private void NoServiceConnection()
        {
            ToggleIrrigation.IsEnabled = false;
            ToggleIrrigation.Content = "No service connection";
            SprinklerStatusDisplay.Text = "--";
        }
        private async void SetFromSprinklerStatus()
        {
            ToggleIrrigation.Content = "";
            SprinklerStatusDisplay.Text = "";

            if (await _service.GetIrrigationStatus())
            {
                // sprinklers are on
                ToggleIrrigation.Content = "Stop";
                SprinklerStatusDisplay.Text = "On";
            }
            else
            {
                // sprinklers are off
                ToggleIrrigation.Content = "Start";
                SprinklerStatusDisplay.Text = "Off";
            }
        }

        private void Settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private async void ToggleIrrigation_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (await _service.GetIrrigationStatus())
            {
                // irrigation is on, so we must want to turn it off
                await _service.StopIrrigation();
            }
            else
            {
                // irrigation is off, so we want to turn it on
                await _service.StartIrrigation(10);
            }

            SetFromSprinklerStatus();
        }
    }
}
