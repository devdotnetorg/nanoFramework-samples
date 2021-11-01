using System;
using System.Diagnostics;
using System.Threading;
using System.Device.Gpio;

namespace nanoframework_esp32_button
{   
    public class Program
    {
        //Board: ESP32 DevKit
        static GpioController s_GpioController;
        static int s_BluePinNumber=2;        
        static int s_UserButtonPinNumber=25;
        public static void Main()
        {
            s_GpioController = new GpioController();
            //setup blue LED
            s_GpioController.OpenPin(s_BluePinNumber, PinMode.Output);
            s_GpioController.Write(s_BluePinNumber, PinValue.Low);
            //setup user button            
            s_GpioController.OpenPin(s_UserButtonPinNumber, PinMode.Input);
            //s_GpioController.OpenPin(s_UserButtonPinNumber, PinMode.InputPullUp);
            //Event registration
            s_GpioController.RegisterCallbackForPinValueChangedEvent(
                s_UserButtonPinNumber,
                PinEventTypes.Falling | PinEventTypes.Rising,
                UserButton_ValueChanged);            
            //Infinite
            Thread.Sleep(Timeout.Infinite);            
        }

        private static void UserButton_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            // read Gpio pin value from event
            Debug.WriteLine("USER BUTTON (event) : " + e.ChangeType.ToString());
            Debug.WriteLine("USER BUTTON (event) : " + ((((int)e.ChangeType) == 1) ? "Rising" : "Falling"));

            //if (e.ChangeType != PinEventTypes.Rising) //for DFRobot
            if (e.ChangeType == PinEventTypes.Rising)
            {
                s_GpioController.Write(s_BluePinNumber, PinValue.High);
            }
            else
            {
                s_GpioController.Write(s_BluePinNumber, PinValue.Low);
            }
        }

    }
}
