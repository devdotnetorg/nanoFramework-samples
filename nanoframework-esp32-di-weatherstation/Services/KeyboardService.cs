using Iot.Device.Mpr121;
using System;
using System.Device.I2c;
using System.Diagnostics;
using Weatherstation.Interfaces;

namespace Weatherstation.Services
{
    internal class KeyboardService : IKeyboardService, IDisposable
    {
        private const int busId = 1; // bus id on the MCU
        private I2cDevice _i2cDevice;
        private Mpr121 _mpr121;
        public KeyboardService()
        {
            this.InitMpr121();
        }
        private void InitMpr121()
        {
            Debug.WriteLine("Init InitMpr121!");
            I2cConnectionSettings i2cSettings = new(busId, Mpr121.DefaultI2cAddress);
            _i2cDevice = I2cDevice.Create(i2cSettings);
            _mpr121 = new Mpr121(_i2cDevice);
        }

        public void Dispose()
        {
            _mpr121?.Dispose();
            _mpr121 = null;

            _i2cDevice?.Dispose();
            _i2cDevice = null!;
        }

        public int ReadKey()
        {
            bool[] channelStatuses = _mpr121.ReadChannelStatuses();
            int key = -1;
            for (int i = 0; i < channelStatuses.Length; i++)
            {
                if (channelStatuses[i])
                {
                    key = i;
                    break;
                }
            }
            return key;
        }
    }
}
