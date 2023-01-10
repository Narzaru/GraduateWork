using System;
using System.IO.Ports;
using System.Threading;

namespace ArduinoSerial
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            // try find arduino with my sketch
            var serial = new Arduino();
            if (serial.AutodetectArduinoPort(9600, TimeSpan.FromSeconds(2)))
            {
                Console.WriteLine($"detected on {serial.PortName}");
            } else
            {
                Console.WriteLine("doesnt detected");
                return -1;
            }

            // open arduino port
            var port = new SerialPort(serial.PortName, 9600);
            port.ReadTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            port.Open();

            port.DataReceived += (sender, e) =>
            {
                Console.WriteLine(port.ReadLine());
            };

            // do commands
            string command;
            do
            {
                command = Console.ReadLine()!;
                port.Write(command);
            } while (command != "exit");
            
            port.Close();
            return 0;
        }
    }
}