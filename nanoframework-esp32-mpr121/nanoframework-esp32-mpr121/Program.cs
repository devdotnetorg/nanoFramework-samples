using Iot.Device.Mpr121;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;

namespace nanoframework_esp32_mpr121
{
    public class Program
    {
        public static void Main()
        {
            /* Board: ESP32 DevKit
            Connecting the  MPR121 Capacitive Touch Keypad Shield to the I2C bus
            Sample: https://github.com/nanoframework/nanoFramework.IoT.Device/blob/develop/devices/Mpr121/README.md
            Docs: https://docs.nanoframework.net/api/Iot.Device.Mpr121.Mpr121.html
            */
            Debug.WriteLine("Hello  MPR121!");
            // when connecting to an ESP32 device, need to configure the I2C GPIOs
            // used for the bus
            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);
            // bus id on the MCU            
            var i2cSettings = new I2cConnectionSettings(busId: 1, deviceAddress: Mpr121.DefaultI2cAddress);
            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
            // Initialize controller with default configuration and auto-refresh the channel statuses every 100 ms.
            var mpr121 = new Mpr121(i2cDevice: i2cDevice, periodRefresh: 100);            
            // Subscribe to channel statuses updates.
            mpr121.ChannelStatusesChanged += (object? sender, ChannelStatusesChangedEventArgs e) =>
            {
                var channelStatuses = e.ChannelStatuses;
                for (int i = 0; i < channelStatuses.Length; i++)
                {
                    Debug.WriteLine(channelStatuses[i] ? $"{i} #" : $"{i} ");
                }
            };
            //
            Debug.WriteLine($"---------------");
            //Infinite
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
