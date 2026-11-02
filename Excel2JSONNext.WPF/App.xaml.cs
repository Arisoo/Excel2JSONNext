using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;

namespace Excel2JSONNext.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // .net core 에서 ExcelDataReader 라이브러리 사용을 위한
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }

}
