using System;

namespace ArduinoSerial.Interfaces
{
    public interface IArduinoDriver : IDisposable
    {
        public delegate void Connection(string portName);

        public delegate void MessageReceived(byte[] answer);

        public delegate void MessageSent();

        public event Connection? OnConnectionLost;
        public event Connection? OnConnection;
        public event MessageReceived? OnMessageReceived;
        public event MessageSent? OnMessageSent;

        public string ConnectionPort { get; }
        public bool IsConnected { get; }
        public bool IsInProgress { get; }
        public byte[] BytesRead { get; }

        public void CloseConnection();
        public void OpenConnection(string portName, int baudRate);

        public void Send(object obj);
        public void Read(int bytesToReceive, TimeSpan timeOut);

        public void ReadInBlocking(int bytesToReceive, TimeSpan timeOut);
        public void SendInBlocking(object obj);
    }
}