using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Excel2JSONCoreLib.JSONConvert;
using Excel2JSONCoreLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2JSONCoreLib.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        // 1. 전면 패널을 켜고 끌 스위치 변수 (자동으로 IsBusy 프로퍼티가 생성됨)
        [ObservableProperty]
        private bool _isBusy;
        // [ObservableProperty]를 붙이면 소문자 logText가 
        // 툴에 의해 대문자 'LogText' 프로퍼티로 자동 생성됩니다.
        [ObservableProperty]
        private string _logText = "프로그램 시작됨...\n";

        

        [ObservableProperty]
        private string _xlsxFolder = string.Empty;

        [ObservableProperty]
        private string _jsonFolder = string.Empty;

        [ObservableProperty]
        private string _clsFolder = string.Empty;

        [ObservableProperty]
        private string _namespaceForClasses = string.Empty;

        private StringBuilder logsb = new StringBuilder();

        private string configfilefullpath;
        public Config config;

        private void syncConfig()
        {
            config.LastExport_Class_Path = ClsFolder;
            config.LastExport_JSON_Path = JsonFolder;
            config.LastExport_Excel_Path = XlsxFolder;
            config.NameSpace = NamespaceForClasses;

        }

        private void saveConfig()
        {
            try
            {
                using (var stringWriter = new StringWriter())
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    // 들여쓰기 활성화
                    jsonWriter.Formatting = Formatting.Indented;

                    // 들여쓰기 문자를 탭('\t')으로 설정
                    jsonWriter.IndentChar = '\t';

                    // 각 계층당 탭 개수 (기본값은 2이므로 1로 지정해야 탭 1개씩 들여씀)
                    jsonWriter.Indentation = 1;

                    var serializer = new JsonSerializer();
                    serializer.Serialize(jsonWriter, config);

                    // config는 크지 않으니 none-async
                    File.WriteAllText(configfilefullpath, stringWriter.ToString());
                }                
            }
            catch(Exception ex)
            {
                AppendLogStringLine(string.Format($"config 파일 저장 실패. - {ex.Message}"));
            }
        }

        public void RestoreFromConfig(string fullpath)
        {
            try
            {
                configfilefullpath = fullpath;

                var str = File.ReadAllText(fullpath);
                var _ro = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(str);
                if (null == _ro)
                {
                    config = new Config();     
                    
                    saveConfig();
                }
                else
                {
                    config = _ro;
                }
                
                XlsxFolder = string.IsNullOrEmpty(config.LastExport_Excel_Path) ? Config.Default_Excel_Path : config.LastExport_Excel_Path;
                JsonFolder = string.IsNullOrEmpty(config.LastExport_JSON_Path) ? Config.Default_JSON_Path : config.LastExport_JSON_Path;
                ClsFolder = string.IsNullOrEmpty(config.LastExport_Class_Path) ? Config.Default_Class_Path : config.LastExport_Class_Path;
                NamespaceForClasses = config.NameSpace;
            }
            catch (Exception ex) 
            {
                // avalonia 의 경우 view 생성자에선 이 처리가 반영이 안됨?
                AppendLogStringLine(string.Format($"Config 파일을 체크하고 프로그램을 다시 실행하세요. - {ex.Message}"));
            }
        }

        public void AppendLogStringLine(string logstr)
        {
            // LogText += $"{logstr}\n"; 보다는 메모리관리 측면에서 아래 방식이 더 나음
            //  TextBox 이기 때문에, 전체 텍스트를 홀드하고 있어야함
            //  ObservableCollection<string>과 ListBox/ListView 사용은, 로그 전문 복사가 편해야해서 일단은 제외, 성능/절약 측면에선 리스트박스가 압도적으로 좋음
            logsb.AppendLine(logstr);
            LogText = logsb.ToString();
        }

        [RelayCommand]
        private async void ExportJSON()
        {
            syncConfig();
            saveConfig();

            IsBusy = true;

            try
            {
                //var rt = await exportJSONTask();

                exportJSONTask(true);
            }
            catch (Exception ex) { IsBusy = false; }
        }

        private async Task<int> exportJSONTask(bool bSetBusyComplete = false)
        {
            // async로 I/O 처럼 올래 걸리는 작업들을 비동기 처리
            //  비록 실제 이 함수 호출측에서 블러킹 되더라도, async await 를 통해 AppendLogStringLine() 등을 통한 LogString TextBox 갱신 처리 반응성을 높인다.

            AppendLogStringLine("==================================================================");
            AppendLogStringLine("Export Json 작업 시작");

            // test code
            await Task.Delay(1000);

            try
            {

                

                foreach (var xf in config.Files)
                {
                    try
                    {
                        var srcname = Path.Combine(config.LastExport_Excel_Path, xf.filename);
                        var ext = Path.GetExtension(srcname);
                        var destPath = Path.Combine(config.LastExport_JSON_Path, xf.filename);
                        var destname = destPath.Replace(ext, ".json");

                        AppendLogStringLine(string.Empty);

                        {
                            var str = JSONGenerator.ExcelToJSONV4(srcname, this);

                            // 암호화된 파일 추가 버전
                            if (!string.IsNullOrEmpty(config.AES_Key))
                            {
                                try
                                {
                                    AppendLogStringLine($"\t 암호화파일 추가 생성 시작");
                                    Cryption cryption = new Cryption(config.AES_Key, config.AES_Iv);
                                    string ciphertxt = cryption.Encrypt(str);

                                    var dfilename = destname + ".bytes";
                                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(new System.IO.FileStream(dfilename, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite)))
                                    {
                                        await sw.WriteLineAsync(ciphertxt);
                                    }
                                    AppendLogStringLine($"\t 암호화파일 추가 생성 완료 {dfilename}");
                                }
                                catch (Exception ex)
                                {
                                    AppendLogStringLine($"\t 암호화파일 추가 생성 오류 {ex.Message}");
                                    AppendLogStringLine($"\t {ex.StackTrace}");
                                }
                            }

                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(new System.IO.FileStream(destname, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite)))
                            {
                                await sw.WriteLineAsync(str);
                            }

                        }
                        AppendLogStringLine(string.Empty);

                    }
                    catch (Exception ex)
                    {
                        AppendLogStringLine(ex.Message);
                        AppendLogStringLine(ex.StackTrace);
                        AppendLogStringLine(string.Empty);
                    }

                }

            }
            catch(Exception ex)
            {
                AppendLogStringLine(ex.Message);
                AppendLogStringLine(ex.StackTrace);
                AppendLogStringLine(string.Empty);

                if(bSetBusyComplete) IsBusy = false;
                return -1;
            }

            AppendLogStringLine("Export Json 작업 완료");
            AppendLogStringLine("==================================================================");
            if (bSetBusyComplete) IsBusy = false;
            return 0;
        }


        [RelayCommand]
        private async void ExportClasses()
        {
            syncConfig();
            saveConfig();

            IsBusy = true;

            try
            {

                //var rt = await exportClassesTask();
                exportClassesTask(true);

            }
            catch (Exception ex) { IsBusy = false; }
        }

        private async Task<int> exportClassesTask(bool bSetBusyComplete=false)
        {
            AppendLogStringLine("==================================================================");
            AppendLogStringLine("Export C# cs 작업 시작");

            // test code
            await Task.Delay(1000);

            try
            {
                foreach (var xf in config.Files)
                {
                    var srcname = Path.Combine(config.LastExport_Excel_Path, xf.filename);
                    var clsname = xf.clsname;
                    if (!Utils.IsValidIdentifier(xf.clsname))
                    {
                        clsname = Path.GetFileNameWithoutExtension(xf.filename);
                        clsname = clsname.Replace('-', '_');
                    }

                    var ext = Path.GetExtension(xf.filename);

                    var destPath = Path.Combine(config.LastExport_Class_Path, xf.filename);
                    var destname = destPath.Replace(ext, ".cs");

                    var str = ClassGenerator.ClassCSGenerator.GenCS(srcname, config.ContainerBaseClassName, config.SeetNodeBaseClassName, clsname, config.NameSpace, this);
                    await File.WriteAllTextAsync(destname, str);
                }
            }
            catch (Exception ex)
            {
                AppendLogStringLine(ex.Message);
                AppendLogStringLine(ex.StackTrace);
                AppendLogStringLine(string.Empty);

                if(bSetBusyComplete) IsBusy = false;
                return -1;
            }

            AppendLogStringLine("Export C# cs 작업 완료");
            AppendLogStringLine("==================================================================");
            if (bSetBusyComplete) IsBusy = false;
            return 0;
        }

        [RelayCommand]
        private async void ExportAll()
        {
            try
            {
                IsBusy = true;
                exportJSONTask();
                exportClassesTask(true);
            }
            catch(Exception ex)
            {
                IsBusy = false;
            }
        }
    }
}
