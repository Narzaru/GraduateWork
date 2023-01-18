using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace ArduinoSerial;

public class ArduinoDriver : IDisposable
{
    public byte[] AsBytesArray => m_bytes.ToArray();
    public string AsString => Encoding.ASCII.GetString(AsBytesArray);
    public bool IsConnected { get; private set; }
    public string PortName => m_serial.PortName;

    public ArduinoDriver(string comPort, int baudRate) : this()
    {
        OpenConnection(comPort, baudRate);
    }

    public ArduinoDriver()
    {
        IsConnected = false;
        m_serial = new SerialPort();
        m_serial.DataReceived += DataReceivedHandler;
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

    public void SendCommand(object obj)
    {
        m_serial.WriteTimeout = (int)TimeSpan.FromMilliseconds(1).TotalMilliseconds;
        if (obj is ArduinoCommands arduinoCommand)
        {
            m_serial.Write(arduinoCommand.ToString());
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

    public void ReadAnswer(int bytesToReceive, TimeSpan timeOut)
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