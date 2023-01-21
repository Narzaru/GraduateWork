using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UI.Views.FixedPoints;

public partial class LoadFixedPointsView : UserControl
{
    public LoadFixedPointsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}