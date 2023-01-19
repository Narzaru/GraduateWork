using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using DynamicData;
using ReactiveUI;

namespace UI.ViewModels;

public class FixedPointsViewModel : ViewModelBase
{
    public FixedPointsViewModel()
    {
        LoadCommand = ReactiveCommand.Create(Load);
        for (int i = 0; i < 10; i++)
        {
            Points.Add("asd");
        }
    }

    public ReactiveCommand<Unit, Unit> LoadCommand { get; }

    public void Load()
    {
        Points.Add("fooo");
    }

    public ObservableCollection<string> Points { get; } = new ObservableCollection<string>();
}