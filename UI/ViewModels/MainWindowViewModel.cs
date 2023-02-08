using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
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
        m_status = new();
        m_statusColor = new ImageBrush();
        Status.PropertyChanged += (sender, args) => this.RaisePropertyChanged(nameof(Status));
        Status.Waiting();
        StatusColor = Status.Level.ToBrush();
        m_content = List = new FixedPointsViewModel(this);
        SelectedComPort = string.Empty;
        IsConnectingInProgress = false;
        BoundRate = "115200";
        TimeOut = "2";
        PacketSize = "17";
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
            Logs.AddToHistory($"connecting to {SelectedComPort}");
            Status.Connecting();
            StatusColor = Status.Level.ToBrush();
            IsConnectingInProgress = true;
            m_arduinoModel.Connect(
                SelectedComPort,
                int.Parse(BoundRate),
                TimeSpan.FromSeconds(int.Parse(TimeOut)),
                int.Parse(PacketSize)
            );
            Status.Waiting();
            StatusColor = Status.Level.ToBrush();
            Logs.AddToHistory(
                m_arduinoModel.IsConnected
                    ? $"connected to {SelectedComPort}"
                    : $"connection error, validate your input");

            IsConnectingInProgress = false;
        });
        thread.Start();
    }

    #endregion

    #region GoToZeroCommand

    public ReactiveCommand<Unit, Unit> GoToZeroCommand { get; set; }

    public void GoToZero()
    {
        // TODO(narzaru) remove another state...
        if (!m_arduinoModel.IsConnected) return;

        IsConnectingInProgress = true;
        new Thread(_ =>
        {
            Logs.AddToHistory("move to zero command received");
            Status.MoveTowardsZero();
            StatusColor = Status.Level.ToBrush();
            m_arduinoModel.GoToZero();
            Status.Waiting();
            StatusColor = Status.Level.ToBrush();
            Logs.AddToHistory("move to zero completed");
            IsConnectingInProgress = false;
        }).Start();
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
                if (List.Points.Count != 2)
                {
                    Logs.AddToHistory("Incorrect fixed points");
                    IsConnectingInProgress = false;
                    return;
                }

                Logs.AddToHistory("move to fixed points command received");
                Status.MovingTowards();
                StatusColor = Status.Level.ToBrush();
                m_arduinoModel.GoToFixed(
                    new Vector2(List.Points[0].PositionX, List.Points[0].PositionY),
                    new Vector2(List.Points[1].PositionX, List.Points[1].PositionY));
                Status.Waiting();
                StatusColor = Status.Level.ToBrush();
                Logs.AddToHistory("move to fixed points completed");
                IsConnectingInProgress = false;
            }).Start();
        }
    }

    #endregion

    #region Logs

    public Logs Logs { get; } = new(100);

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

    public CurrentStatus Status
    {
        get => m_status;
        set => m_status = value;
    }

    public IBrush StatusColor
    {
        get => m_statusColor;
        set => this.RaiseAndSetIfChanged(ref m_statusColor, value);
    }

    private CurrentStatus m_status;

    private IBrush m_statusColor;

    #endregion

    #region Models

    private readonly ArduinoModel m_arduinoModel = new ArduinoModel();

    #endregion
}