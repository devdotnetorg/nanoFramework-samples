using nanoFramework.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using Weatherstation.Interfaces;
using Weatherstation.Sensors;

namespace Weatherstation.Services
{
    internal class MonitorService : IHostedService
    {
        private ISensorsService _sensorsService { get; }
        private IDisplayService _displayService { get; set; }
        private IKeyboardService _keyboardService { get; }
        private Thread _handlerThread;
        private CancellationTokenSource _cs;

        public MonitorService(ISensorsService sensorsService, IDisplayService displayService, IKeyboardService keyboardService)
        {
            _sensorsService = sensorsService;
            _displayService = displayService;
            _keyboardService = keyboardService;
        }
        public void Start()
        {
            Debug.WriteLine("MonitorService started");
            int key = -1;
            const int readingIntervalSensors = 50;
            int timeSensors = readingIntervalSensors;
            SensorsResult sensorsResult = new SensorsResult();
            Screen currentScreen = Screen.DateTime_1;
            //Thread
            _cs = new CancellationTokenSource();
            CancellationToken csToken = _cs.Token;
            _handlerThread = new Thread(() =>
            {
                while (!csToken.IsCancellationRequested)
                {
                    //sensors
                    if (timeSensors >= readingIntervalSensors)
                    {
                        sensorsResult = _sensorsService.GetSensorsResult();
                        timeSensors = 0;
                    }
                    timeSensors++;
                    //key
                    key = _keyboardService.ReadKey();
                    switch (key)
                    {
                        case 8:
                            currentScreen = Screen.DateTime_1;
                            break;
                        case 4:
                            currentScreen = Screen.TempHum_2;
                            break;
                        case 0:
                            currentScreen = Screen.Pressure_3;
                            break;
                        default:
                            // code block
                            break;
                    }
                    //screen
                    switch (currentScreen)
                    {
                        case Screen.DateTime_1:
                            DateTime currentDateTime = DateTime.UtcNow + TimeSpan.FromHours(3); // +3 GMT
                            _displayService.Show(Screen.DateTime_1, currentDateTime);
                            break;
                        case Screen.TempHum_2:
                            _displayService.Show(Screen.TempHum_2, sensorsResult);
                            break;
                        case Screen.Pressure_3:
                            _displayService.Show(Screen.Pressure_3, sensorsResult);
                            break;
                        default:
                            // code block
                            break;
                    }
                    Thread.Sleep(500);
                }
            });
            _handlerThread.Start();
        }

        public void Stop()
        {
            Debug.WriteLine("MonitorService stopped");
            _cs.Cancel();
            Thread.Sleep(2000);
            if (_handlerThread.ThreadState == ThreadState.Running) _handlerThread.Abort();
        }
    }
}
