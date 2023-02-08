using System;

namespace ArduinoSerial;

internal static class Program
{
    public static int Main(string[] args)
    {
        var arduino = new Arduino();
        arduino.AutodetectArduinoPort(115200, TimeSpan.FromSeconds(4), 17);

        var commandBuilder = new ArduinoCommandBuilder();

        arduino.Driver.Send("M000000000");
        arduino.Driver.Read(10, TimeSpan.FromSeconds(2));
        Console.WriteLine(arduino.Driver.AsString);
        return 0;
    }
}