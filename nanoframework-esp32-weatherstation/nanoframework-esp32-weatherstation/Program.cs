using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.FilteringMode;
using Iot.Device.Common;
using Iot.Device.Ssd13xx;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;
using UnitsNet;

namespace nanoframework_esp32_weatherstation
{
    public class Program
    {
        public static void Main()
        {
            /* Board: ESP32 DevKit
            Connecting the BME280 sensor to the I2C bus
            Sample: https://github.com/nanoframework/nanoFramework.IoT.Device/blob/develop/devices/Bmxx80/README.md
            Docs: https://docs.nanoframework.net/devices/Iot.Device.Bmxx80.Bme280.html
            */
            Debug.WriteLine("Hello Bme280!");
            // when connecting to an ESP32 device, need to configure the I2C GPIOs
            // used for the bus
            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);
            // bus id on the MCU
            const int busId = 1;
            I2cConnectionSettings i2cSettings = new(busId, Bme280.SecondaryI2cAddress);
            
            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
            using Bme280 bme80 = new Bme280(i2cDevice)
            {
                // set higher sampling
                TemperatureSampling = Sampling.LowPower,
                PressureSampling = Sampling.UltraHighResolution,
                HumiditySampling = Sampling.Standard,
            };

            // set this to the current sea level pressure in the area for correct altitude readings
            Pressure defaultSeaLevelPressure = WeatherHelper.MeanSeaLevel;

            while (true)
            {
                // Perform a synchronous measurement
                var readResult = bme80.Read();

                // Note that if you already have the pressure value and the temperature, you could also calculate altitude by using
                // var altValue = WeatherHelper.CalculateAltitude(preValue, defaultSeaLevelPressure, tempValue) which would be more performant.
                bme80.TryReadAltitude(defaultSeaLevelPressure, out var altValue);

                Debug.WriteLine($"Temperature: {readResult.Temperature.DegreesCelsius}\u00B0C");
                Debug.WriteLine($"Pressure: {readResult.Pressure.Hectopascals}hPa");
                Debug.WriteLine($"Altitude: {altValue.Meters}m");
                Debug.WriteLine($"Relative humidity: {readResult.Humidity.Percent}%");

                // WeatherHelper supports more calculations, such as saturated vapor pressure, actual vapor pressure and absolute humidity.
                if (!readResult.Temperature.Equals(null) && !readResult.Humidity.Equals(null))
                {
                    Debug.WriteLine($"Heat index: {WeatherHelper.CalculateHeatIndex(readResult.Temperature, readResult.Humidity).DegreesCelsius}\u00B0C");
                    Debug.WriteLine($"Dew point: {WeatherHelper.CalculateDewPoint(readResult.Temperature, readResult.Humidity).DegreesCelsius}\u00B0C");
                }

                Debug.WriteLine($"---------------");
                Thread.Sleep(5000);
            }
            
            //Infinite
            //Thread.Sleep(Timeout.Infinite);
        }        
    }
}

