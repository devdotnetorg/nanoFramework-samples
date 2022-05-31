using Iot.Device.Ssd13xx;
using Iot.Device.Ssd13xx.Samples;
using nanoFramework.Hardware.Esp32;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;
using System.Device.Wifi;
using System;
using System.Device.Gpio;

namespace nanoframework_esp32_scan_wifi
{
    public class Program
    {
        private static Ssd1306 device;
        private static GpioPin led;
        private static int numberLines=6;

        public static void Main()
        {
            Debug.WriteLine("Scan WiFi!");
            //LED ========================
            var gpioController= new GpioController();
            led = gpioController.OpenPin(16, PinMode.Output); // ESP32 - 2, Wemos TTGO OLED Battery - 16
            led.Write(PinValue.High); //ESP32 - Low, Wemos TTGO OLED Battery - High
            //LCD ========================
            //configure the I2C GPIOs            
            Configuration.SetPinFunction(5, DeviceFunction.I2C1_DATA);  // ESP32 - 21, Wemos TTGO OLED Battery - 5
            Configuration.SetPinFunction(4, DeviceFunction.I2C1_CLOCK); // ESP32 - 22, Wemos TTGO OLED Battery - 4          
            //Init LCD
            device = new Ssd1306(I2cDevice.Create(new I2cConnectionSettings(1, Ssd1306.DefaultI2cAddress)), Ssd13xx.DisplayResolution.OLED128x64);            
            device.ClearScreen();
            device.Font = new BasicFont();
            device.DrawString(2, 2, "nF IOT!", 2);//large size 2 font
            device.DrawString(2, 32, "Scan WiFi", 1, true);//centered text
            device.Display();
            Thread.Sleep(1500);            
            //============================
            //start wifi
            try
            {
                // Get the first WiFI Adapter
                WifiAdapter wifi = WifiAdapter.FindAllAdapters()[0];                
                // Set up the AvailableNetworksChanged event to pick up when scan has completed
                wifi.AvailableNetworksChanged += Wifi_AvailableNetworksChanged;
                // Loop forever scanning every 15 seconds
                while (true)
                {
                    Debug.WriteLine("starting Wifi scan");
                    led.Toggle();
                    wifi.ScanAsync();
                    Thread.Sleep(15000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("message:" + ex.Message);
                Debug.WriteLine("stack:" + ex.StackTrace);
            }
            //Infinite
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Event handler for when Wifi scan completes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Wifi_AvailableNetworksChanged(WifiAdapter sender, object e)
        {
            led.Toggle();
            Debug.WriteLine("Wifi_AvailableNetworksChanged - get report");
            // Get Report of all scanned Wifi networks
            WifiNetworkReport report = sender.NetworkReport;
            //LCD
            device.ClearScreen();
            device.DrawString(2, 2, "WiFi networks:", 1);
            device.Display();
            var line = 0;
            // Enumerate though networks looking for our network            
            foreach (WifiAvailableNetwork net in report.AvailableNetworks)
            {
                // Show all networks found
                Debug.WriteLine($"Net SSID :{net.Ssid},  BSSID : {net.Bsid},  rssi : {net.NetworkRssiInDecibelMilliwatts},  signal : {net.SignalBars}");
                //LCD
                if (line <= numberLines)
                {
                    //show
                    device.DrawString(2, 12+(line*9), $"{net.Ssid} {net.NetworkRssiInDecibelMilliwatts} {net.SignalBars}", 1);
                }
                line++;
            }
            //LCD
            device.Display();
        }
    }
}
