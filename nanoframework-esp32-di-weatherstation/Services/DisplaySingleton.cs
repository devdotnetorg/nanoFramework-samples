using System;
using System.Device.I2c;
using Iot.Device.Ssd13xx;
using Iot.Device.Ssd13xx.Samples;
using System.Diagnostics;
using System.Threading;
using Weatherstation.Interfaces;
using System.Globalization;
using Weatherstation.Sensors;

namespace Weatherstation.Services
{
    internal class DisplaySingleton : IDisplayService, IDisposable
    {
        private const int busId = 1;
        private I2cDevice _i2cDevice;
        private Ssd1306 _device;
        public DisplaySingleton()
        {
            this.InitSsd1306();
        }
        private void InitSsd1306()
        {
            Debug.WriteLine("Init Ssd1306 Sample!");
            //LCD with reset. Pin number 18
            I2cConnectionSettings i2cSettings = new(busId, Ssd1306.SecondaryI2cAddress);
            _i2cDevice = I2cDevice.Create(i2cSettings);
            _device = new Ssd1306(_i2cDevice, Ssd13xx.DisplayResolution.OLED128x64, 18);
            _device.ClearScreen();
            _device.Font = new BasicFont();
            _device.DrawString(2, 2, "nF IOT!", 2);//large size 2 font
            _device.DrawString(2, 32, "nanoFramework", 1, true);//centered text
            _device.Display();
            Thread.Sleep(1000);
        }

        public void Dispose()
        {
            _device?.Dispose();
            _device = null;

            _i2cDevice?.Dispose();
            _i2cDevice = null!;
        }

        public void Show(Screen screen, object obj = null)
        {
            switch (screen)
            {
                case Screen.DateTime_1:
                    DateTime currentDateTime = (DateTime)obj;
                    string line1 = $"{currentDateTime.Day}, {DateTimeFormatInfo.CurrentInfo.DayNames[(int)currentDateTime.DayOfWeek]}";
                    string line2 = currentDateTime.ToString("HH:mm");
                    string line3 = $".{currentDateTime.Second.ToString("D2")}";
                    _device.ClearScreen();
                    _device.DrawString(2, 2, line1, 1);
                    _device.DrawString(2, 32, line2, 2);
                    _device.DrawString(82, 38, line3, 1);
                    _device.Display();
                    break;
                case Screen.TempHum_2:
                    SensorsResult sensorsResult = (SensorsResult)obj;
                    line1 = $"+{sensorsResult.Temperature}";
                    line2 = $"Hum:{sensorsResult.Humidity}%";
                    _device.ClearScreen();
                    _device.DrawString(2, 18, line1, 3, true);
                    _device.DrawString(2, 44, line2, 2, true);
                    _device.Display();
                    break;
                case Screen.Pressure_3:
                    sensorsResult = (SensorsResult)obj;
                    line1 = "Pressure:";
                    line2 = $"{sensorsResult.Pressure}hPa";
                    _device.ClearScreen();
                    _device.DrawString(2, 4, line1, 1, true);
                    _device.DrawString(2, 22, line2, 2, true);
                    _device.Display();
                    break;
                default: //Clear_0
                    _device.ClearScreen();
                    break;
            }
        }
    }
}
