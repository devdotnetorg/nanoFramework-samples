using Weatherstation.Sensors;

namespace Weatherstation.Interfaces
{
    public interface IDisplayService
    {
        public void Show(Screen screen, object obj);
    }
    public enum Screen
    {
        Clear_0,
        DateTime_1,
        TempHum_2,
        Pressure_3,
    }
}
