using System;
using System.Diagnostics;
using System.Threading;
using System.Device.Gpio;

namespace nanoframework_esp32_blink
{
    public class Program
    {
        private static GpioController s_GpioController;
        public static void Main()
        {
            s_GpioController = new GpioController();
            // ESP32 DevKit: 4 is a valid GPIO pin in, some boards like Xiuxin ESP32 may require GPIO Pin 2 instead.
            GpioPin led = s_GpioController.OpenPin(2,PinMode.Output);
            led.Write(PinValue.Low);
            while (true)
            {
                led.Write(PinValue.High);
                Thread.Sleep(1000);
                led.Write(PinValue.Low);
                Thread.Sleep(1000);
                /*
                //Ñan be used: led.Toggle();
                led.Toggle();
                Thread.Sleep(125);
                led.Toggle();
                Thread.Sleep(125);
                led.Toggle();
                Thread.Sleep(125);
                led.Toggle();
                Thread.Sleep(525);
                */
            }
        }        
    }
}
