using Excel2JSONCoreLib.ViewModels;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Excel2JSONNext.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            vm = (MainWindowViewModel)DataContext;

            vm.RestoreFromConfig("./config.json");
        }

        private void LogTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // 텍스트박스가 포커스를 가지지 않은 상태라면 포커스를 강제로 부여
                if (!textBox.IsFocused)
                {
                    textBox.Focus();
                }
            }
        }

        private void ChooseExcelFileFolder_Click(object sender, RoutedEventArgs e)
        {            
            // avalonia 가 아니고, windows folderbrowser는 전형적인 view이니, viewmodel에서 처리 하면 안된다.

            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "엑셀파일 폴더 선택",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
                //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                //InitialDirectory = vm.XlsxFolder
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFolder = dialog.FolderName;
                // 선택한 폴더 경로 처리
                vm.XlsxFolder = selectedFolder;
            }

        }

        private void ChooseJSONFileFolder_Click(object sender, RoutedEventArgs e)
        {
            // avalonia 가 아니고, windows folderbrowser는 전형적인 view이니, viewmodel에서 처리 하면 안된다.

            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "JSON 출력 폴더 선택",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
                //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                //InitialDirectory = vm.JsonFolder
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFolder = dialog.FolderName;
                // 선택한 폴더 경로 처리
                vm.JsonFolder = selectedFolder;
            }
        }

        private void ChooseClassFileFolder_Click(object sender, RoutedEventArgs e)
        {
            // avalonia 가 아니고, windows folderbrowser는 전형적인 view이니, viewmodel에서 처리 하면 안된다.

            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "클래스 cs 출력  폴더 선택",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
                //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                //InitialDirectory = vm.ClsFolder
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFolder = dialog.FolderName;
                // 선택한 폴더 경로 처리
                vm.ClsFolder = selectedFolder;
            }

        }
    }
}