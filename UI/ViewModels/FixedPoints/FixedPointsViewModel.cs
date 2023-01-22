using System.Collections.ObjectModel;
using Data.Models;
using ReactiveUI;

namespace UI.ViewModels.FixedPoints;

public class FixedPointsViewModel : ViewModelBase
{
    public FixedPointsViewModel(MainWindowViewModel parent)
    {
        m_parent = parent;
    }

    public ObservableCollection<Point> Points { get; } = new ObservableCollection<Point>();
    
    #region ParentMainWindow

    public MainWindowViewModel MainWindowParent
    {
        get => m_parent;
        private set => this.RaiseAndSetIfChanged(ref m_parent, value);
    }

    private MainWindowViewModel m_parent;

    public FixedPointsViewModel()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}