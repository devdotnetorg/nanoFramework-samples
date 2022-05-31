using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace Init_SSD1306_on_I2C_SPI
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Init SSD1306 on I2C/SPI");
            var pinOut = 18;
            //Init
            var s_GpioController = new GpioController();
            GpioPin rstPin = s_GpioController.OpenPin(pinOut, PinMode.Output);
            rstPin.Write(PinValue.High);
            Thread.Sleep(1); // VDD goes high at start, pause for 1 ms
            rstPin.Write(PinValue.Low); // Bring reset low
            Thread.Sleep(10); // Wait 10 ms
            rstPin.Write(PinValue.High); // Bring out of reset
            //end
            Console.WriteLine("Init end");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
