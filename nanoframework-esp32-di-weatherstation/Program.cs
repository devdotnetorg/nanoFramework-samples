using nanoFramework.Hosting;
using nanoFramework.DependencyInjection;
using Weatherstation.Interfaces;
using Weatherstation.Services;
using nanoFramework.Hardware.Esp32;

namespace Weatherstation.Hosting
{
    public class Program
    {
        public static void Main()
        {
            //////////////////////////////////////////////////////////////////////
            // when connecting to an ESP32 device, need to configure the I2C GPIOs
            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);
            //////////////////////////////////////////////////////////////////////            
            IHost host = CreateHostBuilder().Build();
            // starts application and blocks the main calling thread 
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    //Receiving data from sensors
                    services.AddSingleton(typeof(ISensorsService), typeof(SensorsSingleton));
                    //Data output to the display
                    services.AddSingleton(typeof(IDisplayService), typeof(DisplaySingleton));
                    //Keyboard
                    services.AddSingleton(typeof(IKeyboardService), typeof(KeyboardSingleton));
                    //MonitorService
                    services.AddHostedService(typeof(MonitorService));
                    //Connecting to WiFi and time synchronization
                    services.AddHostedService(typeof(ConnectionService));
                });
    }
}
