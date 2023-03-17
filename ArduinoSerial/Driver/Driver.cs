using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using ArduinoSerial.Command;
using ArduinoSerial.Interfaces;

namespace ArduinoSerial.Driver;

public class Driver : IArduinoDriver
{
    #region IArduinoDriver

    #region Properties

    public bool IsConnected { get; private set; }
    public string ConnectionPort => m_serialPort.PortName;
    public byte[] BytesRead => m_bytesRead.ToArray();
    public bool IsInProgress => m_inProgress;

    #endregion

    #region Events

    public event IArduinoDriver.Connection? OnConnectionLost;
    public event IArduinoDriver.Connection? OnConnection;
    public event IArduinoDriver.MessageReceived? OnMessageReceived;
    public event IArduinoDriver.MessageSent? OnMessageSent;

    #endregion

    #region Methods

    public void OpenConnection(string comPort, int baudRate)
    {
        CloseConnection();

        m_serialPort.PortName = comPort;
        m_serialPort.BaudRate = baudRate;

        try
        {
            m_serialPort.Open();
            IsConnected = true;
            OnConnection?.Invoke(ConnectionPort);
        }
        catch
        {
            CloseConnection();
        }
    }

    public void CloseConnection()
    {
        if (m_serialPort.IsOpen)
        {
            m_serialPort.Close();
        }

        IsConnected = false;
    }

    public void Send(object obj)
    {
        new Thread(_ =>
        {
            m_inProgress = true;
            SendInBlocking(obj);
            OnMessageSent?.Invoke();
            m_inProgress = false;
        }).Start();
    }

    public void Read(int bytesToReceive, TimeSpan timeOut)
    {
        new Thread(_ =>
        {
            m_inProgress = true;
            ReadInBlocking(bytesToReceive, timeOut);
            OnMessageReceived?.Invoke(BytesRead);
            m_inProgress = false;
        }).Start();
    }

    public void ReadInBlocking(int bytesToReceive, TimeSpan timeOut)
    {
        if (m_inProgress) throw new Exception("Reading currently in progress");
        m_timer.Restart();
        while (m_timer.ElapsedMilliseconds <= timeOut.TotalMilliseconds && m_buffer.Count < bytesToReceive)
        {
        }

        m_timer.Stop();
        m_bytesRead.Clear();
        m_bytesRead.AddRange(m_buffer);
        m_buffer.Clear();
    }

    public void SendInBlocking(object obj)
    {
        if (m_inProgress) throw new Exception("Send currently in progress");
        if (obj is BaseArduinoCommand arduinoCommand)
        {
            m_serialPort.Write(arduinoCommand.Bytes, 0, arduinoCommand.BytesCount);
        }
        else if (obj is string command)
        {
            m_serialPort.Write(command);
        }
        else if (obj is byte[] bytes)
        {
            m_serialPort.Write(bytes, 0, bytes.Length);
        }
        else throw new Exception("Attempt to send an unknown object");
    }

    #endregion

    #endregion

    #region Driver

    public Driver(string comPort, int baudRate) : this()
    {
        OpenConnection(comPort, baudRate);
    }

    public Driver()
    {
        IsConnected = false;
        m_inProgress = false;
        m_serialPort = new SerialPort();
        m_serialPort.DataReceived += DataReceivedHandler;
        m_timer = new Stopwatch();
        new Thread(_ => CheckConnection(TimeSpan.FromSeconds(0.5))).Start();
    }

    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        if (sender is not SerialPort serialPort)
            return;

        m_timer.Restart();
        int bytesToRead = serialPort.BytesToRead;
        byte[] bytes = new byte[bytesToRead];
        serialPort.Read(bytes, 0, bytesToRead);
        m_buffer.AddRange(bytes);
    }

    private void CheckConnection(TimeSpan sleepTime)
    {
        while (!m_disposed)
        {
            if (IsConnected)
            {
                if (!m_serialPort.IsOpen)
                {
                    CloseConnection();
                    OnConnectionLost?.Invoke(m_serialPort.PortName);
                }
            }

            Thread.Sleep(sleepTime);
        }

        OnConnectionLost?.Invoke(m_serialPort.PortName);
    }

    #region Data

    private Stopwatch m_timer;
    private readonly SerialPort m_serialPort;
    private bool m_inProgress;
    private List<byte> m_bytesRead = new();
    private List<byte> m_buffer = new();

    #endregion

    #endregion

    #region Dispose

    private bool m_disposed = false;

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (m_disposed) return;

        if (disposing)
        {
        }


        CloseConnection();
        m_serialPort.Dispose();

        m_disposed = true;
    }

    ~Driver()
    {
        Dispose(false);
    }

    #endregion
}