using System;
using System.Collections.Generic;
using System.Linq;

namespace ArduinoSerial.Command;

public class BaseArduinoCommand
{
    public BaseArduinoCommand()
    {
        ByteList = new List<byte>();
    }

    public byte[] Bytes
    {
        get => ByteList.ToArray();
        set => ByteList = value.ToList();
    }
    

    public int BytesCount => ByteList.Count;

    protected List<byte> ByteList;
}

