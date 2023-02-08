using System;

namespace ArduinoSerial;

public class BaseArduinoCommand
{
    public BaseArduinoCommand(int packetSize)
    {
        PacketSize = packetSize;
        BytesArray = new Byte[PacketSize];
    }

    public Byte[] Bytes => BytesArray;
    public int BytesCount => PacketSize;

    protected Byte[] BytesArray;
    protected readonly int PacketSize;
}

