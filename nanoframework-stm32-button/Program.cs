using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Gpio;

namespace nanoframework_stm32_button
{
    public class Program
    {
        private static GpioController s_GpioController;
        public static void Main()
        {
            s_GpioController = new GpioController();

            // STM32F091RC: PA5 is LED_GREEN            
            GpioPin led = s_GpioController.OpenPin(
            PinNumber('A', 5));            
            led.SetDriveMode(GpioPinDriveMode.Output);
            led.Write(GpioPinValue.Low);


            // Interrupt pin for RX message & TX done notification 
            var InterruptGpioPin = s_GpioController.OpenPin(
            PinNumber('C', 13));
            InterruptGpioPin.SetDriveMode(GpioPinDriveMode.Input);

            // InterruptGpioPin_ValueChanged
            InterruptGpioPin.ValueChanged += UserButton_ValueChanged;

            

            Debug.WriteLine("Hello!");

            while (true)
            {
                led.Toggle();
                Thread.Sleep(125);
                led.Toggle();
                Thread.Sleep(125);
                led.Toggle();
                Thread.Sleep(125);
                led.Toggle();
                Thread.Sleep(525);
            }

        }
        private static void UserButton_ValueChanged(object sender, GpioPinValueChangedEventArgs e)
        {
            Debug.WriteLine(e.Edge.ToString());
            
        }
        static int PinNumber(char port, byte pin)
        {
            if (port < 'A' || port > 'J')
                throw new ArgumentException();

            return ((port - 'A') * 16) + pin;
        }
    }
}
