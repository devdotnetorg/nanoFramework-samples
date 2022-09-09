using nanoFramework.Hosting;
using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Threading;

namespace Weatherstation.Services
{
    internal class ConnectionService : BackgroundService
    {
        protected override void ExecuteAsync()
        {
            //connecting to WiFi
            const string Ssid = "ap";
            const string Password = "*****";
            // Give 60 seconds to the wifi join to happen
            CancellationTokenSource cs = new(60000);
            bool flag = false;
            while (!flag)
            {
                var success = WifiNetworkHelper.ConnectDhcp(Ssid, Password, requiresDateTime: false, token: cs.Token);
                if (!success)
                {
                    // Something went wrong, you can get details with the ConnectionError property:
                    Debug.WriteLine($"Can't connect to the network, error: {WifiNetworkHelper.Status}");
                    if (WifiNetworkHelper.HelperException != null)
                        Debug.WriteLine($"ex: {WifiNetworkHelper.HelperException}");
                }
                else
                {
                    Debug.WriteLine($"success");
                    flag = true;
                }
            }
            //time synchronization           
            Sntp.Server1 = "0.fr.pool.ntp.org";
            Sntp.UpdateNow();
            Debug.WriteLine($"Now: {DateTime.UtcNow}");
        }
    }
}
