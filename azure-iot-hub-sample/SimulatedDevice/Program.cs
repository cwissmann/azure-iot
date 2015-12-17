using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimulatedDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "[iothuburi]";
        static string deviceId = "[deviceid]";
        static string deviceKey = "[devicekey]";
        static string connectionString = "[connectionstring]";

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated Device ({0})\n", deviceId);
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString);

            SendMessageToCloudAsync();
            Console.ReadLine();
        }

        public static async void SendMessageToCloudAsync()
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = deviceId,
                    windSpeed = currentWindSpeed
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }
    }
}
