using System;
using System.Collections.ObjectModel;
using System.Threading;
using ArduinoSerial.Interfaces;

namespace UI.Models;

public class Logs : ObservableCollection<string>, ICommandLogger 
{
    public Logs(int maxSize)
    {
        m_sizeLimit = maxSize;
        m_mutex = new Mutex();
    }

    public void AddToHistory(string command)
    {
        m_mutex.WaitOne();
        if (string.IsNullOrEmpty(command)) throw new ArgumentNullException(nameof(command));

        if (Count > m_sizeLimit) RemoveAt(Count - 1);

        Insert(0, WrapString(command));

        m_mutex.ReleaseMutex();
    }

    public void NewLog(string message)
    {
        AddToHistory(message);
    }

    private string WrapString(string command) => $"{DateTime.Now} {command}";

    private int m_sizeLimit;
    private Mutex m_mutex;
}