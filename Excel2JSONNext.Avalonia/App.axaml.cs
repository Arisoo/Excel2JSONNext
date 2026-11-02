using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Excel2JSONCoreLib.ViewModels;
using Excel2JSONNext.Avalonia.ViewModels;
using Excel2JSONNext.Avalonia.Views;
using System.Text;

namespace Excel2JSONNext.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        // .net core 에서 ExcelDataReader 라이브러리 사용을 위한
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new JSONConvertWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}