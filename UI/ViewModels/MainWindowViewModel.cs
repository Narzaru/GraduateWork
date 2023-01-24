using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Data;
using DynamicData;
using ReactiveUI;
using UI.Models;
using UI.ViewModels.FixedPoints;

namespace UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Constructors

    public MainWindowViewModel()
    {
        // TODO(narzaru) remove state to a model class
        m_status = "waiting user input...";
        m_content = List = new FixedPointsViewModel(this);
        SelectedComPort = string.Empty;
        IsConnectingInProgress = false;
        BoundRate = "115200";
        TimeOut = "4";
        PacketSize = "10";
        UpdateComPortsCommand = ReactiveCommand.Create(UpdatePorts);
        ConnectArduinoCommand = ReactiveCommand.Create(ConnectArduino);
        LoadFixedPointsCommand = ReactiveCommand.Create(LoadFixedPoints);
        NewFixedPointsCommand = ReactiveCommand.Create(NewFixedPoints);
        GoToZeroCommand = ReactiveCommand.Create(GoToZero);
        MoveToFixedCommand = ReactiveCommand.Create(MoveToFixed);
    }

    #endregion

    #region BoundRateTextBox

    public string BoundRate { get; set; }

    #endregion

    #region TimeOutTextBox

    public string TimeOut { get; set; }

    #endregion

    #region PacketSizeTextBox

    public string PacketSize { get; set; }

    #endregion

    #region ComPortsCombatBox

    public List<string> ComPorts
    {
        get => m_listOfComPorts;
        set => this.RaiseAndSetIfChanged(ref m_listOfComPorts, value);
    }

    private List<string> m_listOfComPorts = new();

    public string SelectedComPort { get; set; }

    #endregion

    #region RefreshButton

    public ReactiveCommand<Unit, Unit> UpdateComPortsCommand { get; }

    public void UpdatePorts()
    {
        ComPorts = SerialPort.GetPortNames().ToList();
    }

    #endregion

    #region ConnectButton

    public ReactiveCommand<Unit, Unit> ConnectArduinoCommand { get; }

    private void ConnectArduino()
    {
        var thread = new Thread(() =>
        {
            // TODO(narzaru) remove commands to another class
            Commands.Insert(0, $"{DateTime.Now} connecting to {SelectedComPort}");
            Status = "connection in progress...";
            IsConnectingInProgress = true;
            m_arduinoModel.Connect(
                SelectedComPort,
                int.Parse(BoundRate),
                TimeSpan.FromSeconds(int.Parse(TimeOut)),
                int.Parse(PacketSize)
            );
            Status = "waiting user input...";
            Commands.Insert(0,
                m_arduinoModel.IsConnected
                    ? $"{DateTime.Now} connected to {SelectedComPort}"
                    : $"{DateTime.Now} connection error, validate your input");

            IsConnectingInProgress = false;
        });
        thread.Start();
    }

    #endregion

    #region GoToZeroCommand

    public ReactiveCommand<Unit, Unit> GoToZeroCommand { get; set; }

    public void GoToZero()
    {
        // TODO(narzaru)another state...
        if (m_arduinoModel.IsConnected)
        {
            IsConnectingInProgress = true;
            new Thread(_ =>
            {
                Commands.Insert(0, "move to zero command received");
                Status = "move to zero...";
                m_arduinoModel.GoToZero();
                Status = "waiting user input...";
                Commands.Insert(0, "move to zero completed");
                IsConnectingInProgress = false;
            }).Start();
        }
    }

    #endregion

    #region MoveToFixedCommand

    public ReactiveCommand<Unit, Unit> MoveToFixedCommand { get; set; }

    public void MoveToFixed()
    {
        // TODO(narzaru)another state...
        if (m_arduinoModel.IsConnected)
        {
            IsConnectingInProgress = true;
            new Thread(_ =>
            {
                Commands.Insert(0, "move to fixed points command received");
                Status = "move to fixed points...";
                m_arduinoModel.GoToFixed();
                Status = "waiting user input...";
                Commands.Insert(0, "move to fixed points completed");
                IsConnectingInProgress = false;
            }).Start();
        }
    }

    #endregion

    #region ListOfArduinoCommands

    public ObservableCollection<string> Commands { get; set; } = new();

    #endregion

    // TODO(Narzaru) move connection state to ArduinoModel class

    #region ConnectionState

    public bool IsConnectingInProgress
    {
        get => m_isConnectingInProgress;
        private set => this.RaiseAndSetIfChanged(ref m_isConnectingInProgress, value);
    }

    private bool m_isConnectingInProgress;

    #endregion

    #region FixedPointsView

    public ViewModelBase Content
    {
        get => m_content;
        set => this.RaiseAndSetIfChanged(ref m_content, value);
    }

    public FixedPointsViewModel List { get; }

    private ViewModelBase m_content;

    public ReactiveCommand<Unit, Unit> LoadFixedPointsCommand { get; set; }

    public void LoadFixedPoints()
    {
        var vm = new LoadFixedPointsViewModel();

        Observable.Merge(vm.OkCommand, vm.CancelCommand)
            .Subscribe(pointsSet =>
            {
                if (pointsSet is not null)
                {
                    List.Points.Clear();
                    List.Points.AddRange(pointsSet.Points);
                }

                Content = List;
            });

        Content = vm;
    }

    public ReactiveCommand<Unit, Unit> NewFixedPointsCommand { get; set; }

    public void NewFixedPoints()
    {
        var vm = new NewFixedPointsViewModel();

        Observable.Merge(vm.CreateCommand, vm.CancelCommand)
            .Subscribe(model =>
            {
                if (model != null)
                {
                    using var dbContext = new FixedPointsContext();
                    dbContext.PointsSet!.Add(model);
                    dbContext.Points!.AddRange(model.Points);
                    dbContext.SaveChangesAsync();
                    List.Points.Clear();
                    List.Points.AddRange(model.Points);
                }

                Content = List;
            });

        Content = vm;
    }

    #endregion // FixedPointMenu

    #region StatusTextBox

    public string Status
    {
        get => m_status;
        set => this.RaiseAndSetIfChanged(ref m_status, value);
    }

    private string m_status;

    #endregion

    #region Models

    private readonly ArduinoModel m_arduinoModel = new ArduinoModel();

    #endregion
}