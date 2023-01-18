using System;

namespace ArduinoSerial;

internal static class Program
{
    public static int Main(string[] args)
    {
        var arduino = new Arduino();
        arduino.AutodetectArduinoPort(115200, TimeSpan.FromSeconds(4), 10);

        arduino.Driver.SendCommand("M000000000");
        arduino.Driver.ReadAnswer(10, TimeSpan.FromSeconds(2));
        Console.WriteLine(arduino.Driver.AsString);
        return 0;
    }
}