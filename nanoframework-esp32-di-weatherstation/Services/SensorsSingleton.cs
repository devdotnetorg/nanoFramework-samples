using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using System;
using System.Device.I2c;
using System.Diagnostics;
using Weatherstation.Interfaces;
using Weatherstation.Sensors;

namespace Weatherstation.Services
{
    internal class SensorsSingleton : ISensorsService, IDisposable
    {
        private const int busId = 1;
        private readonly object _syncLock = new object();
        private I2cDevice _i2cDevice;
        private Bme280 _bme280;
        private SensorsResult _sensorsResult;

        public SensorsSingleton()
        {
            this.InitBme280();
        }
        private void InitBme280()
        {
            Debug.WriteLine("Init Bme280!");
            I2cConnectionSettings i2cSettings = new(busId, Bme280.SecondaryI2cAddress);
            _i2cDevice = I2cDevice.Create(i2cSettings);
            _bme280 = new Bme280(_i2cDevice)
            {
                // set higher sampling
                TemperatureSampling = Sampling.LowPower,
                PressureSampling = Sampling.UltraHighResolution,
                HumiditySampling = Sampling.Standard,
            };
        }
        public void Dispose()
        {
            _bme280?.Dispose();
            _bme280 = null;

            _i2cDevice?.Dispose();
            _i2cDevice = null!;
        }
        private void Read()
        {
            Bme280ReadResult readResult = _bme280.Read();
            lock (_syncLock)
            {
                _sensorsResult = new SensorsResult(
                    Math.Truncate(readResult.Temperature.DegreesCelsius),
                    Math.Truncate(readResult.Pressure.Hectopascals),
                    Math.Truncate(readResult.Humidity.Percent));
            }

            Debug.WriteLine($"Temperature: {readResult.Temperature.DegreesCelsius}\u00B0C");
            Debug.WriteLine($"Pressure: {readResult.Pressure.Hectopascals}hPa");
            Debug.WriteLine($"Relative humidity: {readResult.Humidity.Percent}%");
        }

        public SensorsResult GetSensorsResult()
        {
            this.Read();
            lock (_syncLock)
            {
                return _sensorsResult;
            }
        }
    }
}
