using System;

namespace ArduinoSerial
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var arduino = new Arduino();
            if (arduino.AutodetectArduinoPort(115200, TimeSpan.FromSeconds(5)))
            {
                Console.WriteLine($"detected on {arduino.PortName}");
            }
            else
            {
                Console.WriteLine("doesnt detected");
                return -1;
            }

            return 0;
        }
    }
}