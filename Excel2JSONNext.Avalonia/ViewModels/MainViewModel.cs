using CommunityToolkit.Mvvm.ComponentModel;

namespace Excel2JSONNext.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
