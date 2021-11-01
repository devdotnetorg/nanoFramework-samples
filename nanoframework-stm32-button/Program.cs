using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Gpio;

namespace nanoframework_stm32_button
{
    public class Program
    {
        private static int LED_PIN;
        private static int BUTTON_PIN;
        private static GpioPin ledPin;
        private static GpioPin buttonPin;        
        private static GpioPinValue ledPinValue = GpioPinValue.High;
        public static void Main()
        {
            /*
            Board: ST Nucleo64 F411RE
            source: Push button - Raspberry Pi
            https://docs.microsoft.com/en-us/samples/microsoft/windows-iotcore-samples/push-button/
            */

            // ST Nucleo64 F411RE: PA5 is LED_GREEN
            LED_PIN = PinNumber('A', 5);
            // ST Nucleo64 F411RE: PC13 is B1_USER
            BUTTON_PIN = PinNumber('C', 13);
            //Next as .NET for Raspberry Pi, Windows IoT Core
            //Controller
            var gpio = new GpioController();
            //Init
            ledPin = gpio.OpenPin(LED_PIN);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);
            buttonPin = gpio.OpenPin(BUTTON_PIN);
            buttonPin.SetDriveMode(GpioPinDriveMode.Input);
            // Initialize LED to the OFF state by first writing a HIGH value
            // We write HIGH because the LED is wired in a active LOW configuration
            ledPin.Write(ledPinValue);            
            // Check if input pull-up resistors are supported
            if (buttonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonPin.SetDriveMode(GpioPinDriveMode.Input);
            // Set a debounce timeout to filter out switch bounce noise from a button press
            buttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            // Register for the ValueChanged event so our buttonPin_ValueChanged 
            // function is called when the button is pressed
            buttonPin.ValueChanged += buttonPin_ValueChanged;
            //Infinite
            Thread.Sleep(Timeout.Infinite);
        }

        static void buttonPin_ValueChanged(object sender, GpioPinValueChangedEventArgs e)
        {
            Debug.WriteLine("USER BUTTON (event) : " + ((e.Edge ==GpioPinEdge.RisingEdge)? "RisingEdge" : "FallingEdge"));            
            // toggle the state of the LED every time the button is pressed
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                ledPinValue = (ledPinValue == GpioPinValue.Low) ?
                    GpioPinValue.High : GpioPinValue.Low;
                ledPin.Write(ledPinValue);
            }
        }
            
        static int PinNumber(char port, byte pin)
        {
            if (port < 'A' || port > 'J')
                throw new ArgumentException();
            return ((port - 'A') * 16) + pin;
        }
    }
}
