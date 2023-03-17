using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Threading;
using Data;
using DynamicData;
using ReactiveUI;
using UI.Models;
using UI.ViewModels.FixedPoints;

namespace UI.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    #region Constructors

    public MainWindowViewModel()
    {
        m_status = new CurrentStatus();
        m_statusColor = new ImageBrush();
        Status.PropertyChanged += (sender, args) => this.RaisePropertyChanged(nameof(Status));
        Status.Waiting();
        m_arduinoModel.Arduino.Driver.OnConnection +=
            name => Dispatcher.UIThread.Post(() => IsConnected = true);
        m_arduinoModel.Arduino.Driver.OnConnectionLost +=
            name => Dispatcher.UIThread.Post(() => IsConnected = false);
        StatusColor = Status.Level.ToBrush();
        m_content = List = new FixedPointsViewModel(this);
        SelectedComPort = string.Empty;

        IsExecuting = false;
        IsFixedPointsProcessing = false;
        IsConnected = false;

        BoundRate = "115200";
        TimeOut = "2";
        PacketSize = "17";

        LoadFixedPointsCommand = ReactiveCommand.CreateFromTask(_ => Task.Run(LoadFixedPoints));
        NewFixedPointsCommand = ReactiveCommand.CreateFromTask(_ => Task.Run(NewFixedPoints));

        var canExecuteArduinoCommand = this.WhenAnyValue(
            x => x.IsConnected,
            x => x.IsExecuting,
            x => x.IsFixedPointsProcessing,
            (isConnected, isExecuting, isPointsProcessing) => IsConnected && !isExecuting && !isPointsProcessing
        );

        UpdateComPortsCommand = ReactiveCommand.Create(UpdatePorts);

        ConnectArduinoCommand = ReactiveCommand.CreateFromTask(_ => Task.Run(ConnectArduino));
        ConnectArduinoCommand.IsExecuting.Subscribe(value => IsExecuting = value);
        GoToZeroCommand = ReactiveCommand.CreateFromTask(_ => Task.Run(GoToZero), canExecuteArduinoCommand);
        GoToZeroCommand.IsExecuting.Subscribe(value => IsExecuting = value);
        MoveToFixedCommand = ReactiveCommand.CreateFromTask(_ => Task.Run(MoveToFixed), canExecuteArduinoCommand);
        MoveToFixedCommand.IsExecuting.Subscribe(value => IsExecuting = value);
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
        Logs.AddToHistory($"connecting to {SelectedComPort}");
        Status.Connecting();
        StatusColor = Status.Level.ToBrush();
        m_arduinoModel.Connect(
            SelectedComPort,
            int.Parse(BoundRate),
            TimeSpan.FromSeconds(float.Parse(TimeOut)),
            int.Parse(PacketSize)
        );
        Status.Waiting();
        StatusColor = Status.Level.ToBrush();
        Logs.AddToHistory(
            m_arduinoModel.IsConnected
                ? $"connected to {SelectedComPort}"
                : $"connection error, validate your input");
    }

    #endregion

    #region GoToZeroCommand

    public ReactiveCommand<Unit, Unit> GoToZeroCommand { get; set; }

    public void GoToZero()
    {
        Logs.AddToHistory("move to zero command received");
        Status.MoveTowardsZero();
        StatusColor = Status.Level.ToBrush();
        var isSuccess = m_arduinoModel.GoToZero();
        Status.Waiting();
        StatusColor = Status.Level.ToBrush();
        Logs.AddToHistory(isSuccess ? "move to zero completed" : "move to zero error");
    }

    #endregion

    #region MoveToFixedCommand

    public ReactiveCommand<Unit, Unit> MoveToFixedCommand { get; set; }

    public void MoveToFixed()
    {
        if (List.Points.Count != 2)
        {
            Logs.AddToHistory("Incorrect fixed points");
            return;
        }

        Logs.AddToHistory("move to fixed points command received");
        Status.MovingTowards();
        StatusColor = Status.Level.ToBrush();
        var isSuccess = m_arduinoModel.GoToFixed(
            new Vector2(List.Points[0].PositionX, List.Points[0].PositionY),
            new Vector2(List.Points[1].PositionX, List.Points[1].PositionY));
        Status.Waiting();
        StatusColor = Status.Level.ToBrush();
        Logs.AddToHistory(isSuccess ? "move to fixed points completed" : "move to fixed points error");
    }

    #endregion

    #region Logs

    public Logs Logs { get; } = new(100);

    #endregion

    #region ConnectionState

    public bool IsExecuting
    {
        get => m_isExecuting;
        private set => this.RaiseAndSetIfChanged(ref m_isExecuting, value);
    }

    private bool m_isExecuting;

    public bool IsConnected
    {
        get => m_isConnected;
        private set => this.RaiseAndSetIfChanged(ref m_isConnected, value);
    }

    private bool m_isConnected;

    public bool IsFixedPointsProcessing
    {
        get => m_isFixedPointsProcessing;
        private set => this.RaiseAndSetIfChanged(ref m_isFixedPointsProcessing, value);
    }

    private bool m_isFixedPointsProcessing;

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