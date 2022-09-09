using System;
using System.Collections.Generic;
using System.Text;

namespace Weatherstation.Sensors
{
    public class SensorsResult
    {
        public SensorsResult() { }
        public SensorsResult(double temperature, double pressure, double humidity)
        {
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
        }
        public double Temperature { get; }
        public double Pressure { get; }
        public double Humidity { get; }
    }
}
