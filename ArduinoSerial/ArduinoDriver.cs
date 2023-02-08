using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;
using ArduinoSerial.Interfaces;

namespace ArduinoSerial;

public class ArduinoDriver : IArduinoDriver, IDisposable
{
    public byte[] Bytes => m_bytes.ToArray();
    public List<byte> AsByteList => m_bytes;
    public string AsString => Encoding.ASCII.GetString(m_bytes.ToArray());

    public bool IsConnected { get; private set; }
    public string PortName => m_serial.PortName;

    public event ConnectionLost? OnConnectionLost;

    public ArduinoDriver(string comPort, int baudRate) : this()
    {
        OpenConnection(comPort, baudRate);
    }

    public ArduinoDriver()
    {
        IsConnected = false;
        m_serial = new SerialPort();
        m_serial.DataReceived += DataReceivedHandler;
        new Thread(_ => CheckConnection(TimeSpan.FromSeconds(0.5))).Start();
    }

    public bool OpenConnection(string comPort, int baudRate)
    {
        CloseConnection();
        m_serial.PortName = comPort;
        m_serial.BaudRate = baudRate;

        if (!m_serial.IsOpen)
        {
            m_serial.Open();
            IsConnected = true;
        }

        return IsConnected;
    }

    public void Send(object obj)
    {
        if (obj is BaseArduinoCommand arduinoCommand)
        {
            m_serial.Write(arduinoCommand.Bytes, 0, arduinoCommand.BytesCount);
        }
        else if (obj is string command)
        {
            m_serial.Write(command);
        }
        else if (obj is byte[] bytes)
        {
            m_serial.Write(bytes, 0, bytes.Length);
        }
    }

    public void Read(int bytesToReceive, TimeSpan timeOut)
    {
        m_isReadingComplete = false;
        m_bytes.Clear();

        var timer = Stopwatch.StartNew();
        while (!m_isReadingComplete)
        {
            if (timer.ElapsedMilliseconds > timeOut.TotalMilliseconds || m_bytes.Count == bytesToReceive)
            {
                timer.Stop();
                m_isReadingComplete = true;
            }
        }
    }

    public void CloseConnection()
    {
        IsConnected = false;
        if (m_serial.IsOpen)
        {
            m_serial.Close();
        }
    }

    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        if (sender is SerialPort serialPort)
        {
            int bytesToRead = serialPort.BytesToRead;
            byte[] bytes = new byte[bytesToRead];
            serialPort.Read(bytes, 0, bytesToRead);
            foreach (var b in bytes)
            {
                m_bytes.Add(b);
            }
        }
    }

    private void CheckConnection(TimeSpan period)
    {
        while (!m_disposed)
        {
            Thread.Sleep(period);
            if (IsConnected)
            {
                if (!m_serial.IsOpen)
                {
                    IsConnected = false;
                    CloseConnection();
                    OnConnectionLost?.Invoke(m_serial.PortName);
                }
            }
        }
    }

    private readonly SerialPort m_serial;
    private bool m_isReadingComplete;
    private List<byte> m_bytes = new();

    #region dispose_reggion

    private bool m_disposed = false;

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!m_disposed)
        {
            if (disposing)
            {
                CloseConnection();
            }

            m_disposed = true;
        }
    }

    #endregion
}