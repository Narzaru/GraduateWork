using Avalonia.Controls;
using Avalonia.Interactivity;
using UI.ViewModels;

namespace UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void PortsComboBox_OnTapped(object? sender, RoutedEventArgs e)
    {
        MainWindowViewModel? mv = DataContext as MainWindowViewModel;
        PortsComboBox.Items = mv?.Ports;
    }
}