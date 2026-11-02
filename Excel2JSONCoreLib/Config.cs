using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2JSONCoreLib
{
    /// <summary>
    /// 이 값들은 viewmodel에 연동됨, 따라서 실제 인스턴스는 MainWindowViewModel이 가지고 있어야함
    ///     exe project 에서 config 파일 경로를 안내하는식
    ///     Config.json.template - 형상 관리되는 템플릿
    ///     Config.json - 형상 관리 안되는 로컬상 실제 파일 (사용자 마다 Path 값이 다르니깐)
    /// </summary>
    public partial class Config
    {
        /// <summary>
        /// 컨버팅할 엑셀파일 하나를 정의
        /// </summary>
        public partial class FileInfo
        {
            /// <summary>
            /// 컨버팅 대상 엑셀파일이름
            /// 생성될 json 파일 , cs 파일 이름
            /// </summary>
            public string filename { get; set; }
            

            #region for Classes Generation
            /// <summary>
            /// 클래스파일을 생성할 경우 엑셀파일 1개당 1개의 그룹 클래스가 만들어지는데, 이 이름
            ///     비어있으면 파일이름으로 소문자로 생성(파일이름이 C# 식별자 규칙을 따라야함)
            /// </summary>
            public string clsname { get; set; } = string.Empty;
            #endregion for Classes Generation
        }


        public static readonly string Default_JSON_Path = "./GameDatas";
        public static readonly string Default_Excel_Path = "./GameDataExcels";
        public static readonly string Default_Class_Path = "./Classes/DataTables";

        /// <summary>
        /// 유효한 값이면, AES 암호화하여 컨버팅함
        /// </summary>
        public string AES_Key = string.Empty;
        public string AES_Iv = string.Empty;

        public string LastExport_JSON_Path = string.Empty;
        public string LastExport_Excel_Path = string.Empty;
        public string LastExport_Class_Path = string.Empty;

        /// <summary>
        /// 컨버팅할 엑셀 파일들
        /// </summary>
        public List<FileInfo> Files = new List<FileInfo>();

        #region for Classes Generation

        /// <summary>
        /// 
        /// </summary>
        public string NameSpace = string.Empty;
        /// <summary>
        /// DataTables 계층  seet 실제 행 데이터 클래스들의 베이스 클래스, 비어두어도 무관
        /// </summary>
        public string SeetNodeBaseClassName = string.Empty;
        /// <summary>
        /// xlsx 하나에 대응되는 컨테이너 클래스(시트들을 컨테이닝하는)들의 베이스 클래스, 비어두어도 무관
        /// </summary>
        public string ContainerBaseClassName = string.Empty;

        #endregion for Classes Generation
    }
}
