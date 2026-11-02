using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Excel2JSONCoreLib.ViewModels;
using System;

namespace Excel2JSONNext.Avalonia.Views
{
    public partial class JSONConvertWindow : Window
    {
        private MainWindowViewModel vm;
        public JSONConvertWindow()
        {
            InitializeComponent();

            // opened 와 Initialized 는 좀 다르지만, 메인윈도우 창은 프로그램 시작시 1번만 뜨니깐.
            //  WPF의 경우엔 View에서 DataContext를 new 하지만, Avalonia는 wrapping한 컴포넌트(Window, UserControl)등에서 처리한다.
            Opened += JSONConvertWindow_Initialized;

        }

        private void JSONConvertWindow_Initialized(object? sender, EventArgs e)
        {
            if (DataContext is MainWindowViewModel docVm)
            {
                vm = docVm;
                vm.RestoreFromConfig("./config.json");
            }
            else
            {
                // 심각한 에러를 띄워야하는데, 이건 프로그래머 잘못임!
            }
        }
    }
}