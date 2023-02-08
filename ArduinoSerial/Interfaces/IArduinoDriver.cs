using System;

namespace ArduinoSerial.Interfaces
{
    public delegate void ConnectionLost(string portName);

    public interface IArduinoDriver
    {
        public event ConnectionLost? OnConnectionLost;

        public void Send(object obj);
        public void Read(int bytesToReceive, TimeSpan timeOut);

        public byte[] Bytes { get; }
    }
}