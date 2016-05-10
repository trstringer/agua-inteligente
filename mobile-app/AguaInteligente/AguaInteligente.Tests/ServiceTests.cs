using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using AguaInteligente;

namespace AguaInteligente.Tests
{
    public class ServiceTests
    {
        private readonly ITestOutputHelper _output;

        private const string _serviceUrl = "http://192.168.1.200:3000/";
        private const string _badServiceUrl = "http://192.168.1.200:2999/";

        public ServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async void GetServiceStatus()
        {
            ControllerService service = new ControllerService(_serviceUrl);

            ServiceStatus status = await service.GetServiceStatus();
            Assert.True(status == ServiceStatus.Available);

            service = new ControllerService(_badServiceUrl);

            status = await service.GetServiceStatus();
            Assert.True(status == ServiceStatus.Unavailable);
        }

        [Fact]
        public async void GetIrrigationStatus()
        {
            bool status = await new ControllerService(_serviceUrl).GetIrrigationStatus();
            _output.WriteLine("Current irrigation status is {0}", status);
        }

        [Fact]
        public async void StartIrrigation()
        {
            ControllerService service = new ControllerService(_serviceUrl);

            await service.StartIrrigation();
            
            Assert.True(await service.GetIrrigationStatus());
        }

        [Fact]
        public async void StopIrrigation()
        {
            ControllerService service = new ControllerService(_serviceUrl);

            await service.StartIrrigation();

            bool status = await service.GetIrrigationStatus();
            _output.WriteLine("Current irrigation status after starting is {0}", status);
            Assert.True(status);

            int delaySeconds = 2;
            _output.WriteLine("Delaying {0} seconds because... hardware", delaySeconds);
            await Task.Delay(delaySeconds * 1000);

            await service.StopIrrigation();

            status = await service.GetIrrigationStatus();
            _output.WriteLine("Current irrigation status after stopping is {0}", status);
            Assert.False(status);
        }
    }
}
